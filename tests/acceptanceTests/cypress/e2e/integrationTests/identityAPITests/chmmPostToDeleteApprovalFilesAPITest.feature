@APITest
Feature: Verify CHMM POST to delete approval file to an organisation API works as expected
    CHMM POST to upload a file to rejection files API test
    # NOTE: Organisation with Id = c7e522cc-8d46-4218-8e83-70b9a3d15f38 and email address = triad.test.acc.4+mfr1@gmail.com is present in the DB


    Background: Create a manufacturer account
   # Scenario Outline: 1. Verify that POST delete manufacturer onboarding application by an admin user returns 204

      #  Given the user has authentication token for "<authEmailId>" email id
      #  And the user have a manufacturer onboarding application request body json with the following data:
      #      | key                        | value                        |
      #      | isOnBehalfOfGroup          | <isOnBehalfOfGroup>          |
      #      | organisationName           | newRandom                    |
      #      | companyNumber              | newRandom                    |
      #      | isFossilFuelBoilerSeller   | <isFossilFuelBoilerSeller>   |
      #      | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
      #      | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
      #      | isResponsibleOfficer       | <isResponsibleOfficer>       |
      #      | creditTransferOptIn        | <creditTransferOptIn>        |
      #      | creditTransferEmailId      | <creditTransferEmailId>      |
      #  And the user send a POST request to "/identity/organisations/onboard" with json body and authentication token to submit manufacturer onboarding application
      #  And the user has the manufacturer onboarding application json for the new organisation

     #   When the user send a POST request to "/identity/organisations/organisationId/approval-files/upload" to upload "<fileNames>" file for "files" with authentication token

      #  Then the response status code should be 200
      #  When the user send a POST request to "/identity/organisations/organisationId/approval-files/delete" to delete the uploaded "<fileNames>" file with authentication token
      #  Then the response status code should be 200

 # And the response body array should contain 0 items
       # Examples:
       #     | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | comment                                                 | fileNames          |
       #     | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | API Test comment admin approve manufacturer application | test_doc_file.docx |


    Scenario Outline: 2. Verify that POST delete rejection files by a manufacturer user returns 403

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

       When the user send a POST request to "/identity/organisations/organisationId/approval-files/delete" to upload "<fileNames>" file for "files" with authentication token

        Then the response status code should be 403

        Examples:
            | authEmailId                     | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | fileNames          |
            | triad.test.acc.5+mfr1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | test_doc_file.docx |



    