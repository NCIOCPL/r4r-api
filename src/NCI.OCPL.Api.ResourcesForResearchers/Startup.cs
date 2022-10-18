using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
// using Nest;
// using Elasticsearch.Net;
// using NSwag.AspNetCore;
using NJsonSchema;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

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

        // /// <summary>
        // /// Configures the services.
        // /// </summary>
        // /// <param name="services">Services.</param>
        // // This method gets called by the runtime. Use this method to add services to the container.
        // public void ConfigureServices(IServiceCollection services)
        // {
        //     services.AddLogging();
        //     services.AddOptions();
        //     services.AddCors();

        //     //This allows us to easily generate URLs to routes
        //     services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        //     services.AddScoped<IUrlHelper>(factory =>
        //     {
        //         var actionContext = factory.GetService<IActionContextAccessor>()
        //                                    .ActionContext;
        //         return new UrlHelper(actionContext);
        //     });


        //     services.AddMvc();
        // }

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

        /// <summary>
        /// Configure the specified app and env.
        /// </summary>
        /// <returns>The configure.</returns>
        /// <param name="app">App.</param>
        /// <param name="env">Env.</param>
        /// <param name="loggerFactory">Logger.</param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        // {
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }

            // app.UseStaticFiles();
            // // Enable the Swagger UI middleware and the Swagger generator
            // app.UseSwaggerUi3(typeof(Startup).GetTypeInfo().Assembly, settings =>
            // {
            //     settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;

            //     if(!string.IsNullOrEmpty(Configuration["NSwag:Title"]))
            //     {
            //         settings.GeneratorSettings.Title = Configuration["NSwag:Title"];
            //     }

            //     if (!string.IsNullOrEmpty(Configuration["NSwag:Description"]))
            //     {
            //         settings.GeneratorSettings.Description = Configuration["NSwag:Description"];
            //     }

            //     settings.SwaggerUiRoute = "";
            //     settings.PostProcess = document =>
            //     {
            //         document.Host = null;
            //     };
            // });

            // // Allow use from anywhere.
            // app.UseCors(builder => builder.AllowAnyOrigin());

            // // This is equivelant to the old Global.asax OnError event handler.
            // // It will handle any unhandled exception and return a status code to the
            // // caller.  IF the error is of type APIErrorException then we will also return
            // // a message along with the status code.  (Otherwise we )
            // app.UseExceptionHandler(errorApp => {
            //     errorApp.Run(async context => {
            //         context.Response.StatusCode = 500; // or another Status accordingly to Exception Type
            //         context.Response.ContentType = "application/json";

            //         var error = context.Features.Get<IExceptionHandlerFeature>();

            //         if (error != null)
            //         {
            //             var ex = error.Error;

            //             //Unhandled exceptions may not be sanitized, so we will not
            //             //display the issue.
            //             string message = "Errors have occurred.  Type: " + ex.GetType().ToString();

            //             //Our own exceptions should be sanitized enough.
            //             if (ex is APIErrorException)
            //             {
            //                 context.Response.StatusCode = ((APIErrorException)ex).HttpStatusCode;
            //                 message = ex.Message;
            //             }

            //             byte[] contents = Encoding.UTF8.GetBytes(new ErrorMessage()
            //             {
            //                 Message = message
            //             }.ToString());


            //             // THIS IS A HACK!!
            //             // When the pull request that fixes the timing of setting the CORS header (https://github.com/aspnet/CORS/pull/163) goes through,
            //             // we should remove this and test to see if it works without the hack.
            //             if (context.Request.Headers.ContainsKey("Origin"))
            //             {
            //                 context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //             }

            //             await context.Response.Body.WriteAsync(contents, 0, contents.Length);
            //         }
            //     });
            // });

            //app.UseMvc();
        // }

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
