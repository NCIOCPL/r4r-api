#nullable enable // This should be removed once we move to nullables globally.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;

using NCI.OCPL.Api.ResourcesForResearchers.Models;

namespace NCI.OCPL.Api.ResourcesForResearchers.Services
{
    /// <summary>
    /// Service for fetching R4R Resource Aggregations
    /// </summary>
    public class ESResourceAggregationService : ESResourceServiceBase, IResourceAggregationService
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:R4RAPI.Services.ESResourceAggregationService"/> class.
        /// </summary>
        /// <param name="client">A configured Elasticsearch client</param>
        /// <param name="apiOptionsAccessor">The R4RAPIOptions Accessor</param>
        /// <param name="logger">A logger for logging.</param>
        public ESResourceAggregationService(ElasticsearchClient client, IOptions<R4RAPIOptions> apiOptionsAccessor, ILogger<ESResourceAggregationService> logger)
            : base(client, apiOptionsAccessor, logger) {}


        /// <summary>
        /// Asynchronously gets the key label aggregation for a field
        /// </summary>
        /// <param name="field">The field to aggregate</param>
        /// <param name="query">The query for the results</param>
        /// <returns>The aggregation items</returns>
        public async Task<KeyLabelAggResult[]> GetKeyLabelAggregationAsync(string field, ResourceQuery query) {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));

            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (!this._apiOptions.AvailableFacets.ContainsKey(field))
            {
                throw new ArgumentException($"Field, {field}, does not have any configuration");
            }

            //Get config
            R4RAPIOptions.FacetConfig facetConfig = this._apiOptions.AvailableFacets[field];

            //Make sure we have a valid config.
            if (facetConfig == null)
            {
                throw new ArgumentNullException($"Facet configuration could not be found for {field}");
            }

            if (
                !string.IsNullOrWhiteSpace(facetConfig.RequiresFilter) &&
                (!query.Filters.ContainsKey(facetConfig.RequiresFilter) || query.Filters[facetConfig.RequiresFilter].Length == 0)
            )
            {
                throw new ArgumentException($"Facet, {facetConfig.FilterName}, requires filter, {facetConfig.RequiresFilter}");
            }

            SearchRequest req = new SearchRequest(this._apiOptions.AliasName)
            {
                Size = 0, //req.Size = 0; //Set the size to 0 in order to return no Resources

                // Let's check if the field has a period if it does, then we need
                // to handle it differently because it is either toolType.type or
                // toolType.subtype.
                Aggregations = GetAggQuery(facetConfig, query)
            };

            var searchQuery = GetSearchQueryForFacet(facetConfig.FilterName, query);
            if (searchQuery != null)
            {
                req.Query = searchQuery;
            }

            try
            {
                //We must(?) pass the C# type to map the results to
                //even though our queries should not return anything.
                var res = await this._elasticClient.SearchAsync<Resource>(req);

                return ExtractAggResults(facetConfig, res).ToArray();
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Could not fetch aggregates for field: {field}");
                throw new Exception($"Could not fetch aggregates for field: {field}", ex);
            }
        }

        /// <summary>
        /// Gets a search query (the Query portion of a SearchRequest) with the
        /// requested facet's filter removed.
        /// </summary>
        /// <returns>The search query for facet.</returns>
        /// <param name="field">The facet filter being requested.</param>
        /// <param name="resourceQuery">Resource query.</param>
        private Query? GetSearchQueryForFacet(string field, ResourceQuery resourceQuery)
        {
            Query? query = null;

            var filteredFilters = from filter in resourceQuery.Filters
                                  where filter.Key != field
                                  select filter;

            query = this.GetFullQuery(
                resourceQuery.Keyword,
                new Dictionary<string, string[]>(filteredFilters)
            );

            return query;
        }

        /// <summary>
        /// Helper function to extract the aggs for a simple (e.g. non-toolType) aggregation
        /// </summary>
        /// <returns>The simple aggs.</returns>
        /// <param name="facetConfig">Configuration for the field being aggregating</param>
        /// <param name="res">Res.</param>
        private IEnumerable<KeyLabelAggResult> ExtractAggResults(R4RAPIOptions.FacetConfig facetConfig, SearchResponse<Resource> res)
        {

            var nestedAgg = res.Aggregations!.GetNested($"{facetConfig.FilterName}_agg");
            var currBucket = nestedAgg!.Aggregations!;

            //We need to go one level deeper if this has a dependent filter
            if (!String.IsNullOrWhiteSpace(facetConfig.RequiresFilter)) {
                var filterAgg = currBucket.GetFilter($"{facetConfig.FilterName}_filter");
                currBucket = filterAgg!.Aggregations!;
            }

            var keys = currBucket.GetStringTerms($"{facetConfig.FilterName}_key");

            foreach(var keyBucket in keys!.Buckets){
                long count = keyBucket.DocCount;
                string key = keyBucket.Key.ToString();

                var label = "";
                var labelBuckets = keyBucket.Aggregations!.GetStringTerms($"{facetConfig.FilterName}_label")!.Buckets;
                if (labelBuckets.Count() > 0)
                {
                    label = labelBuckets.First().Key.ToString();
                }

                yield return new KeyLabelAggResult()
                {
                    Key = key,
                    Label = label,
                    Count = count
                };
            }
        }

        /// <summary>
        /// Gets the simple aggregation "query"
        /// </summary>
        /// <returns>An AggregationDictionary containing the "query"</returns>
        /// <param name="facetConfig">Configuration for the field to aggregate</param>
        /// <param name="query"></param>
        private IDictionary<string, Aggregation> GetAggQuery(R4RAPIOptions.FacetConfig facetConfig, ResourceQuery query)
        {

            // If we *really* need the parentKey for this facet, then we must add it to the aggregation.
            // however, we may not really need it.
            var keyLabelAggregate = new Dictionary<string, Aggregation>
            {
                { $"{facetConfig.FilterName}_key", new Aggregation
                {
                    Terms = new TermsAggregation
                    {
                        Field = $"{facetConfig.FilterName}.key", //Set the field to rollup
                        Size = 999 //Use a large number to indicate unlimted (def is 10)
                    },
                    // Now, we need to get the labels for the keys and thus
                    // we need to add a sub aggregate for this term.
                    // Normally you would do this for something like city/state rollups
                    Aggregations = new Dictionary<string, Aggregation>
                    {
                        { $"{facetConfig.FilterName}_label", new Aggregation
                        {
                            Terms = new TermsAggregation
                            {
                                Field = $"{facetConfig.FilterName}.label"
                            }
                        }}
                    }
                }}
            };

            IDictionary<string, Aggregation> aggBody = keyLabelAggregate;
            // This facet requires a parent and thus needs a filter aggregate
            // to wrap the keyLabelAggregate
            if (!String.IsNullOrWhiteSpace(facetConfig.RequiresFilter)) {
                aggBody = new Dictionary<string, Aggregation>
                {
                    { $"{facetConfig.FilterName}_filter", new Aggregation
                    {
                        Filter = this.GetQueryForFilterField($"{facetConfig.FilterName}.parentKey", query.Filters[facetConfig.RequiresFilter]),
                        Aggregations = keyLabelAggregate
                    }}
                };
            }

            //Start with a nested aggregation
            return new Dictionary<string, Aggregation>
            {
                { $"{facetConfig.FilterName}_agg", new Aggregation
                {
                    Nested = new NestedAggregation
                    {
                        Path = facetConfig.FilterName //Set the path of the nested agg
                    },
                    // Add the sub aggregates (bucket keys)
                    Aggregations = aggBody
                }}
            };
        }

    }
}
