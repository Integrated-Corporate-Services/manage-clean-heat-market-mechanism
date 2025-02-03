@APITest
Feature: Verify CHMM Post to submit Quarterly boiler sales for an organisation API works as expected
    CHMM Post to submit quarterly boiler sales for an organisation API test


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


    Scenario Outline: 1. Verify that POST to submit quarterly boiler sales for an organisation by admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit boiler sales figures with date "2024-04-01", authentication token and following data:
            | key                 | value                 |
            | organisationId      | <organisationId>      |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |

        Then the response status code should be 201
        And the response body should contain uuid
        And the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarters" to fetch boiler sales figures with authentication token
        And the response status code should be 200
        And the response body array for quarterly boiler sales for "<schemeYearQuarterId>" quarter should contain the following data:
            | key            | value            |
            | organisationId | <organisationId> |
            | gas            | <gas>            |
            | oil            | <oil>            |
            | status         | Submitted        |

        Examples:
            | authEmailId                       | organisationId | schemeYearId                         | schemeYearQuarterId                  | oil     | gas     |
            | triad.test.acc.2+admin1@gmail.com | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 101     | 102     |
            | triad.test.acc.2+admin1@gmail.com | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 0       | 0       |
            | triad.test.acc.2+admin1@gmail.com | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 1010000 | 1020000 |


    Scenario Outline: 2. Verify that POST to submit quarterly boiler sales for an organisation by manufacturer user returns 200

        Given the user has authentication token for "newRandom" manufacturer user email id

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit boiler sales figures with date "2024-04-01", authentication token and following data:
            | key                 | value                 |
            | organisationId      | <organisationId>      |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |

        Then the response status code should be 201
        And the response body should contain uuid
        And the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarters" to fetch boiler sales figures with authentication token
        And the response status code should be 200
        And the response body array for quarterly boiler sales for "<schemeYearQuarterId>" quarter should contain the following data:
            | key            | value            |
            | organisationId | <organisationId> |
            | gas            | <gas>            |
            | oil            | <oil>            |
            | status         | Submitted        |

        Examples:
            | adminEmailId                      | organisationId | schemeYearId                         | schemeYearQuarterId                  | oil     | gas     |
            | triad.test.acc.2+admin1@gmail.com | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 101     | 102     |
            | triad.test.acc.2+admin1@gmail.com | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 0       | 0       |
            | triad.test.acc.2+admin1@gmail.com | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 1010000 | 1020000 |


    Scenario Outline: 3. Verify that POST to submit quarterly boiler sales with invalid organisationId/schemeYearQuarterId returns 404

        Given the user has authentication token for "<authEmailId>" manufacturer user email id

        When the user send a POST request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit boiler sales figures with date "2024-04-01", authentication token and following data:
            | key                 | value                 |
            | organisationId      | newRandom             |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |

        Then the response status code should be 404

        Examples:
            | authEmailId                       | schemeYearId                         | schemeYearQuarterId                  | oil | gas | organisationId                       |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 101 | 102 | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d  | eb186587-f5d7-422e-b90b-c46b9196c957 | 101 | 102 | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c95  | 101 | 102 | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 |
            # manufacturer user
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 101 | 102 | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  |
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d  | eb186587-f5d7-422e-b90b-c46b9196c957 | 101 | 102 | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 |
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c95  | 101 | 102 | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 |


    Scenario Outline: 4. Verify that POST to submit quarterly boiler sales figures by an admin/manufacturer user with decimal numbers returns 400

        Given the user has authentication token for "<authEmailId>" manufacturer user email id

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit boiler sales figures with date "2024-04-01", authentication token and following data:
            | key                 | value                 |
            | organisationId      | newRandom             |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |

        Then the response status code should be 400

        Examples:
            | authEmailId                       | schemeYearId                         | schemeYearQuarterId                  | oil   | gas   |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 10.11 | 10.12 |
            # manufacturer user
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 10.11 | 10.12 |


    Scenario Outline: 5. Verify that POST to submit quarterly boiler sales figures without authentication token return 401

        When the user send a POST request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit "quarterly" boiler sales figures without authentication token and following data:
            | key                 | value                 |
            | organisationId      | <organisationId>      |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |

        Then the response status code should be 401

        Examples:
            | authEmailId                       | schemeYearId                         | schemeYearQuarterId                  | oil   | gas   | organisationId                       |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 10.11 | 10.12 | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 |
            # manufacturer user
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 10.11 | 10.12 | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 |
