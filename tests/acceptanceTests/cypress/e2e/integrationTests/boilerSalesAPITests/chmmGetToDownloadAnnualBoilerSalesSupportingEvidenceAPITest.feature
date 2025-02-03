@APITest
Feature: Verify CHMM Get to Download Annual boiler sales supproting evidence for an organisation API works as expected
    CHMM Get to download annual boiler sales supproting evidence for an organisation API test


    Scenario Outline: 1. Verify that GET to download annual boiler sales supproting evidence for an organisation by an admin user returns 200

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
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence/download?fileName=<fileNames>" to download files with authentication token

        Then the response status code should be 200

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames                  |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |  test_doc_file.docx         |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file_example_JPG_500kB.jpg |


    Scenario Outline: 2. Verify that GET to download annual boiler sales supproting evidence for an organisation by a manufacturer user returns 200

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
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence/download?fileName=<fileNames>" to download files with authentication token

        Then the response status code should be 200

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames                  |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |  test_doc_file.docx         |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file_example_JPG_500kB.jpg |


    Scenario Outline: 3. Verify that GET to download annual boiler sales supproting evidence for an organisation by an admin/manufacturer user with invalid organisationId/schemeYearId returns 404

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/annual/supporting-evidence/download?fileName=<fileNames>" to download files with authentication token

        Then the response status code should be 404

        Examples:
            | authEmailId                       | organisationId                       | schemeYearId                         | fileNames             |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  | d5105bda-d3af-45c0-9478-816e02096e9c | test_doc_file.docx    |
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d5105bda-d3af-45c0-9478-816e02096e9  | file-sample_100kB.doc |
            # Manufacuter user
            | triad.test.acc.6+mfr10@gmail.com  | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  | d5105bda-d3af-45c0-9478-816e02096e9c | test_doc_file.docx    |
            | triad.test.acc.6+mfr10@gmail.com  | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d5105bda-d3af-45c0-9478-816e02096e9  | file-sample_100kB.doc |


    Scenario Outline: 4. Verify that GET to download annual boiler sales supproting evidence for an organisation by an admin user with non existing file returns 400

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
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence/download?fileName=<downloadFileNames>" to download files with authentication token

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value    |
            | status | 400      |
            | detail | NotFound |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames                  | downloadFileNames           |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file_example_JPG_500kB.jpg | file_example_JPG_500kB1.jpg |


    Scenario Outline: 5. Verify that GET to download annual boiler sales supproting evidence for an organisation by a manufacturer user with non existing file returns 400

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
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence/download?fileName=<downloadFileNames>" to download files with authentication token

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value    |
            | status | 400      |
            | detail | NotFound |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames                  | downloadFileNames           |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file_example_JPG_500kB.jpg | file_example_JPG_500kB1.jpg |


    Scenario Outline: 6. Verify that GET to download annual boiler sales supproting evidence for an organisation without authentication token returns 401

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/annual/supporting-evidence/download?fileName=<fileNames>" to download files without authentication token

        Then the response status code should be 401

        Examples:
            | authEmailId                       | organisationId                       | schemeYearId                         | fileNames          |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d5105bda-d3af-45c0-9478-816e02096e9c | test_doc_file.docx |
            # Manufacuter user
            | triad.test.acc.6+mfr10@gmail.com  | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d5105bda-d3af-45c0-9478-816e02096e9c | test_doc_file.docx |
