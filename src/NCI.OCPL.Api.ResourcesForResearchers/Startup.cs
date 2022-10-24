using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.ResourcesForResearchers.Models;
using NCI.OCPL.Api.ResourcesForResearchers.Services;

namespace NCI.OCPL.Api.ResourcesForResearchers
{
    /// <summary>
    /// The API Startup.
    /// </summary>
    public class Startup : NciStartupBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:NCI.OCPL.Api.ResourcesForResearchers.Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Startup(IConfiguration configuration)
            : base(configuration) { }


        /*****************************
         * ConfigureServices methods *
         *****************************/

        /// <summary>
        /// Adds the configuration mappings.
        /// </summary>
        /// <param name="services">Services.</param>
        protected override void AddAdditionalConfigurationMappings(IServiceCollection services)
        {
        }

        /// <summary>
        /// Adds in the application specific services
        /// </summary>
        /// <param name="services">Services.</param>
        protected override void AddAppServices(IServiceCollection services)
        {
            //This allows us to easily generate URLs to routes
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                            .ActionContext;
                return new UrlHelper(actionContext);
            });

            // Add our Query Services
            services.AddSingleton<IResourceQueryService, ESResourceQueryService>();
            services.AddSingleton<IResourceAggregationService, ESResourceAggregationService>();

            services.Configure<R4RAPIOptions>(Configuration.GetSection("R4RAPI"));
        }


        /*****************************
         *     Configure methods     *
         *****************************/

        /// <summary>
        /// Configure the specified app and env.
        /// </summary>
        /// <returns>The configure.</returns>
        /// <param name="app">App.</param>
        /// <param name="env">Env.</param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        protected override void ConfigureAppSpecific(IApplicationBuilder app, IWebHostEnvironment env)
        {
            return;
        }
    }
}
