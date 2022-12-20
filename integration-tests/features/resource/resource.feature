Feature: Users may retrieve details about an individual resource.

    Background:
        * url apiHost


    Scenario: Resource ID not specified.

        Given path 'resource'
        When method get
        Then status 404
        # No actual response body to check.


    Scenario Outline: Resource ID does not exist

        Given path 'resource', id
        When method get
        Then status 404
        And match response == {"Message":"Resource not found."}

        Examples:
            | id      |
            | -5      |
            | 0       |
            | 9999999 |


    Scenario Outline: Resource ID is not valid

        Given path 'resource', id
        When method get
        Then status 400
        And match response == {"Message":"The resource identifier is invalid."}

        Examples:
            | id      |
            | chicken |
            | 1,2,3   |
            | 12.3456 |
            | 12+34   |
            | 12%2034 |


    Scenario Outline: Valid ID

        Given path 'resource', id
        When method get
        Then status 200
        And match response == read( expected )

        Examples:
            | id  | expected               |
            | 1   | resource-valid-1.json  |
            | 100 | resource-valid-100.json  |
            | 200 | resource-valid-200.json  |
