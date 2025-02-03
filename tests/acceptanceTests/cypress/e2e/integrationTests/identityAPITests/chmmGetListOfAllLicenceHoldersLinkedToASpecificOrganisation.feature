@APITest
Feature: Verify CHMM Get list of all licence holders linked to an organisation API works as expected
    CHMM Get list of all licence holders linked to an organisation API test


    Scenario Outline: 1. Verify that GET licence holder linked to an organisation by an admin user returns 200

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
                "startDate": "<startDate>"
            }
            """

        When the user send a GET request to "/identity/licenceholders/linked-to/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array should contain <itemCount> items
        And the response body array for linked licence holder with name "newRandom" should contain the following data:
            | key               | value                 |
            | id                | uuid                  |
            | licenceHolderId   | <mcsManufacturerId>   |
            | licenceHolderName | <mcsManufacturerName> |
            | organisationId    | newRandom             |
            | organisationName  | newRandom             |
            | startDate         | <startDate>           |
            | endDate           | null                  |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | itemCount | startDate  |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 1         | 2024-03-01 |


    Scenario Outline: 2. Verify that GET list of all licence holders linked to an organisation by an admin user returns 200

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
                "startDate": "<startDate>"
            }
            """
        And the user send a POST request to "/identity/licenceholders" to create a licence holder with authentication token and following data:
            | key                 | value                 |
            | mcsManufacturerId   | <mcsManufacturerId>   |
            | mcsManufacturerName | <mcsManufacturerName> |
        And the user send a POST request to "/identity/licenceholders/licenceHolderId/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "<startDate>"
            }
            """

        When the user send a GET request to "/identity/licenceholders/linked-to/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array should contain <itemCount> items
        And the response body array for linked licence holder with name "newRandom" should contain the following data:
            | key               | value                 |
            | id                | uuid                  |
            | licenceHolderId   | <mcsManufacturerId>   |
            | licenceHolderName | <mcsManufacturerName> |
            | organisationId    | newRandom             |
            | organisationName  | newRandom             |
            | startDate         | <startDate>           |
            | endDate           | null                  |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | itemCount | startDate  |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 2         | 2024-03-01 |


    Scenario Outline: 3. Verify that GET list of all licence holders linked to an organisation by a manufacturer user returns 200

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
                "startDate": "<startDate>"
            }
            """
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a GET request to "/identity/licenceholders/linked-to/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array should contain <itemCount> items
        And the response body array for linked licence holder with name "newRandom" should contain the following data:
            | key               | value                 |
            | id                | uuid                  |
            | licenceHolderId   | <mcsManufacturerId>   |
            | licenceHolderName | <mcsManufacturerName> |
            | organisationId    | newRandom             |
            | organisationName  | newRandom             |
            | startDate         | <startDate>           |
            | endDate           | null                  |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | itemCount | startDate  |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 1         | 2024-03-01 |


    Scenario Outline: 4. Verify that GET list of all licence holders linked to an organisation by an admin user for invalid organisation id returns 404

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/identity/licenceholders/linked-to/<organisationId>" with authentication token

        Then the response status code should be 404

        Examples:
            | authEmailId                       | organisationId                      |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8 |

    
    Scenario Outline: 5. Verify that GET list of all licence holders linked to an organisation without authentication token returns 401

        When the user send a GET request to "/identity/licenceholders/linked-to/<organisationId>" without authentication token

        Then the response status code should be 401

        Examples:
            | organisationId                       |
            | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 |


    Scenario Outline: 6. Verify that GET list of all licence holders linked to an organisation by a different manufacturer user returns 403

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
        And the user has authentication token for "triad.test.acc.4+mfr1@gmail.com" manufacturer user email id

        When the user send a GET request to "/identity/licenceholders/linked-to/organisationId" with authentication token

        Then the response status code should be 403
        
        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           |
