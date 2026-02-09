#nullable enable // This should be removed once we move to nullables globally.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

using NCI.OCPL.Api.ResourcesForResearchers.Models;


namespace NCI.OCPL.Api.ResourcesForResearchers.Services
{

    /// <summary>
    /// Base class for all ElasticSearch based ResourceServices
    /// </summary>
    public abstract class ESResourceServiceBase
    {
        /// <summary>
        /// The elasticsearch client
        /// </summary>
        protected readonly ElasticsearchClient _elasticClient;

        /// <summary>
        /// The API options.
        /// </summary>
        protected readonly R4RAPIOptions _apiOptions;

        /// <summary>
        /// A logger to use for logging
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:R4RAPI.Services.ESResourceServiceBase"/> class.
        /// </summary>
        /// <param name="client">An instance of a <see cref="T:Nest.ElasticClient"/>Client.</param>
        /// <param name="apiOptionsAccessor">API options accessor.</param>
        /// <param name="logger">Logger.</param>
        public ESResourceServiceBase(ElasticsearchClient client, IOptions<R4RAPIOptions> apiOptionsAccessor, ILogger logger)
        {
            this._elasticClient = client;
            this._apiOptions = apiOptionsAccessor.Value.IsValid() ? apiOptionsAccessor.Value : throw new Exception("R4RAPIOptions is misconfigured.");
            this._logger = logger;
        }

        /// <summary>
        /// Gets the complete query with the keyword part and the filters part.
        /// </summary>
        /// <remarks>This is used by both the Query and Aggregation services.</remarks>
        /// <returns>A QueryContainer representing the entire query.  </returns>
        /// <param name="keyword">Keyword for the search</param>
        /// <param name="filtersList">The complete filters list</param>
        protected Query? GetFullQuery(string keyword, Dictionary<string, string[]> filtersList) {
            Query? query = null;

            Query? keywordQuery = GetKeywordQuery(keyword);
            ICollection<Query> filtersQueries = GetAllFiltersForQuery(filtersList);

            if (keywordQuery != null && filtersQueries.Count > 0) {
                query = new BoolQuery
                {
                    Filter = filtersQueries,
                    Must = new Query[] { keywordQuery }
                };
            } else if (keywordQuery != null) {
                query = keywordQuery;
            } else if (filtersQueries.Count > 0) {
                query = new BoolQuery
                {
                    Filter = filtersQueries
                };
            } //Else there is no query.

            return query;
        }

        /// <summary>
        /// Gets a query object to be used for all filters.
        /// </summary>
        /// <remarks>
        /// When more than one filter is used we must use a Bool query (Must) to wrap the
        /// TermQuery objects that represent the filters. When only one filter is used,
        /// then we only need to return a single TermQuery.
        /// </remarks>
        /// <returns>All of the filters for this query.  This is something that can be used for the filter
        /// portion of a bool query.</returns>
        /// <param name="filtersList">A dictionary containing of all of the filters.
        /// The key should be the name of the field to filter.
        /// The values are a list of all of the filters.
        /// </param>
        protected ICollection<Query> GetAllFiltersForQuery(Dictionary<string,string[]> filtersList) {

            //NOTE: This assumes there are not dependencies between fields. (e.g. toolType & toolSubtype)
            //Therefore we are not required to do any complicated nested queries. This will work if all
            //the keys of the filters are unique.
            //e.g. toolType: foo|toolSubtype: bar && toolType: bazz| toolSubtype: bar would not work.
            ICollection<Query> queries = new Query[]{};

            if (filtersList.Count == 1) {
                KeyValuePair<string, string[]> filter = filtersList.First();
                queries = new Query[] { GetQueryForFilterField($"{filter.Key}.key", filter.Value) };
            } else if (filtersList.Count > 1) {
                queries = (from filter in filtersList
                          select GetQueryForFilterField($"{filter.Key}.key", filter.Value)).ToList();
            }

            return queries;
        }

        /// <summary>
        /// Gets a query object used for filtering a field given one or more filters
        /// </summary>
        /// <remarks>
        /// When more than one filter is used we must use a Bool query (Should) to wrap the
        /// TermQuery objects that represent the filters. When only one filter is used,
        /// then we only need to return a single TermQuery.
        /// </remarks>
        /// <returns>The QueryContainer to be used by the filter.</returns>
        /// <param name="field">The field to filter on.</param>
        /// <param name="filters">The filters to turn into the query</param>
        /// <exception cref="ArgumentNullException">If there are 0 items in the filters list</exception>
        protected Query GetQueryForFilterField(string field, string[] filters) {
            Query? query = null;

            if (filters.Length == 0)
            {
                throw new ArgumentException("Filters must contain at least one item");
            }

            if (filters.Length == 1)
            {
                //There is only one, so it can just be a term query.
                query = GetQueryForField(field, filters[0]);
            }
            else
            {
                query = new BoolQuery {
                    Should = (from filter in filters
                                select (Query)GetQueryForField(field, filter)).ToList(),
                    MinimumShouldMatch = 1
                };
            }

            return query!;
        }

