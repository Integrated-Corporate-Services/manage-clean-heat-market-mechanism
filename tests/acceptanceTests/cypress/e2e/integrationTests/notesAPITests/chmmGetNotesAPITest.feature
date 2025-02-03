@APITest
Feature: Verify CHMM GET notes for an organisation API works as expected
    CHMM GET notes for an organisation API test
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


    Scenario Outline: 1. Verify that GET notes for an organisation by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | details        | <note>         |

        When the user send a GET request to "/notes/manufacturer/organisationId/year/<schemeYearId>/notes" with authentication token

        Then the response status code should be 200
        And the response body array for get notes for "newRandom" organisation should contain the following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | details        | <note>         |

        Examples:
            | authEmailId                       | schemeYearId                         | note                                                                               |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | Admin API test notes !"£$%^"                                                       |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | adsdadhdhakhad akdhasjdasd ad sjjasdhakdhakdh asdhsahadjaf we2983423572905 1`123$% |


    Scenario Outline: 2. Verify that GET notes for an organisation by manufacturer user returns 403

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/notes/manufacturer/c7e522cc-8d46-4218-8e83-70b9a3d15f38/year/<schemeYearId>/notes" with authentication token

        Then the response status code should be 403

        Examples:
            | authEmailId                     | schemeYearId                         |
            | triad.test.acc.4+mfr1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |


    Scenario Outline: 3. Verify that GET notes with invalid organisationId/schemeYearId returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/notes/manufacturer/<organisationId>/year/<schemeYearId>/notes" with authentication token

        Then the response status code should be <statusCode>

        Examples:
            | authEmailId                       | schemeYearId                         | organisationId                       | statusCode |
            # Invalid Organisation Id
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 400        |
            # Invalid schemeYearId
            | newRandom                         | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | 403        |


    Scenario: 4. Verify that GET notes without authentication token returns 401

        When the user send a GET request to "/notes/manufacturer/c7e522cc-8d46-4218-8e83-70b9a3d15f38/year/d525e380-4aee-40e9-a7f0-1784d8cb49d9/notes" without authentication token

        Then the response status code should be 401
