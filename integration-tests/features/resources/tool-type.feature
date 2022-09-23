Feature: Users may look up resources by tool type

    Background:
        * url apiHost


    Scenario Outline: Load various tool types

        Given path 'resources'
        And param toolTypes = toolType
        When method get
        Then status 200
        And match response == read( expectation )

        Examples:
            | toolType                 | expectation                   |
            | analysis_tools           | tool-type-analysis_tools.json           |
            | datasets_databases       | tool-type-datasets_databases.json       |
            | lab_tools                | tool-type-lab_tools.json                |
            | community_research_tools | tool-type-community_research_tools.json |
