using System.Text.Json.Nodes;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport.Extensions;
using Xunit;

namespace NCI.OCPL.Utils.Testing
{

    /// <summary>
    /// Tools for mocking elasticsearch clients
    /// </summary>
    public static class QueryComparerUtil
    {

        /// <summary>
        /// Asserts that a Query Container matches the JSON represented as expectedStr.
        /// </summary>
        /// <param name="expectedStr">The JSON representing the expected query</param>
        /// <param name="query">The query object</param>
        public static void AssertQueryJson(string expectedStr, Query query)
        {
            JsonNode expected = JsonNode.Parse(expectedStr);

            ElasticsearchClient client = new ElasticsearchClient();
            string json = client.RequestResponseSerializer.SerializeToString(query);

            JsonNode actual = JsonNode.Parse(json);

            Assert.True(JsonNode.DeepEquals(expected, actual));
        }

    }
}