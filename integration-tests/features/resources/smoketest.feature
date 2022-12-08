Feature: Extremely simple tests.

    Background:
        * url apiHost

    Scenario: All the resources are loaded

        # Same query as the r4r app's initial load
        Given path 'resources'
        And params {includeFacets: [toolTypes, researchAreas]}
        When method get
        Then def size = response.meta.totalResults

        Given path 'resources'
        And param size = size
        When method get
        Then status 200
        And response.meta.totalResults == 258
        And response.results.count == 258
        And match response == read('smoketest-big-blob.json')


