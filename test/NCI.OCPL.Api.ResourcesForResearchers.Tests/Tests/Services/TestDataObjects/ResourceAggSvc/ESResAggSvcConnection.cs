using NCI.OCPL.Api.Common.Testing;

namespace NCI.OCPL.Api.ResourcesForResearchers.Tests.Services
{
    /// <summary>
    /// Class used for mocking BestBet Match requests to Elasticsearch.  This should be
    /// used as the base class of test specific Connections object passed into an ElasticClient.
    /// </summary>
    /// <seealso cref="NCI.OCPL.Utils.Testing.ElasticsearchInterceptingConnection" />
    public class ESResAggSvcConnection : InMemoryConnection
    {

        /// <summary>
        /// Creates a new instance of the ESResAggSvcConnection class
        /// </summary>
        /// <param name="testFile">The JSON file for the test response</param>
        public ESResAggSvcConnection(string testFile)
            : base(TestingTools.GetTestFileAsBytes($"ESResAggSvcData/{testFile}.json"), statusCode: 200)
        {
        }
    }
}