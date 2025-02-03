@APITest
Feature: Verify CHMM POST to clear files for a new note for an organisation API works as expected
    CHMM POST to clear files for a new note for an organisation API test
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


    Scenario Outline: 1. Verify that POST to clear files for a new note by an admin user returns 204

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token

        When the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/clear-files" with authentication token

        Then the response status code should be 204
        And the user send a GET request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file-names" with authentication token
        And the response body array should contain <itemCount> items

        Examples:
            | authEmailId                       | schemeYearId                         | fileNames             | itemCount |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx    | 0         |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file-sample_100kB.doc | 0         |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | pptx_test_file.pptx   | 0         |


    Scenario Outline: 2. Verify that POST to clear files for a new note by a manufacturer user returns 403

        Given the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/file" to upload "<fileNames>" file for "files" with authentication token
        And the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/organisationId/year/<schemeYearId>/new-note/clear-files" with authentication token

        Then the response status code should be 403

        Examples:
            | authEmailId                     | schemeYearId                         | fileNames          | itemCount |
            | triad.test.acc.4+mfr1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx | 0         |


    Scenario Outline: 3. Verify that POST to clear files with invalid organisationId/schemeYearId returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/<organisationId>/year/<schemeYearId>/new-note/clear-files" with authentication token

        Then the response status code should be 400

        Examples:
            | authEmailId                       | schemeYearId                         | organisationId                       |
            # Invalid organisationId
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            # organisation Id as schemeYearId
            | triad.test.acc.2+admin1@gmail.com | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            # non-existing schemeYearId
            | triad.test.acc.2+admin1@gmail.com | a1ab57ad-b2a5-4487-ad46-e206dd0345d8 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |


    Scenario: 4. Verify that POST to clear files without authentication token returns 401

        When the user send a POST request to "/notes/manufacturer/organisationId/year/d525e380-4aee-40e9-a7f0-1784d8cb49d9/new-note/clear-files" without authentication token

        Then the response status code should be 401
