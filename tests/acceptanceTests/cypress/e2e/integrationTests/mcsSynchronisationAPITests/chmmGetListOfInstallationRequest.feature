@APITest
Feature: Verify CHMM GET a list of installation request summaries details API works as expected
    CHMM GET a list of installtion request summaries detailing all the requests that have been made to retrieve MCS data API test


    Scenario: 1. Verify that GET list of installation requests by an admin user returns 200

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a GET request to "/mcssynchronisation/data/requests" with authentication token

        Then the response status code should be 200
        And the response body array should not be empty



    Scenario Outline: 2. Verify that GET list of installation requests by a manufacuter user/inactive admin user returns 403

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/mcssynchronisation/data/requests" with authentication token

        Then the response status code should be 403

        Examples:
            | authEmailId                        |
            | triad.test.acc.4+mfr1@gmail.com    |
            | triad.test.acc.2+admin10@gmail.com |


    Scenario: 3. Verify that GET list of installation requests without authentication returns 401

        When the user send a GET request to "/mcssynchronisation/data/requests" without authentication token

        Then the response status code should be 401
