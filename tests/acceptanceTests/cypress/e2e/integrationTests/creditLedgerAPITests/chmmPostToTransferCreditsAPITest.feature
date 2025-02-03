@APITest
Feature: Verify CHMM POST to transfer credits between organisations API works as expected
    CHMM POST to transfer credits between organisations API test
    # NOTE: Licence holder 'API LicenceHolder8' id = fe72e94d-026d-42c8-a5e1-46f5e766107d is present in the DB
    # Organisation with Id = c7e522cc-8d46-4218-8e83-70b9a3d15f38 and email address = triad.test.acc.4+mfr1@gmail.com is present in the DB and is a Scheme participant


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
    # | isNonSchemeParticipant     | false     |


    # Bug - https://triadgroupplc.atlassian.net/browse/CHMMBETA-804. Change the transfers to decimals once bug is fixed
    Scenario Outline: 1. Verify that POST to transfer credits between organisations by an admin/manufacturer user returns 201

        Given the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | 25             |
        And the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | newRandom                            |
            | destinationOrganisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId              | <schemeYearId>                       |
            | value                     | <value>                              |

        Then the response status code should be 201
        And the user send a GET request to "/creditledger/manufacturer/organisationId/year/<schemeYearId>/credit-balance" with authentication token
        And the response json body should contain the following data:
            | key           | value           |
            | creditBalance | <creditBalance> |

        Examples:
            | authEmailId                       | schemeYearId                         | value | creditBalance |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 7     | 18            |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 7.5   | 17.5          |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 0.5   | 24.5          |
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 7     | 18            |
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 7.5   | 17.5          |
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 0.5   | 24.5          |


    @UnlinkLicenceHolder
    Scenario Outline: 2. Verify that POST to transfer credits between organisations that are aquired from licence holder by an admin/manufacturer user returns 201

        Given the user has authentication token for "<adminEmailId>" email id
        And the user send a POST request to "/identity/licenceholders/<licenceHolderId>/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """
        And the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | newRandom                            |
            | destinationOrganisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId              | <schemeYearId>                       |
            | value                     | 7.5                                  |

        Then the response status code should be 201
        And the user send a GET request to "/creditledger/manufacturer/organisationId/year/<schemeYearId>/credit-balance" with authentication token
        And the response json body should contain the following data:
            | key           | value |
            | creditBalance | 105.5 |
        And the user has authentication token for "<adminEmailId>" email id
        And the user send a POST request to "/identity/licenceholders/<licenceHolderId>/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

        Examples:
            | adminEmailId                      | authEmailId                       | schemeYearId                         | licenceHolderId                      |
            | triad.test.acc.2+admin1@gmail.com | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | fe72e94d-026d-42c8-a5e1-46f5e766107d |
            | triad.test.acc.2+admin1@gmail.com | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | fe72e94d-026d-42c8-a5e1-46f5e766107d |


    Scenario Outline: 3. Verify that POST to transfer credits between organisations by a manufacturer user for a different organisation returns 403

        Given the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | 25             |
        And the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | newRandom                            |
            | destinationOrganisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId              | <schemeYearId>                       |
            | value                     | 7                                    |

        Then the response status code should be 403

        Examples:
            | authEmailId                     | schemeYearId                         |
            | triad.test.acc.4+mfr1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |


    Scenario Outline: 4. Verify that POST to transfer credits with invalid organisationId/schemeYearId returns 400

        Given the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | 25             |
        And the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | <organisationId>                     |
            | destinationOrganisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId              | <schemeYearId>                       |
            | value                     | 7                                    |

        Then the response status code should be <statusCode>

        Examples:
            | authEmailId                       | schemeYearId                         | organisationId                       | statusCode |
            # Invalid Organisation Id
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 400        |
            # Invalid schemeYearId
            | newRandom                         | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | 403        |


    # Transfer to their own organisation
    Scenario Outline: 5. Verify that POST to transfer credits from and to their own organisation returns 400

        Given the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | 25             |
        And the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value          |
            | organisationId            | newRandom      |
            | destinationOrganisationId | newRandom      |
            | schemeYearId              | <schemeYearId> |
            | value                     | 7              |

        Then the response status code should be 400

        Examples:
            | authEmailId                       | schemeYearId                         |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |


    # Transfer to an non-existing organisation
    Scenario Outline: 6. Verify that POST to transfer credits to a non-existing organisation returns 400

        Given the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | 25             |
        And the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | newRandom                            |
            | destinationOrganisationId | 256c66df-3be3-430f-9f1c-1a4b30b856b0 |
            | schemeYearId              | <schemeYearId>                       |
            | value                     | 7                                    |

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value |
            | status | 400   |

        Examples:
            | authEmailId                       | schemeYearId                         |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |


    # Transfer without having any credits
    Scenario Outline: 7. Verify that POST to transfer credits from an organisation who has zero credits returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | newRandom                            |
            | destinationOrganisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId              | <schemeYearId>                       |
            | value                     | 7                                    |

        Then the response status code should be 400

        Examples:
            | authEmailId                       | schemeYearId                         |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |


    # Transfer more credits than they have
    Scenario Outline: 8. Verify that POST to transfer credits more credits than the organisation have returns 400

        Given the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | 25             |
        And the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | newRandom                            |
            | destinationOrganisationId | 256c66df-3be3-430f-9f1c-1a4b30b856b0 |
            | schemeYearId              | <schemeYearId>                       |
            | value                     | 50                                   |

        Then the response status code should be 400

        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 50    |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 1.25  |
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 50    |
            | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 1.25  |


    # Transfer without authentication token
    Scenario: 9. Verify that POST to transfer credits without authentication token returns 401

        When the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and without authentication token:
            | key                       | value                                |
            | organisationId            | newRandom                            |
            | destinationOrganisationId | 256c66df-3be3-430f-9f1c-1a4b30b856b0 |
            | schemeYearId              | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            | value                     | 50                                   |

        Then the response status code should be 401

    
    Scenario: 10. Verify that POST to transfer credits to a non-scheme participant organisation returns 400

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        And the user send a PUT request to "/identity/organisations/edit/scheme-participation" to set scheme participation flag to "true" with authentication token

        When the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | destinationOrganisationId | newRandom                            |
            | schemeYearId              | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            | value                     | 5                                    |

        Then the response status code should be 400