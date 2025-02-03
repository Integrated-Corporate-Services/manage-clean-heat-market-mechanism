# @APITest
Feature: Verify CHMM GET to download installation request data API works as expected
    CHMM GET to download a CSV file of the data contained within the specified MCS synchronisation


    Scenario: 1. Verify that GET to download installation data by an admin user returns 200

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a GET request to "/mcssynchronisation/data/requests/875e2472-e2d3-4c02-ac30-955191b5babc/download" with authentication token

        Then the response status code should be 200


    @APITest
    Scenario Outline: 2. Verify that GET to download installation data by a manufacuter user/inactive admin user returns 403

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/mcssynchronisation/data/requests/875e2472-e2d3-4c02-ac30-955191b5babc/download" with authentication token

        Then the response status code should be 403

        Examples:
            | authEmailId                        |
            | triad.test.acc.4+mfr1@gmail.com    |
            | triad.test.acc.2+admin10@gmail.com |


    @APITest
    Scenario: 3. Verify that GET to download installation data without authentication returns 401

        When the user send a GET request to "/mcssynchronisation/data/requests/875e2472-e2d3-4c02-ac30-955191b5babc/download" without authentication token

        Then the response status code should be 401
        