@APITest
Feature: Verify CHMM GET obligation summary API works as expected
    CHMM GET obligation summary API test


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


    Scenario Outline: 1. Verify that GET obligation summary for annual for an organisation by admin/manufacturer user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to submit boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value            |
            | organisationId | <organisationId> |
            | schemeYearId   | <schemeYearId>   |
            | oil            | <oil>            |
            | gas            | <gas>            |

        When the user send a GET request to "/obligation/organisation/organisationId/year/<schemeYearId>/summary" with authentication token

        Then the response status code should be 200
        And the response json body should contain the following data:
            | key                       | value         |
            | generatedObligations      | <obligations> |
            | obligationsBroughtForward | 0             |
            | finalObligations          | <obligations> |
            | obligationsCarriedOver    | 0             |
            | obligationsPaidOff        | 0             |
            | remainingObligations      | <obligations> |

        Examples:
            | authEmailId                       | organisationId | schemeYearId                         | gas   | oil  | obligations |
            | triad.test.acc.2+admin1@gmail.com | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 20050 | 1050 | 4           |
            | newRandom                         | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 20050 | 1050 | 4           |


    Scenario Outline: 2. Verify that GET obligation summary for quarterly for an organisation by admin/manufacturer user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit boiler sales figures with date "2024-04-01", authentication token and following data:
            | key                 | value                 |
            | organisationId      | <organisationId>      |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |

        When the user send a GET request to "/obligation/organisation/organisationId/year/<schemeYearId>/summary" with authentication token

        Then the response status code should be 200
        And the response json body should contain the following data:
            | key                       | value         |
            | generatedObligations      | <obligations> |
            | obligationsBroughtForward | 0             |
            | finalObligations          | <obligations> |
            | obligationsCarriedOver    | 0             |
            | obligationsPaidOff        | 0             |
            | remainingObligations      | <obligations> |

        Examples:
            | authEmailId                       | organisationId | schemeYearId                         | schemeYearQuarterId                  | gas   | oil  | obligations |
            | triad.test.acc.2+admin1@gmail.com | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 1050 | 4           |
            | newRandom                         | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 1050 | 4           |


    Scenario: 3. Verify that GET obligation summary by a manufacturer user for a different organisation returns 403

        Given the user has authentication token for "triad.test.acc.4+mfr1@gmail.com" email id

        When the user send a GET request to "/obligation/organisation/c9ea5101-e45b-4f16-ac8e-e48c6d83ec81/year/d525e380-4aee-40e9-a7f0-1784d8cb49d9/summary" with authentication token

        Then the response status code should be 403
        

    Scenario: 4. Verify that GET obligation summary without authentication token returns 401

        When the user send a GET request to "/obligation/organisation/c9ea5101-e45b-4f16-ac8e-e48c6d83ec81/year/d525e380-4aee-40e9-a7f0-1784d8cb49d9/summary" without authentication token

        Then the response status code should be 401
        And the response json body should contain the following data:
            | key    | value        |
            | status | 401          |
            | title  | Unauthorized |
