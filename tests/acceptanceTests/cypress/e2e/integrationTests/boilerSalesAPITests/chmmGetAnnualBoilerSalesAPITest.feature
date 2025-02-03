@APITest
Feature: Verify CHMM Get annual boiler sales for an organisation API works as expected
    CHMM Get annual boiler sales for an organisation API test


    Background: Create a manufacturer account

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        And the admin user creates an organisation with the following data:
            | key                        | value     |
            | isOnBehalfOfGroup          | false     |
            | organisationName           | newRandom |
            | companyNumber              | newRandom |
            | isFossilFuelBoilerSeller   | false     |
            | manufacturerUserEmailId    | newRandom |
            | manufacturerSROUserEmailId | newRandom |
            | isResponsibleOfficer       | true      |
            | creditTransferOptIn        | false     |
            | creditTransferEmailId      | null      |

    
    Scenario Outline: 1. Verify that GET annual boiler sales for an organisation by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to submit boiler sales figures with date "2025-01-01", authentication token and following data:
            | key                | value                |
            | organisationId     | newRandom            |
            | schemeYearId       | <schemeYearId>       |
            | oil                | <oil>                |
            | gas                | <gas>                |

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to fetch boiler sales figures with authentication token

        Then the response status code should be 200
        And the response json body for annual boiler sales should contain the following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | gas            | <gas>          |
            | oil            | <oil>          |
            | status         | Submitted      |

        Examples:
            | authEmailId                       | schemeYearId                         | oil | gas |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 101 | 102 |


    Scenario Outline: 2. Verify that GET annual boiler sales for an organisation by a manufacturer user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to submit boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | oil            | <oil>          |
            | gas            | <gas>          |
        And the user has authentication token for "newRandom" email id

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to fetch boiler sales figures with authentication token

        Then the response status code should be 200
        And the response json body for annual boiler sales should contain the following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | gas            | <gas>          |
            | oil            | <oil>          |
            | status         | Submitted      |

        Examples:
            | authEmailId                       | schemeYearId                         | oil | gas |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 101 | 102 |


    Scenario: 3. Verify that GET annual boiler sales for an organisation without authentication token returns 401

        When the user send a GET request to "/boilersales/organisation/6d718080-bddd-44ac-a7d0-32e652029b41/year/d525e380-4aee-40e9-a7f0-1784d8cb49d9/annual"

        Then the response status code should be 401


    Scenario Outline: 4. Verify that GET annual boiler sales for an organisation with invalid data returns 404

        When the user send a GET request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/annual"

        Then the response status code should be 404

        Examples:
            | organisationId                       | schemeYearId                         |
            # Invalid Organisation Id
            | 6d718080-bddd-44ac-a7d0              | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            # Invalid schemeYearId
            | 6d718080-bddd-44ac-a7d0-32e652029b41 | d525e380-4aee-40e9-a7f0              |
            # Empty Organisation Id and schemeYearId
            |                                      |                                      |


    Scenario Outline: 5. Verify that GET annual boiler sales for an organisation with an authentication token of a user who is not registered returns 403

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/annual" with authentication token

        Then the response status code should be 403

        Examples:
            | authEmailId                 | organisationId                       | schemeYearId                         |
            | triad.test.acc.61@gmail.com | 6d718080-bddd-44ac-a7d0-32e652029b41 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
