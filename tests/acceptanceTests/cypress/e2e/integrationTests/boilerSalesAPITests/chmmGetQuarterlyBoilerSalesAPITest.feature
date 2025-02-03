@APITest
Feature: Verify CHMM Get quarterly boiler sales for an organisation API works as expected
    CHMM Get quarterly boiler sales for an organisation API test


    ###################### Get annual boiler sales for an organisation #######################

    Scenario Outline: 1. Verify that GET quarterly boiler sales for an organisation by admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the admin user creates an organisation with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | <isOnBehalfOfGroup>          |
            | organisationName           | newRandom                    |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | <isFossilFuelBoilerSeller>   |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | isResponsibleOfficer       | <isResponsibleOfficer>       |
            | creditTransferOptIn        | <creditTransferOptIn>        |
            | creditTransferEmailId      | <creditTransferEmailId>      |
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit boiler sales figures with date "2024-04-01", authentication token and following data:
            | key                 | value                 |
            | organisationId      | newRandom             |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarters" to fetch boiler sales figures with authentication token

        Then the response status code should be 200
        And the response body array for quarterly boiler sales for "<schemeYearQuarterId>" quarter should contain the following data:
            | key            | value     |
            | organisationId | newRandom |
            | gas            | <gas>     |
            | oil            | <oil>     |
            | status         | Submitted |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | schemeYearQuarterId                  | oil | gas |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 101 | 102 |


    Scenario Outline: 2. Verify that GET quarterly boiler sales by manufacturer user for their own organisation returns 200

        Given the user has authentication token for "<adminEmailId>" email id
        And the admin user creates an organisation with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | <isOnBehalfOfGroup>          |
            | organisationName           | newRandom                    |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | <isFossilFuelBoilerSeller>   |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | isResponsibleOfficer       | <isResponsibleOfficer>       |
            | creditTransferOptIn        | <creditTransferOptIn>        |
            | creditTransferEmailId      | <creditTransferEmailId>      |
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit boiler sales figures with date "2024-04-01", authentication token and following data:
            | key                 | value                 |
            | organisationId      | newRandom             |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarters" to fetch boiler sales figures with authentication token

        Then the response status code should be 200
        And the response body array for quarterly boiler sales for "<schemeYearQuarterId>" quarter should contain the following data:
            | key            | value     |
            | organisationId | newRandom |
            | gas            | <gas>     |
            | oil            | <oil>     |
            | status         | Submitted |

        Examples:
            | adminEmailId                      | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | schemeYearQuarterId                  | oil | gas |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 101 | 102 |


    Scenario Outline: 3. Verify that GET quarterly boiler sales for an organisation by admin user with invalid data returns 404

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/quarters" with authentication token

        Then the response status code should be 404

        Examples:
            | authEmailId                       | organisationId                       | schemeYearId                         |
            | triad.test.acc.2+admin1@gmail.com | 48b91d8d-8bec-4cf3-b544              | eb186587-f5d7-422e-b90b-c46b9196c957 |
            | triad.test.acc.2+admin1@gmail.com | 48b91d8d-8bec-4cf3-b544-d9eaa7e82fc9 | eb186587-f5d7-422e-b90b              |
            | triad.test.acc.2+admin1@gmail.com |                                      |                                      |


    Scenario: 4. Verify that GET quarterly boiler sales for an organisation without authentication token returns 401

        When the user send a GET request to "/boilersales/organisation/6d718080-bddd-44ac-a7d0-32e652029b41/year/d525e380-4aee-40e9-a7f0-1784d8cb49d9/quarters"

        Then the response status code should be 401


    Scenario Outline: 5. Verify that GET quarterly boiler sales by a manufacturer user for a different organisation returns 400

        Given the user has authentication token for "<adminEmailId>" email id
        And the admin user creates an organisation with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | <isOnBehalfOfGroup>          |
            | organisationName           | newRandom                    |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | <isFossilFuelBoilerSeller>   |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | isResponsibleOfficer       | <isResponsibleOfficer>       |
            | creditTransferOptIn        | <creditTransferOptIn>        |
            | creditTransferEmailId      | <creditTransferEmailId>      |
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>" to submit boiler sales figures with date "2024-04-01", authentication token and following data:
            | key                 | value                 |
            | organisationId      | newRandom             |
            | schemeYearId        | <schemeYearId>        |
            | schemeYearQuarterId | <schemeYearQuarterId> |
            | oil                 | <oil>                 |
            | gas                 | <gas>                 |
        And the user has authentication token for "triad.test.acc.5+mfr1@gmail.com" manufacturer user email id

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarters" to fetch boiler sales figures with authentication token

        Then the response status code should be 403
        And the response json body should contain the following data:
            | key    | value                                          |
            | status | 403                                            |

        Examples:
            | adminEmailId                      | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | schemeYearQuarterId                  | oil | gas |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | 101 | 102 |

