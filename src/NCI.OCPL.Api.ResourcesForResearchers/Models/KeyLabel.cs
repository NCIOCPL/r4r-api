using System.Text.Json.Serialization;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// Describes an item's key and label
    /// </summary>
    public class KeyLabel
    {
        /// <summary>
        /// The key of the item
        /// </summary>
        /// <value>The key.</value>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// The label of the item
        /// </summary>
        /// <value>The label.</value>
        [JsonPropertyName("label")]
        public string Label { get; set; }
    }
}
