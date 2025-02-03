@APITest
Feature: Verify CHMM POST to submit quarterly boiler obligations for an organisation API works as expected
    CHMM POST to submit quarterly boiler obligations for an organisation API test

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


    Scenario Outline: 1. Verify that POST to submit quarterly boiler obligations is automatically triggered when quarterly boiler sales figures are submitted and approved by an admin user

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit boiler sales figures with date "2024-04-01", authentication token and following data:
            | key                 | value                 |
            | organisationId      | <organisationId>      |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |

        When the user send a GET request to "/obligation/organisation/organisationId/year/<schemeYearId>/summary" with authentication token

        Then the response json body should contain the following data:
            | key                       | value         |
            | generatedObligations      | <obligations> |
            | obligationsBroughtForward | 0             |
            | finalObligations          | <obligations> |
            | obligationsCarriedOver    | 0             |
            | obligationsPaidOff        | 0             |
            | remainingObligations      | <obligations> |

        Examples:
            | organisationId | schemeYearId                         | schemeYearQuarterId                  | gas   | oil  | obligations |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 1050 | 4           |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 19998 | 998  | 0           |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 19999 | 999  | 0           |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20000 | 1000 | 0           |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 0    | 2           |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 0     | 1050 | 2           |


    Scenario Outline: 2. Verify that POST to submit quarterly boiler obligations is automatically triggered when quarterly boiler sales figures are approved by an admin user that are submitted by a manufacturer user

        Given the user has authentication token for "newRandom" email id
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit boiler sales figures with date "2024-04-01", authentication token and following data:
            | key                 | value                 |
            | organisationId      | <organisationId>      |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |

        When the user send a GET request to "/obligation/organisation/organisationId/year/<schemeYearId>/summary" with authentication token

        Then the response json body should contain the following data:
            | key                       | value         |
            | generatedObligations      | <obligations> |
            | obligationsBroughtForward | 0             |
            | finalObligations          | <obligations> |
            | obligationsCarriedOver    | 0             |
            | obligationsPaidOff        | 0             |
            | remainingObligations      | <obligations> |

        Examples:
            | organisationId | schemeYearId                         | schemeYearQuarterId                  | gas   | oil  | obligations |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 1050 | 4           |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 19998 | 998  | 0           |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 19999 | 999  | 0           |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20000 | 1000 | 0           |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 0    | 2           |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 0     | 1050 | 2           |


    Scenario Outline: 3. Verify that POST to submit quarterly boiler obligations for an organisation that has no quarterly bolier sales submitted yet returns 201

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/obligation/quarterly" to submit an obligation with authentication token and following data:
            | key                 | value                 |
            | organisationId      | <organisationId>      |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | gas                 | <gas>                 |
            | oil                 | <oil>                 |
            | transactionDate     | currentDate           |

        Then the response status code should be 201
        And the response body should contain uuid
        And the user send a GET request to "/obligation/organisation/organisationId/year/<schemeYearId>/summary" with authentication token
        And the response json body should contain the following data:
            | key                       | value |
            | generatedObligations      | 4     |
            | obligationsBroughtForward | 0     |
            | finalObligations          | 4     |
            | obligationsCarriedOver    | 0     |
            | obligationsPaidOff        | 0     |
            | remainingObligations      | 4     |

        Examples:
            | authEmailId                       | organisationId | schemeYearId                         | schemeYearQuarterId                  | gas   | oil  |
            | triad.test.acc.2+admin1@gmail.com | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 1050 |
            | newRandom                         | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 1050 |


    Scenario Outline: 4. Verify that POST to submit quarterly boiler obligations with invalid organisationId/schemeYearId returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/obligation/quarterly" to submit an obligation with authentication token and following data:
            | key                 | value                 |
            | organisationId      | <organisationId>      |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | gas                 | <gas>                 |
            | oil                 | <oil>                 |
            | transactionDate     | currentDate           |

        Then the response status code should be <statusCode>

        Examples:
            | authEmailId                       | organisationId                       | schemeYearId                         | schemeYearQuarterId                  | gas   | oil  | statusCode |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | 7b0dfd38-49d8-493c-bccd-407dbece08b5 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 1050 | 400        |
            # Manufacturer user
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 1050 | 403        |
            | newRandom                         | newRandom                            | 22c6d2f7-2b5c-40f0-9841-780452e867b7 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 1050 | 400        |
            | newRandom                         | newRandom                            | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d  | 20050 | 1050 | 400        |


    Scenario Outline: 5. Verify that POST to submit quarterly boiler obligations without authentication token returns 401

        When the user send a POST request to "/obligation/quarterly" to submit an obligation without authentication token and following data:
            | key                 | value                 |
            | organisationId      | <organisationId>      |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | gas                 | <gas>                 |
            | oil                 | <oil>                 |
            | transactionDate     | currentDate           |

        Then the response status code should be 401
        And the response json body should contain the following data:
            | key    | value        |
            | status | 401          |
            | title  | Unauthorized |

        Examples:
            | organisationId | schemeYearId                         | schemeYearQuarterId                  | gas   | oil  |
            | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 20050 | 1050 |
