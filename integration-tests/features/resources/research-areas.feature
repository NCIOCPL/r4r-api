Feature: Users can search for available resources by research area.

    Background:
        * url apiHost


    Scenario Outline: Search by various resource areas.

        Given path 'resources'
        And params {researchAreas: <area>}
        When method get
        Then status 200
        And match response == read( expectation )

        Examples:
            | area                                                                                      | expectation                  |
            | [cancer_biology]                                                                          | research-areas-single.json   |
            | [cancer_biology, cancer_prevention]                                                       | research-areas-multiple.json |
            | [cancer_treatment, causes_of_cancer, cancer_diagnosis, cancer_statistics, bioinformatics] | research-areas-many.json     |

    Scenario Outline: Resource areas with resource types

        Given path 'resources'
        And params {researchAreas: <area>, researchTypes: <type>}
        When method get
        Then status 200
        And match response == read( expectation )

        Examples:
            | area                                      | type                              | expectation                                   |
            | [cancer_biology]                          | [translational]                   | research-areas-single_area-single_type.json   |
            | [cancer_biology, bioinformatics]          | [translational]                   | research-areas-multiple_area-single_type.json |
            | [cancer_diagnosis, cancer_survivorship]   | [basic, epidemiologic, clinical]  | research-areas-multiple_area-multiple_type.json |

