@APITest
Feature: Verify CHMM POST to upload rejection file to an organisation API works as expected
    CHMM POST to upload a file to rejection files API test
    # NOTE: Organisation with Id = c7e522cc-8d46-4218-8e83-70b9a3d15f38 and email address = triad.test.acc.4+mfr1@gmail.com is present in the DB


    Background: Create a manufacturer account
    
    Scenario Outline: 1. Verify that POST rejection files by a manufacturer user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user have a manufacturer onboarding application request body json with the following data:
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
        And the user send a POST request to "/identity/organisations/onboard" with json body and authentication token to submit manufacturer onboarding application
        And the user has the manufacturer onboarding application json for the new organisation

        When the user send a POST request to "/identity/organisations/organisationId/rejection-files/upload" to upload "<fileNames>" file for "files" with authentication token

        Then the response status code should be 200
        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | comment                                                 | fileNames                                                                                                     |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | API Test comment admin approve manufacturer application | docx.docx;file-sample_100kB.doc;test_doc_file.docx;docx.pdf;test_doc_file_with_images.docx |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | admin added comment                                     | csv_test_file.csv;pptx_test_file.pptx;ppt_test_file.ppt;xls_test_file.xls;xlsx_test_file.xlsx                 |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | admin added comment                                     | file_example_JPG_500kB.jpg;file_example_PNG_500kB.png;sample_640×426.bmp                                      |



    Scenario Outline: 2. Verify that POST rejection files by a manufacturer user returns 403

        Given the user has authentication token for "<authEmailId>" email id
        And the user have a manufacturer onboarding application request body json with the following data:
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
        And the user send a POST request to "/identity/organisations/onboard" with json body and authentication token to submit manufacturer onboarding application
        And the user has the manufacturer onboarding application json for the new organisation

        When the user send a POST request to "/identity/organisations/organisationId/rejection-files/upload" to upload "<fileNames>" file for "files" with authentication token

        Then the response status code should be 403

        Examples:
            | authEmailId                     | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | fileNames          |
            | triad.test.acc.5+mfr1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | test_doc_file.docx |



