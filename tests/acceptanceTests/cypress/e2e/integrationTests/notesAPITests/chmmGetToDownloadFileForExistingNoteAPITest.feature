@APITest
Feature: Verify CHMM GET to download file for an existing notes for an organisation API works as expected
    CHMM GET to download file for an existing notes for an organisation API test
    # NOTE: Organisation with Id = c7e522cc-8d46-4218-8e83-70b9a3d15f38 and email address = triad.test.acc.4+mfr1@gmail.com is present in the DB
    # Sequence of commands to run => Upload file to new-note -> add new-note -> Get downaload file with the note id


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


    Scenario Outline: 1. Verify that GET to download file for an existing note from an organisation by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token
        And the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value                        |
            | organisationId | newRandom                    |
            | schemeYearId   | <schemeYearId>               |
            | details        | Admin API test notes !"£$%^" |

        When the user send a GET request to "/notes/manufacturer/organisationId/year/<schemeYearId>/note/noteId/download?fileName=<fileNames>" to download notes files with authentication token

        Then the response status code should be 200

        Examples:
            | authEmailId                       | schemeYearId                         | fileNames                  |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx         |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file-sample_100kB.doc      |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | pptx_test_file.pptx        |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | ppt_test_file.ppt          |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | xls_test_file.xls          |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | xlsx_test_file.xlsx        |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file_example_JPG_500kB.jpg |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file_example_PNG_500kB.png |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | sample_640×426.bmp         |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_eml_file.eml          |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_msg_file.msg          |


    Scenario Outline: 2. Verify that GET to download file for an existing note from an organisation by a manufacturer user returns 403

        Given the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token
        And the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value                        |
            | organisationId | newRandom                    |
            | schemeYearId   | <schemeYearId>               |
            | details        | Admin API test notes !"£$%^" |
        And the user has authentication token for "newRandom" email id

        When the user send a GET request to "/notes/manufacturer/organisationId/year/<schemeYearId>/note/noteId/download?fileName=<fileNames>" to download notes files with authentication token

        Then the response status code should be 403

        Examples:
            | authEmailId                     | schemeYearId                         | fileNames          |
            | triad.test.acc.4+mfr1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx |

    
    Scenario Outline: 3. Verify that GET to download file for an existing note with invalid organisationId/schemeYearId returns 400

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/notes/manufacturer/organisationId/year/d525e380-4aee-40e9-a7f0-1784d8cb49d9/new-note/file" to upload "<fileNames>" file for "files" with authentication token
        And the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value                                |
            | organisationId | newRandom                            |
            | schemeYearId   | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            | details        | Admin API test notes !"£$%^"         |

        When the user send a GET request to "/notes/manufacturer/<organisationId>/year/<schemeYearId>/note/<noteId>/download?fileName=<fileNames>" to download notes files with authentication token

        Then the response status code should be <statusCode>

        Examples:
            | authEmailId                       | schemeYearId                         | organisationId                       | fileNames          | noteId                               | statusCode |
            # Invalid organisationId
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx | noteId                               | 400        |
            # Invalid schemeYearId
            | triad.test.acc.2+admin1@gmail.com | a1ab57ad-b2a5-4487-ad46-e206dd0345d8 | organisationId                       | test_doc_file.docx | noteId                               | 400        |
            # schemeYearId is set to organisation Id
            | triad.test.acc.2+admin1@gmail.com | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | test_doc_file.docx | noteId                               | 400        |
            # Invalid noteId
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | test_doc_file.docx | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | 404        |


    Scenario Outline: 4. Verify that GET to download non-existing file for an existing note by admin user returns 404

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token
        And the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value                        |
            | organisationId | newRandom                    |
            | schemeYearId   | <schemeYearId>               |
            | details        | Admin API test notes !"£$%^" |

        When the user send a GET request to "/notes/manufacturer/organisationId/year/<schemeYearId>/note/noteId/download?fileName=<downloadFileNames>" to download notes files with authentication token

        Then the response status code should be 404

        Examples:
            | authEmailId                       | schemeYearId                         | fileNames          | downloadFileNames    |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx | test_doc_file_1.docx |


    Scenario: 5. Verify that GET to download file for an existing note without authentication token returns 401

        When the user send a GET request to "/notes/manufacturer/c7e522cc-8d46-4218-8e83-70b9a3d15f38/year/d525e380-4aee-40e9-a7f0-1784d8cb49d9/note/337c098b-c677-41ae-a3d4-e1571c95a3c3/download?fileName=test_doc_file.docx" to download notes files without authentication token

        Then the response status code should be 401
