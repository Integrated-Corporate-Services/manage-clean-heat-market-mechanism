@APITest
Feature: Verify CHMM GET a list of audit items for an organisation API works as expected
    CHMM GET a list of audit items for an organisation API test
    # NOTE: Organisation with Id = c7e522cc-8d46-4218-8e83-70b9a3d15f38 and email address = triad.test.acc.4+mfr1@gmail.com is present in the DB


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


    Scenario: 1. Verify that GET a list of audit items for an organisation by an admin user returns 200

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a GET request to "/systemaudit/organisation/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array should not be empty
        And the response body array has json object with key "eventName" and value "Approve Manufacturer Application" should contain the following data:
            | key                          | value                            |
            | eventName                    | Approve Manufacturer Application |
            | createdBy                    | Triad Test 2 + 1                 |
            | auditItemRows[0].label       | Comment                          |
            | auditItemRows[0].simpleValue | Created by admin                 |
            | auditItemRows[0].objectValue | null                             |
            | auditItemRows[1].label       | Account Approval Files           |
            | auditItemRows[1].simpleValue |                                  |
            | auditItemRows[1].objectValue | null                             |


    Scenario: 2. Verify that GET a list of audit items for an organisation by an admin user returns 403

        Given the user has authentication token for "triad.test.acc.4+mfr1@gmail.com" email id

        When the user send a GET request to "/systemaudit/organisation/organisationId" with authentication token

        Then the response status code should be 403


    Scenario: 3. Verify that GET a list of audit items for an organisation without authentication token return 401

        When the user send a GET request to "/systemaudit/organisation/organisationId" without authentication token

        Then the response status code should be 401


    Scenario Outline: 4. Verify that GET a list of boiler sales audit items for an organisation by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to submit boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | oil            | <oil>          |
            | gas            | <gas>          |
        And the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to fetch boiler sales figures with authentication token

        When the user send a GET request to "/systemaudit/organisation/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array has json object with key "eventName" and value "Approve Manufacturer Application" should contain the following data:
            | key                          | value                            |
            | eventName                    | Approve Manufacturer Application |
            | createdBy                    | Triad Test 2 + 1                 |
            | auditItemRows[0].label       | Comment                          |
            | auditItemRows[0].simpleValue | Created by admin                 |
            | auditItemRows[0].objectValue | null                             |
            | auditItemRows[1].label       | Account Approval Files           |
            | auditItemRows[1].simpleValue |                                  |
            | auditItemRows[1].objectValue | null                             |
        And the response body array has json object with key "eventName" and value "Submit Annual Boiler Sales" should contain the following data:
            | key                          | value                      |
            | eventName                    | Submit Annual Boiler Sales |
            | createdBy                    | Triad Test 2 + 1           |
            | auditItemRows[0].label       | Scheme Year Id             |
            | auditItemRows[0].simpleValue | <schemeYearId>             |
            | auditItemRows[0].objectValue | null                       |
            | auditItemRows[1].label       | Oil                        |
            | auditItemRows[1].simpleValue | <oil>                      |
            | auditItemRows[1].objectValue | null                       |
            | auditItemRows[2].label       | Gas                        |
            | auditItemRows[2].simpleValue | <gas>                      |
            | auditItemRows[2].objectValue | null                       |
        And the response body array has json object with key "eventName" and value "Create Annual Obligation" should contain the following data:
            | key                          | value                    |
            | eventName                    | Create Annual Obligation |
            | createdBy                    | Triad Test 2 + 1         |
            | auditItemRows[0].label       | Scheme Year Id           |
            | auditItemRows[0].simpleValue | <schemeYearId>           |
            | auditItemRows[0].objectValue | null                     |
            | auditItemRows[1].label       | Transaction Date         |
            | auditItemRows[1].objectValue | null                     |
            | auditItemRows[2].label       | Override                 |
            | auditItemRows[2].simpleValue | False                    |
            | auditItemRows[2].objectValue | null                     |
            | auditItemRows[3].label       | Gas                      |
            | auditItemRows[3].simpleValue | <gas>                    |
            | auditItemRows[3].objectValue | null                     |
            | auditItemRows[4].label       | Oil                      |
            | auditItemRows[4].simpleValue | <oil>                    |
            | auditItemRows[4].objectValue | null                     |
        And the response body array has json object with key "eventName" and value "Edit Organisation Scheme Participation" should contain the following data:
            | key                          | value                                  |
            | eventName                    | Edit Organisation Scheme Participation |
            | createdBy                    | Triad Test 2 + 1                       |
            | auditItemRows[0].label       | Is Non Scheme Participant              |
            | auditItemRows[0].simpleValue | False                                  |
            | auditItemRows[0].objectValue | null                                   |

        Examples:
            | authEmailId                       | schemeYearId                         | oil | gas |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 101 | 102 |


    Scenario Outline: 5. Verify that GET a list of link licence holder audit items for an organisation by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
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

        When the user send a GET request to "/systemaudit/organisation/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array has json object with key "eventName" and value "Approve Manufacturer Application" should contain the following data:
            | key                          | value                            |
            | eventName                    | Approve Manufacturer Application |
            | createdBy                    | Triad Test 2 + 1                 |
            | auditItemRows[0].label       | Comment                          |
            | auditItemRows[0].simpleValue | Created by admin                 |
            | auditItemRows[0].objectValue | null                             |
            | auditItemRows[1].label       | Account Approval Files           |
            | auditItemRows[1].simpleValue |                                  |
            | auditItemRows[1].objectValue | null                             |
        And the response body array has json object with key "eventName" and value "Link Licence Holder" should contain the following data:
            | key                          | value               |
            | eventName                    | Link Licence Holder |
            | createdBy                    | Triad Test 2 + 1    |
            | auditItemRows[0].label       | Licence Holder Id   |
            # | auditItemRows[0].simpleValue | licenceHolderId     |
            | auditItemRows[0].objectValue | null                |
            | auditItemRows[1].label       | Start Date          |
            | auditItemRows[1].simpleValue | 2024-03-01          |
            | auditItemRows[1].objectValue | null                |


        Examples:
            | authEmailId                       | mcsManufacturerId | mcsManufacturerName |
            | triad.test.acc.2+admin1@gmail.com | newRandom         | newRandom           |


    # Notes event
    Scenario Outline: 6. Verify that GET a list of audit items for adding notes for an organisation by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | details        | <note>         |

        When the user send a GET request to "/systemaudit/organisation/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array has json object with key "eventName" and value "Approve Manufacturer Application" should contain the following data:
            | key                          | value                            |
            | eventName                    | Approve Manufacturer Application |
            | createdBy                    | Triad Test 2 + 1                 |
            | auditItemRows[0].label       | Comment                          |
            | auditItemRows[0].simpleValue | Created by admin                 |
            | auditItemRows[0].objectValue | null                             |
            | auditItemRows[1].label       | Account Approval Files           |
            | auditItemRows[1].simpleValue |                                  |
            | auditItemRows[1].objectValue | null                             |
        And the response body array has json object with key "eventName" and value "Add Manufacturer Note" should contain the following data:
            | key                          | value                 |
            | eventName                    | Add Manufacturer Note |
            | createdBy                    | Triad Test 2 + 1      |
            | auditItemRows[0].label       | Scheme Year Id        |
            | auditItemRows[0].simpleValue | <schemeYearId>        |
            | auditItemRows[0].objectValue | null                  |
            | auditItemRows[1].label       | Details               |
            | auditItemRows[1].simpleValue | <note>                |
            | auditItemRows[1].objectValue | null                  |

        Examples:
            | authEmailId                       | schemeYearId                         | note                       |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | Admin API test notes !£$%^ |


    # Amend obligations event
    Scenario Outline: 7. Verify that GET a list of audit items for amend obligations for an organisation by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/obligation/amend" to amend an obligation with authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | <value>        |

        When the user send a GET request to "/systemaudit/organisation/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array has json object with key "eventName" and value "Approve Manufacturer Application" should contain the following data:
            | key                          | value                            |
            | eventName                    | Approve Manufacturer Application |
            | createdBy                    | Triad Test 2 + 1                 |
            | auditItemRows[0].label       | Comment                          |
            | auditItemRows[0].simpleValue | Created by admin                 |
            | auditItemRows[0].objectValue | null                             |
            | auditItemRows[1].label       | Account Approval Files           |
            | auditItemRows[1].simpleValue |                                  |
            | auditItemRows[1].objectValue | null                             |
        And the response body array has json object with key "eventName" and value "Amend Obligation" should contain the following data:
            | key                          | value            |
            | eventName                    | Amend Obligation |
            | createdBy                    | Triad Test 2 + 1 |
            | auditItemRows[0].label       | Scheme Year Id   |
            | auditItemRows[0].simpleValue | <schemeYearId>   |
            | auditItemRows[0].objectValue | null             |
            | auditItemRows[1].label       | Value            |
            | auditItemRows[1].simpleValue | <value>          |
            | auditItemRows[1].objectValue | null             |

        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | -100  |

    # Amend credits event
    Scenario Outline: 8. Verify that GET a list of audit items for amend credits for an organisation by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | <value>        |

        When the user send a GET request to "/systemaudit/organisation/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array has json object with key "eventName" and value "Approve Manufacturer Application" should contain the following data:
            | key                          | value                            |
            | eventName                    | Approve Manufacturer Application |
            | createdBy                    | Triad Test 2 + 1                 |
            | auditItemRows[0].label       | Comment                          |
            | auditItemRows[0].simpleValue | Created by admin                 |
            | auditItemRows[0].objectValue | null                             |
            | auditItemRows[1].label       | Account Approval Files           |
            | auditItemRows[1].simpleValue |                                  |
            | auditItemRows[1].objectValue | null                             |
        And the response body array has json object with key "eventName" and value "Adjust Credits" should contain the following data:
            | key                          | value            |
            | eventName                    | Adjust Credits   |
            | createdBy                    | Triad Test 2 + 1 |
            | auditItemRows[0].label       | Scheme Year Id   |
            | auditItemRows[0].simpleValue | <schemeYearId>   |
            | auditItemRows[0].objectValue | null             |
            | auditItemRows[1].label       | Value            |
            | auditItemRows[1].simpleValue | <value>          |
            | auditItemRows[1].objectValue | null             |

        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 100.0 |


    # Credits transfer
    Scenario Outline: 9. Verify that GET a list of audit items for credits transfer for an organisation by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | value          | 25             |
        And the user send a POST request to "/creditledger/transfer-credits" to transfer credits with the following data and authentication token:
            | key                       | value                                |
            | organisationId            | newRandom                            |
            | destinationOrganisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId              | <schemeYearId>                       |
            | value                     | 7                                    |

        When the user send a GET request to "/systemaudit/organisation/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array has json object with key "eventName" and value "Approve Manufacturer Application" should contain the following data:
            | key                          | value                            |
            | eventName                    | Approve Manufacturer Application |
            | createdBy                    | Triad Test 2 + 1                 |
            | auditItemRows[0].label       | Comment                          |
            | auditItemRows[0].simpleValue | Created by admin                 |
            | auditItemRows[0].objectValue | null                             |
            | auditItemRows[1].label       | Account Approval Files           |
            | auditItemRows[1].simpleValue |                                  |
            | auditItemRows[1].objectValue | null                             |
        And the response body array has json object with key "eventName" and value "Adjust Credits" should contain the following data:
            | key                          | value            |
            | eventName                    | Adjust Credits   |
            | createdBy                    | Triad Test 2 + 1 |
            | auditItemRows[0].label       | Scheme Year Id   |
            | auditItemRows[0].simpleValue | <schemeYearId>   |
            | auditItemRows[0].objectValue | null             |
            | auditItemRows[1].label       | Value            |
            | auditItemRows[1].simpleValue | <value>          |
            | auditItemRows[1].objectValue | null             |
        And the response body array has json object with key "eventName" and value "Transfer Credits" should contain the following data:
            | key                          | value                                |
            | eventName                    | Transfer Credits                     |
            | createdBy                    | Triad Test 2 + 1                     |
            | auditItemRows[0].label       | Destination Organisation Id          |
            | auditItemRows[0].simpleValue | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | auditItemRows[0].objectValue | null                                 |
            | auditItemRows[1].label       | Scheme Year Id                       |
            | auditItemRows[1].simpleValue | <schemeYearId>                       |
            | auditItemRows[1].objectValue | null                                 |
            | auditItemRows[2].label       | Value                                |
            | auditItemRows[2].simpleValue | 7.0                                  |
            | auditItemRows[2].objectValue | null                                 |

        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 25.0  |
