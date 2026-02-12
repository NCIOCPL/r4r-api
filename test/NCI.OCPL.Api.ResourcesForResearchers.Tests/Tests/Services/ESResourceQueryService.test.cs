using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using Elastic.Transport;
using FluentAssertions;
using Xunit;

using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.ResourcesForResearchers.Models;
using NCI.OCPL.Api.ResourcesForResearchers.Services;

namespace NCI.OCPL.Api.ResourcesForResearchers.Tests.Services
{
  public class ESResourceQueryService_Tests : TestESResourceServiceBase
  {
    #region Test Gets

    [Fact]
    public async Task Get_ValidID()
    {

      //Create new ESRegAggConnection...

      InMemoryConnection conn = new ESResQSvcGetConn("Resource_101");

      //Expected Aggs
      Resource expected = StaticResourceData.GetRes101();

      ESResourceQueryService svc = this.GetService<ESResourceQueryService>(conn);
      Resource actual = await svc.GetAsync(101);

      //Order does matter here, so we can compare the arrays
      actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Get_NotFound()
    {
      //Create new ESRegAggConnection...
      InMemoryConnection conn = new ESResQSvcGetConn("Resource_NotFound", 404);

      ESResourceQueryService svc = this.GetService<ESResourceQueryService>(conn);

      await Assert.ThrowsAnyAsync<Exception>(async () =>
      {
        Resource actual = await svc.GetAsync(1010);
      });
    }

    #endregion

    #region Test Query Building

    [Fact]
    public async Task QueryResources_EmptyQuery()
    {
      //Create new ESRegAggConnection...

      string actualPath = "";
      string expectedPath = "/r4r_v1/_search"; //Use index in config

      JsonNode actualRequest = null;
      JsonNode expectedRequest = JsonNode.Parse(@"
                {
                  ""from"": 0,
                  ""size"": 20,
                  ""_source"": {
                    ""includes"": [
                      ""id"",
                      ""title"",
                      ""website"",
                      ""body"",
                      ""description"",
                      ""toolTypes"",
                      ""researchAreas"",
                      ""researchTypes"",
                      ""resourceAccess"",
                      ""docs"",
                      ""pocs""
                    ]
                  },
                  ""sort"": [
                    {
                      ""title._sort"": {
                        ""order"": ""asc""
                      }
                    }
                  ]
                }"
      );

      var callback = new Action<ApiCallDetails>(details =>
      {
        actualPath = details.Uri.AbsolutePath;
        actualRequest = JsonNode.Parse(Encoding.UTF8.GetString(details.RequestBodyInBytes));
      });

      var svc = this.GetService<ESResourceQueryService>(callback);
      try
      {
        var results = await svc.QueryResourcesAsync(
            query: new ResourceQuery
            {
              Keyword = null,
              Filters = new Dictionary<string, string[]> { }
            },
            includeFields: new string[]
            {
                        "id",
                        "title",
                        "website",
                        "body",
                        "description",
                        "toolTypes",
                        "researchAreas",
                        "researchTypes",
                        "resourceAccess",
                        "docs",
                        "pocs"
            }
        );
      }
      catch (Exception) { } //We don't care how it processes the results...


      Assert.Equal(expectedPath, actualPath);
      Assert.True(JsonNode.DeepEquals(expectedRequest, actualRequest));
    }

    [Fact]
    public async Task QueryResources_From()
    {
      //Create new ESRegAggConnection...

      string actualPath = "";
      string expectedPath = "/r4r_v1/_search"; //Use index in config

      JsonNode actualRequest = null;
      JsonNode expectedRequest = JsonNode.Parse(@"
                {
                  ""from"": 20,
                  ""size"": 20,
                  ""_source"": {
                    ""includes"": [
                      ""id"",
                      ""title"",
                      ""website"",
                      ""body"",
                      ""description"",
                      ""toolTypes"",
                      ""researchAreas"",
                      ""researchTypes"",
                      ""resourceAccess"",
                      ""docs"",
                      ""pocs""
                    ]
                  },
                  ""sort"": [
                    {
                      ""title._sort"": {
                        ""order"": ""asc""
                      }
                    }
                  ]
                }"
      );

      var callback = new Action<ApiCallDetails>(details =>
      {
        actualPath = details.Uri.AbsolutePath;
        actualRequest = JsonNode.Parse(Encoding.UTF8.GetString(details.RequestBodyInBytes));
      });

      var svc = this.GetService<ESResourceQueryService>(callback);
      try
      {
        var results = await svc.QueryResourcesAsync(
            query: new ResourceQuery
            {
              Keyword = null,
              Filters = new Dictionary<string, string[]> { }
            },
            from: 20,
            includeFields: new string[]
            {
                        "id",
                        "title",
                        "website",
                        "body",
                        "description",
                        "toolTypes",
                        "researchAreas",
                        "researchTypes",
                        "resourceAccess",
                        "docs",
                        "pocs"
            }
        );
      }
      catch (Exception) { } //We don't care how it processes the results...


      Assert.Equal(expectedPath, actualPath);
      Assert.True(JsonNode.DeepEquals(expectedRequest, actualRequest));
    }

    [Fact]
    public async Task QueryResources_Size()
    {
      //Create new ESRegAggConnection...

      string actualPath = "";
      string expectedPath = "/r4r_v1/_search"; //Use index in config

      JsonNode actualRequest = null;
      JsonNode expectedRequest = JsonNode.Parse(@"
                {
                  ""from"": 0,
                  ""size"": 50,
                  ""_source"": {
                    ""includes"": [
                      ""id"",
                      ""title"",
                      ""website"",
                      ""body"",
                      ""description"",
                      ""toolTypes"",
                      ""researchAreas"",
                      ""researchTypes"",
                      ""resourceAccess"",
                      ""docs"",
                      ""pocs""
                    ]
                  },
                  ""sort"": [
                    {
                      ""title._sort"": {
                        ""order"": ""asc""
                      }
                    }
                  ]
                }"
      );

      var callback = new Action<ApiCallDetails>(details =>
      {
        actualPath = details.Uri.AbsolutePath;
        actualRequest = JsonNode.Parse(Encoding.UTF8.GetString(details.RequestBodyInBytes));
      });

      var svc = this.GetService<ESResourceQueryService>(callback);
      try
      {
        var results = await svc.QueryResourcesAsync(
            query: new ResourceQuery
            {
              Keyword = null,
              Filters = new Dictionary<string, string[]> { }
            },
            size: 50,
            includeFields: new string[]
            {
                        "id",
                        "title",
                        "website",
                        "body",
                        "description",
                        "toolTypes",
                        "researchAreas",
                        "researchTypes",
                        "resourceAccess",
                        "docs",
                        "pocs"
            }
        );
      }
      catch (Exception) { } //We don't care how it processes the results...


      Assert.Equal(expectedPath, actualPath);
      Assert.True(JsonNode.DeepEquals(expectedRequest, actualRequest));
    }

    [Fact]
    public async Task QueryResources_SingleInclude()
    {
      //Create new ESRegAggConnection...

      string actualPath = "";
      string expectedPath = "/r4r_v1/_search"; //Use index in config

      JsonNode actualRequest = null;
      JsonNode expectedRequest = JsonNode.Parse(@"
                {
                  ""from"": 20,
                  ""size"": 20,
                  ""_source"": {
                    ""includes"": [
                      ""id""
                    ]
                  },
                  ""sort"": [
                    {
                      ""title._sort"": {
                        ""order"": ""asc""
                      }
                    }
                  ]
                }"
      );

      var callback = new Action<ApiCallDetails>(details =>
      {
        actualPath = details.Uri.AbsolutePath;
        actualRequest = JsonNode.Parse(Encoding.UTF8.GetString(details.RequestBodyInBytes));
      });

      var svc = this.GetService<ESResourceQueryService>(callback);
      try
      {
        var results = await svc.QueryResourcesAsync(
            query: new ResourceQuery
            {
              Keyword = null,
              Filters = new Dictionary<string, string[]> { }
            },
            from: 20,
            includeFields: new string[]
            {
                        "id"
            }
        );
      }
      catch (Exception) { } //We don't care how it processes the results...


      Assert.Equal(expectedPath, actualPath);
      Assert.True(JsonNode.DeepEquals(expectedRequest, actualRequest));
    }

    [Fact]
    public async Task QueryResources_SingleFilter()
    {
      //Create new ESRegAggConnection...

      string actualPath = "";
      string expectedPath = "/r4r_v1/_search"; //Use index in config

      JsonNode actualRequest = null;
      JsonNode expectedRequest = JsonNode.Parse(@"
                {
                  ""from"": 0,
                  ""size"": 20,
                  ""_source"": {
                    ""includes"": [
                      ""id"",
                      ""title"",
                      ""website"",
                      ""body"",
                      ""description"",
                      ""toolTypes"",
                      ""researchAreas"",
                      ""researchTypes"",
                      ""resourceAccess"",
                      ""docs"",
                      ""pocs""
                    ]
                  },
                  ""sort"": [
                    {
                      ""title._sort"": {
                        ""order"": ""asc""
                      }
                    }
                  ],
                  ""query"": {
                    ""bool"": {
                        ""filter"": [
                            {""term"": { ""researchTypes.key"": { ""value"": ""basic"" }}}
                        ]
                    }
                  }
                }
            ");

      var callback = new Action<ApiCallDetails>(details =>
      {
        actualPath = details.Uri.AbsolutePath;
        actualRequest = JsonNode.Parse(Encoding.UTF8.GetString(details.RequestBodyInBytes));
      });

      var svc = this.GetService<ESResourceQueryService>(callback);
      try
      {
        var results = await svc.QueryResourcesAsync(
            query: new ResourceQuery
            {
              Filters = new Dictionary<string, string[]>{
                            { "researchTypes", new string[] { "basic"} }
                 },
            },
            includeFields: new string[]
            {
                        "id",
                        "title",
                        "website",
                        "body",
                        "description",
                        "toolTypes",
                        "researchAreas",
                        "researchTypes",
                        "resourceAccess",
                        "docs",
                        "pocs"
            }
        );
      }
      catch (Exception) { } //We don't care how it processes the results...


      Assert.Equal(expectedPath, actualPath);
      Assert.True(JsonNode.DeepEquals(expectedRequest, actualRequest));
    }

    [Fact]
    public async Task QueryResources_SingleFilter_Keyword()
    {
      //Create new ESRegAggConnection...

      string actualPath = "";
      string expectedPath = "/r4r_v1/_search"; //Use index in config

      JsonNode actualRequest = null;
      JsonNode expectedRequest = JsonNode.Parse(@"
                {
                  ""from"": 0,
                  ""size"": 20,
                  ""_source"": {
                    ""includes"": [
                      ""id"",
                      ""title"",
                      ""website"",
                      ""body"",
                      ""description"",
                      ""toolTypes"",
                      ""researchAreas"",
                      ""researchTypes"",
                      ""resourceAccess"",
                      ""docs"",
                      ""pocs""
                    ]
                  },
                  ""sort"": [
                    {
                      ""title._sort"": {
                        ""order"": ""asc""
                      }
                    }
                  ],
                  ""query"": {
                    ""bool"": {
                      ""filter"": [
                        {""term"": { ""researchTypes.key"": { ""value"": ""basic"" }}}
                      ],
                      ""must"": [
                        {
                          ""bool"": {
                            ""should"": [
                              { ""common"": { ""title._fulltext"": { ""query"": ""CGCI"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                              { ""match"": { ""title._fulltext"": { ""query"": ""CGCI"", ""boost"": 1.0 } } },
                              { ""match_phrase"": { ""title._fulltext"": { ""query"": ""CGCI"", ""boost"": 1.0 } } },
                              { ""common"": { ""body._fulltext"": { ""query"": ""CGCI"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                              { ""match"": { ""body._fulltext"": { ""query"": ""CGCI"", ""boost"": 1.0 } } },
                              { ""match_phrase"": { ""body._fulltext"": { ""query"": ""CGCI"", ""boost"": 1.0 } } },
                              { ""match"": { ""pocs.lastname._fulltext"": { ""query"": ""CGCI"", ""boost"": 1.0 } } },
                              { ""match"": { ""pocs.firstname._fulltext"": { ""query"": ""CGCI"", ""boost"": 1.0 } } },
                              { ""match"": { ""pocs.middlename._fulltext"": { ""query"": ""CGCI"", ""boost"": 1.0 } } }
                            ]
                          }
                        }
                      ]
                    }
                  }
                }
            ");
      /*
      */

      var callback = new Action<ApiCallDetails>(details =>
      {
        actualPath = details.Uri.AbsolutePath;
        actualRequest = JsonNode.Parse(Encoding.UTF8.GetString(details.RequestBodyInBytes));
      });

      var svc = this.GetService<ESResourceQueryService>(callback);
      try
      {
        var results = await svc.QueryResourcesAsync(
            query: new ResourceQuery
            {
              Keyword = "CGCI",
              Filters = new Dictionary<string, string[]>{
                            { "researchTypes", new string[] { "basic"} }
                }
            },
            includeFields: new string[]
            {
                        "id",
                        "title",
                        "website",
                        "body",
                        "description",
                        "toolTypes",
                        "researchAreas",
                        "researchTypes",
                        "resourceAccess",
                        "docs",
                        "pocs"
            }
        );
      }
      catch (Exception) { } //We don't care how it processes the results...


      Assert.Equal(expectedPath, actualPath);
      Assert.True(JsonNode.DeepEquals(expectedRequest, actualRequest));
    }

    #endregion

  }
}
