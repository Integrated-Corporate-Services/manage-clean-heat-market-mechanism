@APITest
Feature: Verify CHMM POST to upload a file to a note for an organisation API works as expected
    CHMM POST to upload a file to a note for an organisation API test
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


    Scenario Outline: 1. Verify that POST to upload a file to a notes for an organisation by admin user returns 204

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token

        Then the response status code should be 204
        And the user send a GET request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file-names" with authentication token
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


    Scenario Outline: 2. Verify that POST to upload a file to a notes for an organisation by manufacturer user returns 403

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token

        Then the response status code should be 403

        Examples:
            | authEmailId                     | schemeYearId                         | fileNames          |
            | triad.test.acc.4+mfr1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx |


    Scenario Outline: 3. Verify that POST to upload un-supported/corrupted files to a notes for an organisation by an admin user returns 400/413

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token

        Then the response status code should be <statusCode>

        Examples:
            | authEmailId                       | schemeYearId                         | fileNames                             | statusCode |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | example.json                          | 400        |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_text_file.txt                    | 400        |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | visual_studio_code.exe                | 400        |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx;test_doc_file.docx | 400        |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eicar.txt                             | 400        |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | very_large_file.docx                  | 413        |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | corrupt_mp3_file.mp3                  | 413        |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | corrupt_docx_file.docx                | 413        |


    Scenario Outline: 4. Verify that POST to upload files to a note for an organisation with invalid organisationId/schemeYearId returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/<organisationId>/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token

        Then the response status code should be 400

        Examples:
            | authEmailId                       | schemeYearId                         | organisationId                       | fileNames          |
            # Invalid organisationId
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx |
            # Invalid schemeYearId
            | triad.test.acc.2+admin1@gmail.com | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | test_doc_file.docx |


    Scenario Outline: 5. Verify that POST to upload files to a note for an organisation wihout authentication token returns 401

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" without authentication token

        Then the response status code should be 401

        Examples:
            | authEmailId                     | schemeYearId                         | fileNames          |
            | triad.test.acc.4+mfr1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx |
