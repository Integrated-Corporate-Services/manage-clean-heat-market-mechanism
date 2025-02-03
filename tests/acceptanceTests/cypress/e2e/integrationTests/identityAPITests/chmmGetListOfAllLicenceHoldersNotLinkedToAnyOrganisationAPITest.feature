@APITest
Feature: Verify CHMM Get list of all licence holders that are not linked to any organisation API works as expected
    CHMM Get list of all licence holders that are not linked to any organisation API test


    Scenario: 1. Verify that GET list of all licence holders that are not linked to any organisation by an admin user retuns 200

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a GET request to "/identity/licenceholders/unlinked" with authentication token

        Then the response status code should be 200
        And the response body array should not be empty
        

    Scenario: 2. Verify that GET list of all licence holders that are not linked to any organisation by a manufacturer user returns 403

        Given the user has authentication token for "triad.test.acc.4+mfr1@gmail.com" email id

        When the user send a GET request to "/identity/licenceholders/unlinked" with authentication token

        Then the response status code should be 403


    Scenario: 3. Verify that GET list of all licence holders that are not linked to any organisation without authentication token returns 401

        When the user send a GET request to "/identity/licenceholders/unlinked" without authentication token

        Then the response status code should be 401