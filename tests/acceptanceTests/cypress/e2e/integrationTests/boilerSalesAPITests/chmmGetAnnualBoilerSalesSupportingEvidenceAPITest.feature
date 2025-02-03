@APITest
Feature: Verify CHMM Get Annual boiler sales supporting evidence for an organisation API works as expected
    CHMM Get annual boiler sales supporting evidence for an organisation API test


    Scenario Outline: 1. Verify that GET annual boiler sales supporting evidence for an organisation by an admin user returns 200

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

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to fetch files with authentication token

        Then the response status code should be 200
        And the response body array should contain <itemCount> items
        And the response body array should contain the following items:
            | item        |
            | <fileNames> |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames                                                                                  | itemCount |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx                                                                         | 1         |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file-sample_100kB.doc;test_doc_file.docx;docx.pdf;docx.docx;test_doc_file_with_images.docx | 5         |


    Scenario Outline: 2. Verify that GET annual boiler sales supporting evidence for an organisation by a manufacturer user returns 200

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

        When the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/supporting-evidence" to fetch files with authentication token

        Then the response status code should be 200
        And the response body array should contain <itemCount> items
        And the response body array should contain the following items:
            | item        |
            | <fileNames> |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | fileNames                                                                                  | itemCount |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | test_doc_file.docx                                                                         | 1         |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | file-sample_100kB.doc;test_doc_file.docx;docx.pdf;docx.docx;test_doc_file_with_images.docx | 5         |
     

    Scenario Outline: 3. Verify that GET annual boiler sales supporting evidence for an organisation by an admin/manufacturer user with invalid organisation id/schemeYearId returns 404

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a GET request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/annual/supporting-evidence" to fetch files with authentication token

        Then the response status code should be 404

        Examples:
            | authEmailId                       | organisationId                       | schemeYearId                         |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  | 209b7d48-2307-4a58-9e87-f3e3e7be6b5e |
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | 209b7d48-2307-4a58-9e87-f3e3e7be6b5  |
            # Manufacturer user
            | triad.test.acc.6+mfr10@gmail.com  | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  | 209b7d48-2307-4a58-9e87-f3e3e7be6b5e |
            | triad.test.acc.6+mfr10@gmail.com  | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | 209b7d48-2307-4a58-9e87-f3e3e7be6b5  |


    Scenario: 4. Verify that GET annual boiler sales supporting evidence for an organisation without authentication token returns 401

        When the user send a GET request to "/boilersales/organisation/c9ea5101-e45b-4f16-ac8e-e48c6d83ec81/year/209b7d48-2307-4a58-9e87-f3e3e7be6b5e/annual/supporting-evidence" to fetch files without authentication token

        Then the response status code should be 401
