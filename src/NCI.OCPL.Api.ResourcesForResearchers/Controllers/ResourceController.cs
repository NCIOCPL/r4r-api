using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using NCI.OCPL.Api.Common;

using NCI.OCPL.Api.ResourcesForResearchers.Models;
using NCI.OCPL.Api.ResourcesForResearchers.Services;

namespace NCI.OCPL.Api.ResourcesForResearchers.Controllers
{
    /// <summary>
    /// The Resource Enpoint Controller
    /// </summary>
    [Produces("application/json")]
    [Route("resource")]
    public class ResourceController : Controller
    {
        private IWebHostEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IResourceQueryService _queryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:R4RAPI.Controllers.ResourceController"/> class.
        /// </summary>
        /// <param name="environment">Environment.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="queryService">Query service.</param>
        public ResourceController(IWebHostEnvironment environment, ILogger<ResourcesController> logger, IResourceQueryService queryService)
        {
            _environment = environment;
            _logger = logger;
            _queryService = queryService;
        }

        /// <summary>
        /// Gets a Resource by the identifier.
        /// </summary>
        /// <returns>The by identifier.</returns>
        /// <param name="id">The resource's identifier.</param>
        [HttpGet("{id}")]
        public async Task<Resource> GetById(int id)
        {
            if (id < 1)
            {
                throw new APIErrorException(400, "The resource identifier is invalid.");
            }

            Resource result = await _queryService.GetAsync(id);

            if(result == null)
            {
                _logger.LogError($"Could not fetch resource for ID {id}");
                throw new APIErrorException(404, $"Could not fetch resource for ID {id}");
            }

            return result;
        }
    }
}