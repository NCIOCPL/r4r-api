Feature: Text searches based on actual production usage.

# Text searches started out as the top 50. This list is created by removing examples
# which differ only in case (e.g. Cell vs. cell).

    Background:
        * url apiHost


    Scenario Outline: Text search for resources

        Given path 'resources'
        And param q = searchText
        When method get
        Then status 200
        And match response == read('standard-text-search.expected/' + expectation)

        Examples:
            | searchText                                | expectation                                   |
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

