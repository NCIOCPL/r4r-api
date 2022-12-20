using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Elasticsearch.Net;
using Nest;

using Moq;
using Newtonsoft.Json.Linq;
using Xunit;
using Newtonsoft.Json;

using NCI.OCPL.Api.Common.Testing;

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
        public static void AssertQueryJson(string expectedStr, QueryContainer query)
        {
            JObject expected = JObject.Parse(expectedStr);

            IElasticClient client = new ElasticClient();
            string json = client.RequestResponseSerializer.SerializeToString(query);

            JObject actual = JObject.Parse(json);

            Assert.Equal(expected, actual, new JTokenEqualityComparer());
        }

    }
}