        /// <summary>
        /// Gets a TermQuery for a given field.
        /// </summary>
        /// <returns>The query for field.</returns>
        /// <param name="field">Field.</param>
        /// <param name="value">Value.</param>
        protected TermQuery GetQueryForField(string field, string value)
        {
            TermQuery query = new TermQuery
            {
                Field = field,
                Value = value
            };

            return query;
        }

        /// <summary>
        /// Gets the keyword part of the query.
        /// </summary>
        /// <returns>The keyword query.</returns>
        /// <param name="keyword">Keyword.</param>
        protected Query? GetKeywordQuery(string keyword)
        {
            // Get list of full text fields from options for query building
            R4RAPIOptions.FullTextFieldConfig[] fullTextFieldsList;
            try
            {
                fullTextFieldsList = this._apiOptions.AvailableFullTextFields.Select(f => f.Value).ToArray();
            }
            catch (Exception ex)
            {
                this._logger.LogError("Could not fetch full text fields from configuration.");
                throw new Exception("Could not fetch full text fields from configuration.", ex);
            }

            Query? query = null;
            if (!string.IsNullOrEmpty(keyword))
            {
                query = new BoolQuery
                {
                    Should = GetFullTextQuery(keyword, fullTextFieldsList)
                };
            }
            return query;
        }

        /// <summary>
        /// Gets a list of QueryContainers for all fulltext fields.
        /// </summary>
        /// <returns>The QueryContainers for all fulltext fields.</returns>
        /// <param name="keyword">Keyword text.</param>
        /// <param name="fields">Full-text fields.</param>
        protected ICollection<Query> GetFullTextQuery(string keyword, R4RAPIOptions.FullTextFieldConfig[] fields)
        {
            Query[] fullTextFieldQueries = fields.SelectMany(f => GetQueryForFullTextField(f.FieldName, keyword, f.Boost, f.MatchTypes)).ToArray();

            return fullTextFieldQueries;
        }

        /// <summary>
        /// Gets a QueryContainer for a given fulltext field.
        /// </summary>
        /// <returns>The query for the fulltext field.</returns>
        /// <param name="field">Field.</param>
        /// <param name="keyword">Keyword text.</param>
        /// <param name="boost">Boost.</param>
        /// <param name="matchTypes">Match types.</param>
        protected ICollection<Query> GetQueryForFullTextField(string field, string keyword, int boost, string[] matchTypes)
        {
            ICollection<Query> fullTextFieldQuery = (from matchType in matchTypes
                                 select GetQueryForMatchType(field, keyword, boost, matchType)).ToList();

            return fullTextFieldQuery;
        }

        /// <summary>
        /// Gets a QueryContainer for a given fulltext field's match type.
        /// </summary>
        /// <returns>The query for the specific match type of the fulltext field.</returns>
        /// <param name="field">Field.</param>
        /// <param name="keyword">Keyword text.</param>
        /// <param name="boost">Boost.</param>
        /// <param name="matchType">Match type.</param>
        protected Query GetQueryForMatchType(string field, string keyword, int boost, string matchType)
        {
            switch(matchType)
            {
                case "common":
                    // This will break if/when we move to Elasticsearch 8.x.
                    // The docs say we should replace CommonTermsQuery with MatchQuery.
                    // This will require additional/updated tests.
#pragma warning disable CS0618
                    return new CommonTermsQuery
                    {
                        Field = field,
                        Query = keyword,
                        Boost = boost,
                        CutoffFrequency = 1
                    };
#pragma warning restore CS0618
                case "match":
                    return new MatchQuery
                    {
                        Field = field,
                        Query = keyword,
                        Boost = boost
                    };
                case "match_phrase":
                    return new MatchPhraseQuery
                    {
                        Field = field,
                        Query = keyword,
                        Boost = boost
                    };
                default:
                    throw new ArgumentException($"Given match type {matchType} is for field {field} is invalid: must be common, match, or match_phrase.");
            }
        }
    }
}
