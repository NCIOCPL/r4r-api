Feature: Users can retrieve a list of facets.

    Background:
        * url apiHost


    Scenario Outline: One ore more valid facets

        Given path 'resources'
        And params {includeFacets: <facetList>, size: 0}
        When method get
        Then status 200
        And match response == read( expectation )

        Examples:
            | facetList                  | expectation          |
            | [toolTypes]                | facets-single.json   |
            | [toolTypes, researchAreas] | facets-multiple.json |


    Scenario: Invalid facets are handled.

        Given path 'resources'
        And param includeFacets = 'chicken'
        When method get
        Then status 400
        And match response == {"Message":"Included facets in query are not valid."}
