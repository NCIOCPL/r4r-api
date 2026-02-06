using System;
using System.Text.Json.Serialization;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// Contains the information about a name of a point of contact
    /// </summary>
    public class Name
    {
        /// <summary>
        /// The name prefix of the contact
        /// </summary>
        /// <value>The prefix.</value>
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// The first name of the contact
        /// </summary>
        /// <value>The first name.</value>
        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        /// <summary>
        /// The middle name of the contact
        /// </summary>
        /// <value>The middle name.</value>
        [JsonPropertyName("middlename")]
        public string MiddleName { get; set; }

        /// <summary>
        /// The last name of the contact
        /// </summary>
        /// <value>The last name.</value>
        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        /// <summary>
        /// The name suffix of the contact
        /// </summary>
        /// <value>The suffix.</value>
        [JsonPropertyName("suffix")]
        public string Suffix { get; set; }
    }
}
