@APITest
Feature: Verify CHMM manufacturer edit user API works as expected
    CHMM admin edit manufacturer onboarding application API test
    # Manufacturer users are not allowed to edit the application once submitted

    Scenario Outline: 1. Verify that an manufacturer user can edit manufacturer user details returns 200

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

        When the admin user send a PUT request to "/identity/organisations/approve" to approve the manufacturer onboarding application with authentication token with following comment:
            """
            <comment>
            """

        Then the response status code should be 204
        And the user send a GET request to "/identity/users/manufacturer/organisationId" with authentication token

        Given the user has authentication token for "<manufacturerUserEmailId>" email id
        When the user send a PUT request to "/identity/users/manufacturer" to edit the organisation details with authentication token
            """
            {
                "name": "<name>",
                "jobTitle": "<jobTitle>",
                "telephoneNumber": "<telephoneNumber>"
            }
            """

            Then the response status code should be 200
        Examples:
            | authEmailId                       | isOnBehalfOfGroup | updateIsOnBehalfOfGroup | isFossilFuelBoilerSeller | updateIsFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | comment                                                        | name       | jobTitle         | telephoneNumber |
            | triad.test.acc.2+admin1@gmail.com | false             | true                    | false                    | true                           | newRandom               | true                 | newRandom                  | true                | newRandom             | API Test Edited comment admin approve manufacturer application | API Edited | Job Title Edited | 1111111111      |





    Scenario Outline: 2. Verify that an manufacturer user can edit manufacturer user details with invalid data returns 400

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

        When the admin user send a PUT request to "/identity/organisations/approve" to approve the manufacturer onboarding application with authentication token with following comment:
            """
            <comment>
            """

        Then the response status code should be 204
        And the user send a GET request to "/identity/users/manufacturer/organisationId" with authentication token

        Given the user has authentication token for "<manufacturerUserEmailId>" email id
        When the user send a PUT request to "/identity/users/manufacturer" to edit the organisation details with authentication token
            """
            {
                "name": "<name>",
                "jobTitle": "<jobTitle>",
                "telephoneNumber": "<telephoneNumber>"
            }
            """

            Then the response status code should be 400
        Examples:
            | authEmailId                       | isOnBehalfOfGroup | updateIsOnBehalfOfGroup | isFossilFuelBoilerSeller | updateIsFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | comment                                                        | name                                                                                                             | jobTitle                                                                                                                                                                                      | telephoneNumber |
            | triad.test.acc.2+admin1@gmail.com | false             | true                    | false                    | true                           | newRandom               | true                 | newRandom                  | true                | newRandom             | API Test Edited comment admin approve manufacturer application | API Edited API Edited API Edited API Edited API Edited API Edited API Edited API Edited API Edited API Edited 12 | Job Title Edited                                                                                                                                                                              | 1111111111      |
            | triad.test.acc.2+admin1@gmail.com | false             | true                    | false                    | true                           | newRandom               | true                 | newRandom                  | true                | newRandom             | API Test Edited comment admin approve manufacturer application | API Edited                                                                                                       | Job Title Edited Job Title Edited Job Title Edited Job Title Edited Job Title Edited Job Title Edited Job Title Edited Job Title Edited Job Title Edited Job Title Edited Job Title Edited 12 | 1111111111      |
            | triad.test.acc.2+admin1@gmail.com | false             | true                    | false                    | true                           | newRandom               | true                 | newRandom                  | true                | newRandom             | API Test Edited comment admin approve manufacturer application | API Edited                                                                                                       | Job Title Edited                                                                                                                                                                              | 1111            |
