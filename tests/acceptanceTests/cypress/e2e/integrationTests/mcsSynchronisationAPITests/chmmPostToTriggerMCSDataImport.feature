@APITest
Feature: Verify CHMM POST to trigger the MCS data fetching and persisting API works as expected
    CHMM POST to trigger the MCS data fetching and persisting API test


    Scenario: 1. Verify that POST to trigger the MCS data with API key returns 200

        When the user send a POST request to "/mcssynchronisation/trigger-mcsdata-import" to trigger MCS data with authentication token and following data:
            """
            {
                "startDate": "2024-02-01",
                "endDate": "2024-02-28"
            }
            """

        Then the response status code should be 200

    
    # technologyTypeIds and isNewBuildIds
    Scenario Outline: 2. Verify that POST to trigger the MCS data with different technologyTypeId with API key retruns 200

        When the user send a POST request to "/mcssynchronisation/trigger-mcsdata-import" to trigger MCS data with authentication token and following data:
            """
            {
                "startDate": "2024-02-01",
                "endDate": "2024-02-28",
                "technologyTypeIds": [
                    <technologyTypeId>
                ],
                "isNewBuildIds": [
                    <isNewBuildId>
                ]
            }
            """

        Then the response status code should be 200

        Examples:
            | technologyTypeId | isNewBuildId |
            | 0                | 1            |
            | 0                | 2            |
            | 1                | 1            |
            | 1                | 2            |
            | 2                | 1            |
            | 2                | 2            |
            | 3                | 1            |
            | 3                | 2            |
            | 11111111         | 1            |
            | 11111111         | 2            |

    
    # Invalid without API key
    Scenario: 3. Verify that POST to trigger the MCS data without API key returns 401

        When the user send a POST request to "/mcssynchronisation/trigger-mcsdata-import" to trigger MCS data without authentication token and following data:
            """
            {
                "startDate": "2024-02-01",
                "endDate": "2024-02-28"
            }
            """

        Then the response status code should be 401


    # With authentication token
    Scenario Outline: 4. Verify that POST to trigger the MCS data with authentication token returns 401

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/mcssynchronisation/trigger-mcsdata-import" with authentication token and following data:
            """
            {
                "startDate": "2024-02-01",
                "endDate": "2024-02-28"
            }
            """

        Then the response status code should be 401

        Examples:
            | authEmailId                       |
            | triad.test.acc.2+admin1@gmail.com |
            | triad.test.acc.4+mfr1@gmail.com   |
