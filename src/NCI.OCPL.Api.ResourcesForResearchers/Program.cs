using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using NCI.OCPL.Api.Common;

namespace NCI.OCPL.Api.ResourcesForResearchers
{
    /// <summary>
    /// The R4R API
    /// </summary>
    public class Program : NciApiProgramBase
    {
        /// <summary>
        /// The main entry point for running the API.
        /// </summary>
        public static void Main(string[] args)
        {
            CreateHostBuilder<Startup>(args).Build().Run();
        }
    }
 }
