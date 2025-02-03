@APITest
Feature: Verify CHMM POST to link a licence holder to an organisation API works as expected
    CHMM POST to link a licence holder to an organisation API test

    
    Scenario Outline: 1. Verify that POST to link a licence holder to an organisation by an admin user returns 204

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

        When the user send a POST request to "/identity/licenceholders/licenceHolderId/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "<startDate>"
            }
            """

        Then the response status code should be 201
        And the user send a GET request to "/identity/licenceholders/linked-to/organisationId" with authentication token
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
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 1         | 2028-03-01 |


    Scenario Outline: 2. Verify that POST to link more than one licence holders to an organisation by an admin user returns 204

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

        When the user send a POST request to "/identity/licenceholders/licenceHolderId/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "<startDate>"
            }
            """

        Then the response status code should be 201
        And the user send a GET request to "/identity/licenceholders/linked-to/organisationId" with authentication token
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


    Scenario Outline: 3. Verify that POST to link a licence holder that is already linked to same organisation by admin user returns 400

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

        When the user send a POST request to "/identity/licenceholders/licenceHolderId/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "<startDate>"
            }
            """

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value                                                      |
            | title  | Bad Request                                                |
            | status | 400                                                        |
            | detail | Licence holder licenceHolderId already has an ongoing link |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | itemCount | startDate  |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 1         | 2024-03-01 |


    Scenario Outline: 4. Verify that POST to link a licence holder that is already linked to a different organisation by admin user returns 400

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

        When the user send a POST request to "/identity/licenceholders/licenceHolderId/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "<startDate>"
            }
            """

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value                                                      |
            | title  | Bad Request                                                |
            | status | 400                                                        |
            | detail | Licence holder licenceHolderId already has an ongoing link |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | itemCount | startDate  |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 1         | 2024-03-01 |


    Scenario Outline: 5. Verify that POST to link a licence holder to an organisation by a manufacturer user returns 403

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
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a POST request to "/identity/licenceholders/licenceHolderId/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """

        Then the response status code should be 403

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | itemCount |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 1         |


    Scenario Outline: 6. Verify that POST to link a licence holder to an organisation by a admin user with invalid licenceHolderId/organisationId returns 404

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/identity/licenceholders/<licenceHolderId>/link-to/<organisationId>" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """

        Then the response status code should be 404

        Examples:
            | authEmailId                       | licenceHolderId                      | organisationId                       |
            | triad.test.acc.2+admin1@gmail.com | 729b2461-2f97-47c4-86d3-7d8febf8e0d  | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 |
            | triad.test.acc.2+admin1@gmail.com | 729b2461-2f97-47c4-86d3-7d8febf8e0d4 | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  |


    Scenario: 7. Verify that POST to link a licence holder to an organisation without authentication token returns 401

        When the user send a POST request to "/identity/licenceholders/729b2461-2f97-47c4-86d3-7d8febf8e0d4/link-to/c9ea5101-e45b-4f16-ac8e-e48c6d83ec81" to link licence holder to organisation without authentication token

        Then the response status code should be 401