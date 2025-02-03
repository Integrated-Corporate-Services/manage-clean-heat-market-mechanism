@APITest
Feature: Verify CHMM GET uploaded file names for new notes for an organisation API works as expected
    CHMM GET uploaded file names for new notes for an organisation API test
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


    Scenario Outline: 1. Verify that GET uploaded file names for a new note from an organisation by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token

        When the user send a GET request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file-names" with authentication token

        Then the response status code should be 200
        And the response body array should contain <itemCount> items
        And the response body array should contain the following items:
            | item        |
            | <fileNames> |

        Examples:
            | authEmailId                       | schemeYearId                         | fileNames                                                                                                    | itemCount |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx                                                                                           | 1         |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file-sample_100kB.doc;test_doc_file.docx;docx.pdf;docx.docx;test_doc_file_with_images.docx                   | 5         |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | csv_test_file.csv;pptx_test_file.pptx;ppt_test_file.ppt;xls_test_file.xls;xlsx_test_file.xlsx                | 5         |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file_example_JPG_500kB.jpg;file_example_PNG_500kB.png;sample_640×426.bmp;test_eml_file.eml;test_msg_file.msg | 5         |


    Scenario Outline: 2. Verify that GET uploaded file names for a new note by a manufacturer user returns 403

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file-names" with authentication token

        Then the response status code should be 403

        Examples:
            | authEmailId                     | schemeYearId                         |
            | triad.test.acc.4+mfr1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |


    Scenario Outline: 3. Verify that GET uploaded file names by a different admin user other than the one created them returns 204 with empty file names

        Given the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token
        And the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file-names" with authentication token

        Then the response status code should be 200
        And the response body array should contain <itemCount> items

        Examples:
            | authEmailId                       | schemeYearId                         | fileNames          | itemCount |
            | triad.test.acc.2+admin2@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx | 0         |


    Scenario Outline: 4. Verify that GET uploaded file names with invalid organisationId/schemeYearId returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/notes/manufacturer/<organisationId>/year/<schemeYearId>/new-note/file-names" with authentication token

        Then the response status code should be <statusCode>

        Examples:
            | authEmailId                       | schemeYearId                         | organisationId                       | statusCode |
            # Invalid Organisation Id
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 400        |
            # Invalid schemeYearId
            | newRandom                         | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | 403        |


    Scenario Outline: 5. Verify that GET uploaded file names without authentication token returns 401

        When the user send a GET request to "/notes/manufacturer/c7e522cc-8d46-4218-8e83-70b9a3d15f38/year/d525e380-4aee-40e9-a7f0-1784d8cb49d9/new-note/file-names" without authentication token

        Then the response status code should be 401
