Feature: Stupid simple tests.

    Background:
        * url apiHost

    Scenario: All the resources are loaded

        Given path 'resources'
        And param size = 300
        When method get
        Then status 200
        And response.meta.totalResults == 258
        And match response == read('simple-big-blob.json')


