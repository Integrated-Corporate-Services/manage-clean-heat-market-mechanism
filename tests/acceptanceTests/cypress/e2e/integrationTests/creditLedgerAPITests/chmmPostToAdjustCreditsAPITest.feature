@APITest
Feature: Verify CHMM POST to adjust credits for an organisation API works as expected
    CHMM POST to adjust credits for an organisation API test
    # NOTE: Licence holder 'API LicenceHolder7' id = a8c0f616-82c9-4761-8010-e06793a3d0b1 is present in the DB
    # Organisation with Id = c7e522cc-8d46-4218-8e83-70b9a3d15f38 and email address = triad.test.acc.4+mfr1@gmail.com is present in the DB

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


    # Bug - https://triadgroupplc.atlassian.net/browse/CHMMBETA-804. Change the transfers to decimals once bug is fixed
    Scenario Outline: 1. Verify that POST to adjust credits for an organisation by admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | <value>        |

        Then the response status code should be 201
        And the user send a GET request to "/creditledger/manufacturer/organisationId/year/<schemeYearId>/credit-balance" with authentication token
        And the response json body should contain the following data:
            | key           | value   |
            | creditBalance | <value> |

        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 25    |
    # | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 25.5  |


    # Bug - https://triadgroupplc.atlassian.net/browse/CHMMBETA-804. Change the transfers to decimals once bug is fixed
    Scenario Outline: 2. Verify that POST to adjust credits for an organisation by manufacturer user returns 403

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | 25             |

        Then the response status code should be 403

        Examples:
            | authEmailId | schemeYearId                         |
            | newRandom   | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |


    # Adjust credits that are aquired from a licence holder
    @UnlinkLicenceHolder
    Scenario Outline: 3. Verify that POST to adjust credits that are aquired from a licence holer by an organisation returns 201

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/identity/licenceholders/<licenceHolderId>/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """

        When the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | <value>        |

        Then the response status code should be 201
        And the user send a GET request to "/creditledger/manufacturer/organisationId/year/<schemeYearId>/credit-balance" with authentication token
        And the response json body should contain the following data:
            | key           | value |
            | creditBalance | 88    |
        And the user send a POST request to "/identity/licenceholders/<licenceHolderId>/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

        Examples:
            | authEmailId                       | schemeYearId                         | licenceHolderId                      | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | a8c0f616-82c9-4761-8010-e06793a3d0b1 | -25   |

    # Adjust credits with invalid organisation and schemeYearId
    Scenario Outline: 4. Verify that POST to adjust credits with invalid organisationId/schemeYearId returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value            |
            | organisationId | <organisationId> |
            | schemeYearId   | <schemeYearId>   |
            | value          | 12               |

        Then the response status code should be <statusCode>

        Examples:
            | authEmailId                       | schemeYearId                         | organisationId                       | statusCode |
            # Invalid Organisation Id
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 400        |
            # Invalid schemeYearId
            | newRandom                         | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | 403        |


    # Adjust credits without authentication token
    Scenario: 5. Verify that POST to adjust credits without authentication returns 401

        When the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and without authentication token:
            | key            | value                                |
            | organisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId   | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            | value          | 11                                   |

        Then the response status code should be 401


    # errors
    Scenario Outline: 6. Verify that POST to adjust negative credits for an organisation by admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | <value>        |

        Then the response status code should be 400

        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | -25   |
