Feature: Users can use facets to refine their search results.

    Background:
        * url apiHost

    Scenario: Invalid facets are handled.

        Given path 'resources'
        And param includeFacets = 'chicken'
        When method get
        Then status 400
        And match response == {"Message":"Included facets in query are not valid."}


    Scenario: Retrieve list of only the facets

        Given path 'resources'
        And param size = 0
        When method get
        Then status 200
        And match response == read('facets-all-facets.json')


    Scenario Outline: Restrict facet list

        *   def expected = read('facets-' + expectationName + '.json')
        *   def queryParams = {size: 0}
        *   queryParams.includeFacets = eval(names)

        Given path 'resources'
        And params queryParams
        When method get
        Then status 200
        And match response == expected

        Examples:
            | names                          | expectationName |
            | ['researchTypes']              | single-facet    |
            | ['researchAreas', 'toolTypes'] | multiple-facets |


    Scenario Outline: Filter on a single top-level facet

        *   def expected = read('facets-filter-single-facet-' + expectationName + '.json')
        *   def queryParams = {size: 999}
        *   queryParams[facet] = eval(value)

        Given path 'resources'
        And params queryParams
        When method get
        Then status 200
        And match response == expected

        Examples:
            | facet         | value                                   | expectationName |
            | toolTypes     | ['lab_tools']                           | one-value       |
            | researchAreas | ['cancer_biology', 'cancer_prevention'] | multiple-values |


    Scenario: Filter on multiple top-level facets

        *   def expected = read('facets-filter-multiple-facets.json')
        *   def queryParams = {size: 999}
        *   queryParams.toolTypes = ['datasets_databases']
        *   queryParams.researchAreas = ['cancer_biology', 'cancer_treatment']

        Given path 'resources'
        And params queryParams
        When method get
        Then status 200
        And match response == expected

    Scenario: Multiple toolTypes values are passed.
        *   def expected = { Message: "Cannot have multiple tooltypes." }
        *   def queryParams = {size: 999, toolTypes: ['analysis_tools', 'datasets_databases', 'lab_tools']}

        Given path 'resources'
        And params queryParams
        When method get
        Then status 400
        And match response == expected


    Scenario Outline: toolSubtypes is passed without a parent toolTypes value(s)

        *   def expected = { Message: "Cannot have subtype without tooltype." }
        *   def queryParams = {size: 999}
        *   queryParams[subFacet] = eval(subFacetValues)

        Given path 'resources'
        And params queryParams
        When method get
        Then status 400
        And match response == expected

        Examples:
            | subFacet     | subFacetValues                                             |
            | toolSubtypes | ['genomic_datasets']                                       |
            | toolSubtypes | ['data_visualization', 'statistical_software', 'modeling'] |


    Scenario: toolSubtypes is passed with toolTypes (parent value) set to null.

        *   def expected = { Message: "Cannot have subtype without tooltype." }
        *   def queryParams = {size: 999}
        *   queryParams['tooltype'] = null
        *   queryParams['toolSubtypes'] = ['genomic_datasets']

        Given path 'resources'
        And params queryParams
        When method get
        Then status 400
        And match response == expected


    Scenario Outline: Filter with subfacet

        *   def expected = read('facets-subfacet-' + expectationName + '.json')
        *   def queryParams = {size: 999}
        *   queryParams[facet] = eval(facetValue)
        *   queryParams[subFacet] = eval(subFacetValues)

        Given path 'resources'
        And params queryParams
        When method get
        Then status 200
        And match response == expected

        Examples:
            | facet     | facetValue             | subFacet     | subFacetValues                                             | expectationName |
            | toolTypes | ['datasets_databases'] | toolSubtypes | ['genomic_datasets']                                       | single          |
            | toolTypes | ['analysis_tools']     | toolSubtypes | ['data_visualization', 'statistical_software', 'modeling'] | multiple        |


    Scenario Outline: Filter on text search

        *   def expected = read('facets-text-search-' + expectationName + '.json')
        *   def queryParams = {size: 999}
        *   queryParams.q = text
        *   queryParams[facet] = eval(facetValue)
        *   queryParams[subFacet] = eval(subFacetValues)

        Given path 'resources'
        And params queryParams
        When method get
        Then status 200
        And match response == expected

        Examples:
            | text          | facet     | facetValue             | subFacet     | subFacetValues                                             | expectationName |
            | lung          | toolTypes | ['datasets_databases'] | toolSubtypes | ['clinical_data']                                          | single          |
            | breast cancer | toolTypes | ['analysis_tools']     | toolSubtypes | ['data_visualization', 'modeling', 'statistical_software'] | multiple        |

