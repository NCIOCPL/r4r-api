using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// Describes the information about a resource
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// The resource identifier
        /// </summary>
        /// <value>The identifier.</value>
        [JsonPropertyName("id")]
        public int ID { get; set; }

        /// <summary>
        /// The title of the resource
        /// </summary>
        /// <value>The title.</value>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The URL where you can get more information about the resource
        /// </summary>
        /// <value>The website.</value>
        [JsonPropertyName("website")]
        public string Website { get; set; }

        /// <summary>
        /// The detailed description of the resource
        /// </summary>
        /// <value>The body.</value>
        [JsonPropertyName("body")]
        public string Body { get; set; }

        /// <summary>
        /// A brief description of the resource
        /// </summary>
        /// <value>The description.</value>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// A list of the tool types this resource is categorized as
        /// </summary>
        /// <value>The tool types.</value>
        [JsonPropertyName("toolTypes")]
        public KeyLabel[] ToolTypes { get; set; }

        /// <summary>
        /// A list of the sub tool types this resource is categorized as
        /// </summary>
        /// <value>The tool subtypes.</value>
        [JsonPropertyName("toolSubtypes")]
        public ToolSubtype[] ToolSubtypes { get; set; }

        /// <summary>
        /// A list of the areas of research areas where this resource applies
        /// </summary>
        /// <value>The research areas.</value>
        [JsonPropertyName("researchAreas")]
        public KeyLabel[] ResearchAreas { get; set; }

        /// <summary>
        /// A list of the areas of research types where this resource applies
        /// </summary>
        /// <value>The research types.</value>
        [JsonPropertyName("researchTypes")]
        public KeyLabel[] ResearchTypes { get; set; }

        /// <summary>
        /// Information about how one can aquire access to the resource
        /// </summary>
        /// <value>The resource access.</value>
        [JsonPropertyName("resourceAccess")]
        public ResourceAccess ResourceAccess { get; set; }

        /// <summary>
        /// A list of the NCI Divisions, Offices and Centers (DOCs) that manage the resource
        /// </summary>
        /// <value>The document.</value>
        [JsonPropertyName("docs")]
        public KeyLabel[] DOCs { get; set; }

        /// <summary>
        /// A list of points of contact for the resource
        /// </summary>
        /// <value>The POC.</value>
        [JsonPropertyName("pocs")]
        public Contact[] POCs { get; set; }
    }
}
