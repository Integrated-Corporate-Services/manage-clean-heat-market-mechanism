@APITest
Feature: Verify CHMM Post to delete Quarterly boiler sales supproting evidence for an organisation API works as expected
    CHMM Post to delete quarterly boiler sales supproting evidence for an organisation API test


    Scenario Outline: 1. Verify that POST to delete quarterly boiler sales supproting evidence for an organisation by an admin user returns 200

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
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence/delete" to delete the following file with authentication token:
            """
            {
                "fileName": "<deleteFileName>"
            }
            """

        Then the response status code should be 200
        And the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence" to fetch files with authentication token
        And the response status code should be 200
        And the response body array should contain <itemCount> items
        And the response body array should not contain the following items:
            | item             |
            | <deleteFileName> |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | schemeYearQuarterId                  | fileNames                                                                                  | deleteFileName        | itemCount |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx                                                                         | test_doc_file.docx    | 0         |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | file-sample_100kB.doc;test_doc_file.docx;docx.pdf;docx.docx;test_doc_file_with_images.docx | file-sample_100kB.doc | 4         |


    Scenario Outline: 2. Verify that POST to delete quarterly boiler sales supproting evidence for an organisation by a manufacturer user returns 200

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
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence/delete" to delete the following file with authentication token:
            """
            {
                "fileName": "<deleteFileName>"
            }
            """

        Then the response status code should be 200
        And the user send a GET request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence" to fetch files with authentication token
        And the response status code should be 200
        And the response body array should contain <itemCount> items
        And the response body array should not contain the following items:
            | item             |
            | <deleteFileName> |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | schemeYearQuarterId                  | fileNames                                                                                  | deleteFileName        | itemCount |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx                                                                         | test_doc_file.docx    | 0         |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | file-sample_100kB.doc;test_doc_file.docx;docx.pdf;docx.docx;test_doc_file_with_images.docx | file-sample_100kB.doc | 4         |


    Scenario Outline: 3. Verify that POST to delete quarterly boiler sales supporting evidence for an organisation by an admin user with non existing file name returns 400

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
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence/delete" to delete the following file with authentication token:
            """
            {
                "fileName": "<deleteFileName>"
            }
            """

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value                             |
            | status | 400                               |
            | detail | The selected file does not exists |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | schemeYearQuarterId                  | fileNames          | deleteFileName      |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx | test_doc_file1.docx |


    Scenario Outline: 4. Verify that POST to delete quarterly boiler sales supporting evidence for an organisation by a manufacturer user with non existing file name returns 400

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
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence/delete" to delete the following file with authentication token:
            """
            {
                "fileName": "<deleteFileName>"
            }
            """

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value                             |
            | status | 400                               |
            | detail | The selected file does not exists |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | schemeYearQuarterId                  | fileNames          | deleteFileName      |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx | test_doc_file1.docx |


    Scenario Outline: 5. Verify that POST to delete quarterly boiler sales supporting evidence for an organisation by an admin user for already deleted file name returns 400

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
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence/delete" to delete the following file with authentication token:
            """
            {
                "fileName": "<fileNames>"
            }
            """
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence/delete" to delete the following file with authentication token:
            """
            {
                "fileName": "<fileNames>"
            }
            """

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value                             |
            | status | 400                               |
            | detail | The selected file does not exists |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | schemeYearQuarterId                  | fileNames          |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx |


    Scenario Outline: 6. Verify that POST to delete quarterly boiler sales supporting evidence for an organisation by a manufacturer user for already deleted file name returns 400

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
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence/delete" to delete the following file with authentication token:
            """
            {
                "fileName": "<fileNames>"
            }
            """
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence/delete" to delete the following file with authentication token:
            """
            {
                "fileName": "<fileNames>"
            }
            """

        Then the response status code should be 400
        And the response json body should contain the following data:
            | key    | value                             |
            | status | 400                               |
            | detail | The selected file does not exists |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | schemeYearQuarterId                  | fileNames          |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx |


    Scenario Outline: 7. Verify that POST to delete quarterly boiler sales supporting evidence for an organisation by an admin/manufacturer user with invalid organisationId/schemeYearId returns 404

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence/delete" to delete the following file with authentication token:
            """
            {
                "fileName": "<fileNames>"
            }
            """

        Then the response status code should be 404

        Examples:
            | authEmailId                       | organisationId                       | schemeYearId                         | schemeYearQuarterId                  | fileNames          |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx |
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d525e380-4aee-40e9-a7f0-1784d8cb49d  | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx |
            | triad.test.acc.2+admin1@gmail.com | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c95  | test_doc_file.docx |
            # Manufacturer user
            | triad.test.acc.4@gmail.com        | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx |
            | triad.test.acc.4@gmail.com        | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d525e380-4aee-40e9-a7f0-1784d8cb49d  | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx |
            | triad.test.acc.4@gmail.com        | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c95  | test_doc_file.docx |

    
    Scenario Outline: 8. Verify that POST to delete quarterly boiler sales supporting evidence for an organisation without authentication token returns 401

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
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence" to upload "<fileNames>" file for "supportingEvidence" with authentication token

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/quarter/<schemeYearQuarterId>/supporting-evidence/delete" to delete the following file without authentication token:
            """
            {
                "fileName": "<fileNames>"
            }
            """

        Then the response status code should be 401

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | schemeYearQuarterId                  | fileNames          |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | eb186587-f5d7-422e-b90b-c46b9196c957 | test_doc_file.docx |
