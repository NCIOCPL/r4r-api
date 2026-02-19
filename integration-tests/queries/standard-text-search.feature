Feature: Full text searches

# Text searches started out as the top 50. This list is created by removing examples
# which differ only in case (e.g. Cell vs. cell).

    Background:
        * url esHost


    Scenario Outline: Text search for resources

        * def query =
            """
            {
                "query": {
                    "bool": {
                        "should": [
                            { "common": { "body._fulltext": { "boost": 1, "cutoff_frequency": 1, "query": "#(searchText)" } } },
                            { "match": { "body._fulltext": { "boost": 1, "query": "#(searchText)" } } },
                            { "match_phrase": { "body._fulltext": { "boost": 1, "query": "#(searchText)" } } },
                            { "match": { "pocs.firstname._fulltext": { "boost": 1, "query": "#(searchText)" } } },
                            { "match": { "pocs.lastname._fulltext": { "boost": 1, "query": "#(searchText)" } } },
                            { "match": { "pocs.middlename._fulltext": { "boost": 1, "query": "#(searchText)" } } },
                            { "common": { "title._fulltext": { "boost": 1, "cutoff_frequency": 1, "query": "#(searchText)" } } },
                            { "match": { "title._fulltext": { "boost": 1, "query": "#(searchText)" } } },
                            { "match_phrase": { "title._fulltext": { "boost": 1, "query": "#(searchText)" } } }
                        ]
                    }
                },
                "from": 0,
                "size": 20,
                "sort": { "title._sort": { "order": "asc" } },
                "_source": {
                    "includes": [ "id", "title", "website", "body", "description", "toolTypes", "researchAreas", "researchTypes", "resourceAccess", "docs", "pocs" ]
                }
            }
            """

        Given path 'r4r_v1', '_search'
        And header Content-Type = 'application/json'
        And request query
        When method POST
        Then status 200
        * def expected = read('standard-text-search.expected/' + expectation)
        And match response.hits.hits == expected.hits.hits
        #* karate.write(karate.pretty(response), 'standard-text-search.expected/' + expectation)

        Examples:
            | searchText                                | expectation                                   |
            | genome                                    | genome.json                                   |
            | med-rt                                    | med-rt.json                                   |
            | breast cancer                             | breast-cancer.json                            |
            | cancer                                    | cancer.json                                   |
            | tumor                                     | tumor.json                                    |
            | melanoma                                  | melanoma.json                                 |
            | lung                                      | lung.json                                     |
            | cell line                                 | cell-line.json                                |
            | biospecimen                               | biospecimen.json                              |
            | tcga                                      | tcga.json                                     |
            | SNOMED                                    | snomed.json                                   |
            | tsval                                     | tsval.json                                    |
            | Breast                                    | breast.json                                   |
            | Anti-epileptic Agent                      | anti-epileptic-agent.json                     |
            | ovarian cancer                            | ovarian-cancer.json                           |
            | Captopril 50 mg TAB                       | captopril-50-mg-tab.json                      |
            | cell lines                                | cell-lines.json                               |
            | lung cancer                               | lung-cancer.json                              |
            | PD-L1                                     | pd-l1.json                                    |
            | skin cancer                               | skin-cancer.json                              |
            | evs                                       | evs.json                                      |
            | National Drug File Reference Terminology  | national-drug-file-reference-terminology.json |
            | liver cancer                              | liver-cancer.json                             |
            | NCI EVS Terminology Resources             | nci-evs-terminology-resources.json            |
            | gene expression                           | gene-expression.json                          |
            | informed consent                          | informed-consent.json                         |
            | glioblastoma                              | glioblastoma.json                             |
            | NCI EVS                                   | nci-evs.json                                  |
            | ndf-rt                                    | ndf-rt.json                                   |
            | pclas                                     | pclas.json                                    |
            | trial summary                             | trial-summary.json                            |
            | terminology                               | terminology.json                              |
            | vaccine                                   | vaccine.json                                  |
            | ctep                                      | ctep.json                                     |
            | TP53                                      | tp53.json                                     |
            | seer                                      | seer.json                                     |
            | mRNA                                      | mrna.json                                     |
            | ovarian                                   | ovarian.json                                  |
            | tissue                                    | tissue.json                                   |
            | cervical cancer                           | cervical-cancer.json                          |
            | aml                                       | aml.json                                      |
            | database                                  | database.json                                 |
            | drug                                      | drug.json                                     |

