@APITest
Feature: Verify CHMM POST to unlink a licence holder from an organisation API works as expected
    CHMM POST to unlink a licence holder from an organisation API test

    
    Scenario Outline: 1. Verify that POST to unlink a licence holder from an organisation by an admin user returns 204

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
        And the user send a POST request to "/identity/licenceholders" to create a licence holder with authentication token and following data:
            | key                 | value                 |
            | mcsManufacturerId   | <mcsManufacturerId>   |
            | mcsManufacturerName | <mcsManufacturerName> |
        And the user send a POST request to "/identity/licenceholders/licenceHolderId/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """

        When the user send a POST request to "/identity/licenceholders/licenceHolderId/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

        Then the response status code should be 204
        And the user send a GET request to "/identity/licenceholders/linked-to/organisationId" with authentication token
        And the response body array should contain <itemCount> items

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | itemCount |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 0         |


    Scenario Outline: 2. Verify that POST to unlink a licence holder that is already unlinked by an admin user returns 400

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
        And the user send a POST request to "/identity/licenceholders" to create a licence holder with authentication token and following data:
            | key                 | value                 |
            | mcsManufacturerId   | <mcsManufacturerId>   |
            | mcsManufacturerName | <mcsManufacturerName> |
        And the user send a POST request to "/identity/licenceholders/licenceHolderId/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """
        And the user send a POST request to "/identity/licenceholders/licenceHolderId/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

        When the user send a POST request to "/identity/licenceholders/licenceHolderId/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value                                              |
            | title  | Bad Request                                        |
            | status | 400                                                |
            | detail | Licence holder licenceHolderId is already unlinked |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           |


    Scenario Outline: 3. Verify that POST to unlink a licence holder by a manufacturer user returns 403

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
        And the user send a POST request to "/identity/licenceholders" to create a licence holder with authentication token and following data:
            | key                 | value                 |
            | mcsManufacturerId   | <mcsManufacturerId>   |
            | mcsManufacturerName | <mcsManufacturerName> |
        And the user send a POST request to "/identity/licenceholders/licenceHolderId/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a POST request to "/identity/licenceholders/licenceHolderId/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """
        Then the response status code should be 403

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           |


    Scenario Outline: 4. Verify that POST to unlink a licence holder from an organisation which do not have any linked licence holder by an admin user returns 400

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
        And the user send a POST request to "/identity/licenceholders" to create a licence holder with authentication token and following data:
            | key                 | value                 |
            | mcsManufacturerId   | <mcsManufacturerId>   |
            | mcsManufacturerName | <mcsManufacturerName> |

        When the user send a POST request to "/identity/licenceholders/licenceHolderId/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value                                              |
            | title  | Bad Request                                        |
            | status | 400                                                |
            | detail | Licence holder licenceHolderId is already unlinked |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           |


    Scenario Outline: 5. Verify that POST to unlink a licence holder by a admin user with invalid licenceHolderId/organisationId returns 404

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/identity/licenceholders/<licenceHolderId>/endlink/<organisationId>" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

        Then the response status code should be 404

        Examples:
            | authEmailId                       | licenceHolderId                     | organisationId                      |
            | triad.test.acc.2+admin1@gmail.com | 729b2461-2f97-47c4-86d3-7d8febf8e0d | 729b2461-2f97-47c4-86d3-7d8febf8e0d |


    Scenario: 6. Verify that POST to unlink a licence holder without authentication token returns 401

        When the user send a POST request to "/identity/licenceholders/729b2461-2f97-47c4-86d3-7d8febf8e0d4/endlink/729b2461-2f97-47c4-86d3-7d8febf8e0d4" to unlink licence holder to organisation without authentication token

        Then the response status code should be 401
