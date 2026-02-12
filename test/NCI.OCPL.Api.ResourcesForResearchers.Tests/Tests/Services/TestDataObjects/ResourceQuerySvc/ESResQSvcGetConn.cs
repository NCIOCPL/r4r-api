using NCI.OCPL.Api.Common.Testing;

namespace NCI.OCPL.Api.ResourcesForResearchers.Tests.Services
{
    /// <summary>
    /// Class used for mocking BestBet Match requests to Elasticsearch.  This should be
    /// used as the base class of test specific Connections object passed into an ElasticClient.
    /// </summary>
    /// <seealso cref="NCI.OCPL.Utils.Testing.ElasticsearchInterceptingConnection" />
    public class ESResQSvcGetConn : InMemoryConnection
    {

        /// <summary>
        /// Creates a new instance of the ESResQSvcGetConn class
        /// </summary>
        /// <param name="testFile">The JSON file for the test response</param>
        /// <param name="status">The HTTP status code for the mock response</param>
        public ESResQSvcGetConn(string testFile, int status = 200)
            : base(TestingTools.GetTestFileAsBytes($"ESResQuerySvcData/{testFile}.json"), statusCode: status)
        {
        }
    }
}