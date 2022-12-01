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
            | toolType                 | expectation                             |
            | analysis_tools           | tool-type-analysis_tools.json           |
            | datasets_databases       | tool-type-datasets_databases.json       |
            | lab_tools                | tool-type-lab_tools.json                |
            | community_research_tools | tool-type-community_research_tools.json |


    Scenario Outline: Tool type with Sub-Types

        Given path 'resources'
        And params {toolTypes: 'analysis_tools', toolSubtypes: <subType> }
        When method get
        Then status 200
        And match response == read( expectation )

        Examples:
            | subType                          | expectation                      |
            | [data_visualization]             | tool-type-one_subType.json       |
            | [modeling, statistical_software] | tool-type-multiple_subTypes.json |
            | [chicken]                        | tool-type-invalid_subtype.json   |
