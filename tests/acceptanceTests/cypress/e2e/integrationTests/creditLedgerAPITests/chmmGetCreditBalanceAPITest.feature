@APITest
Feature: Verify CHMM GET credit balance for an organisation API works as expected
    CHMM GET credit balance API test
    # NOTE: Licence holder 'API LicenceHolder6' id = 5f79a5a9-2d46-4149-92df-d4c610ce5c3b is present in the DB
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
        And the user save "5f79a5a9-2d46-4149-92df-d4c610ce5c3b" licence holder Id
        And the user send a POST request to "/identity/licenceholders/5f79a5a9-2d46-4149-92df-d4c610ce5c3b/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """


    @UnlinkLicenceHolder
    Scenario Outline: 1. Verify that GET credit balance for an organisation by an admin/manufacturer user returns 200

        Given the user has authentication token for "<adminEmailId>" email id
        And the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | 25             |
        And the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | -5             |
        And the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | newRandom                            |
            | destinationOrganisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId              | <schemeYearId>                       |
            | value                     | 7                                    |
        And the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | destinationOrganisationId | newRandom                            |
            | schemeYearId              | <schemeYearId>                       |
            | value                     | 4                                    |
        And the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/creditledger/manufacturer/organisationId/year/<schemeYearId>/credit-balance" with authentication token

        Then the response status code should be 200
        And the response json body should contain the following data:
            | key           | value |
            | creditBalance | 130   |
        And the user send a POST request to "/identity/licenceholders/5f79a5a9-2d46-4149-92df-d4c610ce5c3b/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """
        Examples:
            | adminEmailId                      | authEmailId                       | schemeYearId                         |
            | triad.test.acc.2+admin1@gmail.com | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            | triad.test.acc.2+admin1@gmail.com | newRandom                         | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |


    @UnlinkLicenceHolder
    Scenario Outline: 2. Verify that GET credit balance by a manufacturer user for a different organisation returns 403

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/creditledger/manufacturer/organisationId/year/<schemeYearId>/credit-balance" with authentication token

        Then the response status code should be 403

        And the user send a POST request to "/identity/licenceholders/5f79a5a9-2d46-4149-92df-d4c610ce5c3b/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

        Examples:
            | authEmailId                     | schemeYearId                         |
            | triad.test.acc.4+mfr1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |


    @UnlinkLicenceHolder
    Scenario Outline: 3. Verify that GET credit balance by an admin/manufacturer user with invalid organisationId/schemeYearId returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/creditledger/manufacturer/<organisationId>/year/<schemeYearId>/credit-balance" with authentication token

        Then the response status code should be <statusCode>

        And the user send a POST request to "/identity/licenceholders/5f79a5a9-2d46-4149-92df-d4c610ce5c3b/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

        Examples:
            | authEmailId                       | schemeYearId                         | organisationId                       | statusCode |
            # Invalid Organisation Id
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 400        |
            # Invalid schemeYearId
            | newRandom                         | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | 403        |


    @UnlinkLicenceHolder
    Scenario: 4. Verify that GET credit balance without authentication token returns 401

        When the user send a GET request to "/creditledger/manufacturer/c7e522cc-8d46-4218-8e83-70b9a3d15f38/year/d525e380-4aee-40e9-a7f0-1784d8cb49d9/credit-balance" without authentication token

        Then the response status code should be 401

        And the user send a POST request to "/identity/licenceholders/5f79a5a9-2d46-4149-92df-d4c610ce5c3b/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """
            