﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NCI.OCPL.Api.Common;

using NCI.OCPL.Api.ResourcesForResearchers.Models;

using Nest;

namespace NCI.OCPL.Api.ResourcesForResearchers.Services
{
    /// <summary>
    /// Service for fetching R4R Resources
    /// </summary>
    public class ESResourceQueryService : ESResourceServiceBase, IResourceQueryService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:R4RAPI.Services.ESResourceQueryService"/> class.
        /// </summary>
        /// <param name="client">A configured Elasticsearch client</param>
        /// <param name="apiOptionsAccessor">An accessor for the API options</param>
        /// <param name="logger">A logger for logging.</param>
        public ESResourceQueryService(IElasticClient client, IOptions<R4RAPIOptions> apiOptionsAccessor, ILogger<ESResourceQueryService> logger)
            : base(client, apiOptionsAccessor, logger) { }

        /// <summary>
        /// Asynchronously gets a resource from the API via its ID.
        /// </summary>
        /// <param name="id">The ID of the resource</param>
        /// <returns>The resource</returns>
        public async Task<Resource> GetAsync(string id)
        {
            Resource resResult = null;

            // If the given ID is null or empty, throw an exception.
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("The resource identifier is null or an empty string.");
            }

            // Validate if given ID is correctly formatted as an int.
            int resID;
            bool validID = int.TryParse(id, out resID);

            if (validID)
            {
                GetResponse<Resource> response = null;

                try
                {
                    // Fetch the resource with the given ID from the API.
                    response = await _elasticClient.GetAsync<Resource>(new GetRequest(this._apiOptions.AliasName, resID));
                }
                catch (Exception ex)
                {
                    // Throw an exception if an error occurs.
                    _logger.LogError(ex, $"Could not fetch resource ID {resID}");
                    throw new APIErrorException(500, "Could not fetch resource ID " + resID);
                }

                // If the API's response isn't valid, throw an error and return 500 status code.
                if (!response.ApiCall.Success)
                {
                    _logger.LogDebug($"Failed Elasticsearch lookup for id '{id}'.\nDetails:\n\n{response.ApiCall.DebugInformation}");
                    throw new APIErrorException(500, "Errors occurred.");
                }

                // If no resource was found, throw a 404 error.
                // This really should be decided by the controller, but refactoring is out of scope for now.
                if (!response.Found)
                {
                    throw new APIErrorException(404, "Resource not found.");
                }

                // Return the resource.
                resResult = response.Source;
            }
            else
            {
                // Throw an exception if the given ID is invalid (not an int).
                throw new APIErrorException(400, "The resource identifier is invalid.");
            }

            return resResult;
        }

        /// <summary>
        /// Asynchronously gets the resources that match the given params
        /// </summary>
        /// <param name="query">Query parameters (optional)</param>
        /// <param name="size">Number of results to return (optional)</param>
        /// <param name="from">Beginning index for results (optional)</param>
        /// <param name="includeFields">Fields to include (optional)</param>
        /// <returns>Resource query result</returns>
        public async Task<ResourceQueryResult> QueryResourcesAsync(
            ResourceQuery query,
            int size = 20,
            int from = 0,
            string[] includeFields = null
            )
        {
            ResourceQueryResult queryResults = new ResourceQueryResult();

            // Set up the SearchRequest to send to the API.
            Indices index = Indices.Index(new string[] { this._apiOptions.AliasName });
            SearchRequest request = new SearchRequest(index)
            {
                Size = size,
                From = from,
                Sort = new List<ISort>
                {
                    new FieldSort { Field = "title._sort", Order = SortOrder.Ascending }
                },
                //TODO:
                Source = new SourceFilter
                {
                    Includes = includeFields
                }
            };

            //Add in the query
            var searchQuery = this.GetFullQuery(query.Keyword, query.Filters);
            if (searchQuery != null)
            {
                request.Query = searchQuery;
            }

            ISearchResponse<Resource> response = null;

            try
            {
                // Fetch the resources that match the given query and parameters from the API.
                response = await _elasticClient.SearchAsync<Resource>(request);
            }
            catch (Exception ex)
            {
                //TODO: Update error logger to include query
                _logger.LogError(ex, "Could not fetch resources for query.");
                throw new APIErrorException(500, "Could not fetch resources for query.");
            }

            // If the API's response isn't valid, throw an error and return 500 status code.
            if (!response.IsValid)
            {
                _logger.LogError("Bad request.");
                throw new APIErrorException(500, "Errors occurred.");
            }

            // If the API finds resources matching the params, build the ResourceQueryResult to return.
            if (response.Total > 0)
            {
                // Build the array of resources for the returned restult.
                List<Resource> resourceResults = new List<Resource>();
                foreach (Resource res in response.Documents)
                {
                    resourceResults.Add(res);
                }

                queryResults.Results = resourceResults.ToArray();
                queryResults.TotalResults = Convert.ToInt32(response.Total);
                queryResults.From = from;
            }

            return queryResults;
        }

    }
}
