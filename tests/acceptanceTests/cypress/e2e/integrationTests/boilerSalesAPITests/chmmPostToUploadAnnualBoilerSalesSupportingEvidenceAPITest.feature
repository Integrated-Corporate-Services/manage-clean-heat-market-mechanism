@APITest
Feature: Verify CHMM Post to upload Annual boiler sales Supporting Evidence for an organisation API works as expected
    CHMM Post to upload annual boiler sales Supporting Evidence for an organisation API test


    Scenario Outline: 1. Verify that POST to upload annual boiler sales supporting evidence for an organisation by an admin user returns 200

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

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        Then the response status code should be 200

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames                                                                                     |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx                                                                            |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file-sample_100kB.doc;test_doc_file.docx;docx.pdf;docx.docx;test_doc_file_with_images.docx    |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | csv_test_file.csv;pptx_test_file.pptx;ppt_test_file.ppt;xls_test_file.xls;xlsx_test_file.xlsx |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file_example_JPG_500kB.jpg;file_example_PNG_500kB.png;sample_640×426.bmp                      |


    Scenario Outline: 2. Verify that POST to upload annual boiler sales supporting evidence for an organisation by a manufacturer user returns 200

        Given the user has authentication token for "<adminEmailId>" email id
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
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        Then the response status code should be 200

        Examples:
            | adminEmailId                      | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames                                                                                     |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx                                                                            |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file-sample_100kB.doc;test_doc_file.docx;docx.pdf;docx.docx;test_doc_file_with_images.docx    |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | csv_test_file.csv;pptx_test_file.pptx;ppt_test_file.ppt;xls_test_file.xls;xlsx_test_file.xlsx |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file_example_JPG_500kB.jpg;file_example_PNG_500kB.png;sample_640×426.bmp                      |


    Scenario Outline: 3. Verify that POST annual boiler sales supporting evidence for an organisation by an admin user with un-supported file types returns 400

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

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        Then the response status code should be 400

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames                             |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | example.json                          |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_text_file.txt                    |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | visual_studio_code.exe                |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx;test_doc_file.docx |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eicar.txt                             |


    Scenario Outline: 4. Verify that POST annual boiler sales supporting evidence for an organisation by a manufacturer user with un-supported file types returns 400

        Given the user has authentication token for "<adminEmailId>" email id
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
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        Then the response status code should be 400

        Examples:
            | adminEmailId                      | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames                             |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | example.json                          |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_text_file.txt                    |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | visual_studio_code.exe                |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx;test_doc_file.docx |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eicar.txt                             |


    Scenario Outline: 5. Verify that POST annual boiler sales supporting evidence for an organisation by an admin user with large and corrupt files returns 413

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

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        Then the response status code should be 413

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames              |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | very_large_file.docx   |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | corrupt_mp3_file.mp3   |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | corrupt_docx_file.docx |


    Scenario Outline: 6. Verify that POST annual boiler sales supporting evidence for an organisation by a manufacturer user with large and corrupt files returns 413

        Given the user has authentication token for "<adminEmailId>" email id
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
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        Then the response status code should be 413

        Examples:
            | adminEmailId                      | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames              |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | very_large_file.docx   |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | corrupt_mp3_file.mp3   |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | corrupt_docx_file.docx |


    Scenario Outline: 7. Verify that POST annual boiler sales supporting evidence for an organisation by an admin/manufacturer user with invalid organisationId/schemeYearQuarterId returns 404

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        Then the response status code should be 404

        Examples:
            | authEmailId                       | organisationId                       | schemeYearId                         | fileNames          |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx |
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d525e380-4aee-40e9-a7f0-1784d8cb49d  | test_doc_file.docx |
            # Manufacturer user
            | triad.test.acc.4@gmail.com        | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx |
            | triad.test.acc.4@gmail.com        | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d525e380-4aee-40e9-a7f0-1784d8cb49d  | test_doc_file.docx |


    Scenario Outline: 8. Verify that POST annual boiler sales supporting evidence for an organisation by an admin user with same schemeYearId/schemeYearQuarterId returns 400

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

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        Then the response status code should be 400

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames          |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx |


    Scenario Outline: 9. Verify that POST annual boiler sales supporting evidence for an organisation by a manufacturer user with same schemeYearId/schemeYearQuarterId returns 400

        Given the user has authentication token for "<adminEmailId>" email id
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
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        Then the response status code should be 400

        Examples:
            | adminEmailId                      | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames          |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx |


    Scenario Outline: 10. Verify that POST annual boiler sales supporting evidence for an organisation by an admin/manufacturer user without authentication token returns 401

        When the user send a POST request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/annual/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" without authentication token

        Then the response status code should be 401

        Examples:
            | authEmailId                       | organisationId                       | schemeYearId                         | fileNames          |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx |
            # Manufacturer user
            | triad.test.acc.4@gmail.com        | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx |
