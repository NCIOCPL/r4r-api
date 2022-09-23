Feature: Users may look up resources by doing a text search
    Background:
        * url apiHost


    Scenario Outline: Text search for resources

        Given path 'resources'
        And param q = searchText
        When method get
        Then status 200

        Examples:
            | searchText | expectation |
            | community | text-search-community.json |