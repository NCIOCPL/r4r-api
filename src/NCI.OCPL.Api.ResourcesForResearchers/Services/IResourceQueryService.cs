﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCI.OCPL.Api.ResourcesForResearchers.Models;

namespace NCI.OCPL.Api.ResourcesForResearchers.Services
{
    /// <summary>
    /// Interface for a Resource query service.
    /// </summary>
    public interface IResourceQueryService
    {

        /// <summary>
        /// Asynchronously gets a resource from the API via its ID.
        /// </summary>
        /// <param name="id">The ID of the resource</param>
        /// <returns>The resource</returns>
        Task<Resource> GetAsync(string id);

        /// <summary>
        /// Asynchronously gets the resources that match the given params
        /// </summary>
        /// <param name="query">Query parameters (optional)</param>
        /// <param name="size">Number of results to return (optional)</param>
        /// <param name="from">Beginning index for results (optional)</param>
        /// <param name="includeFields">Fields to include (optional)</param>
        /// <returns>Resource query result</returns>
        Task<ResourceQueryResult> QueryResourcesAsync(
            ResourceQuery query,
            int size = 20,
            int from = 0,
            string[] includeFields = null
        );

    }
}
