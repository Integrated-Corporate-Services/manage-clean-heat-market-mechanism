@APITest
Feature: Verify CHMM Admin approves manufacturer onboarding application API works as expected
    CHMM admin approve manufacturer onboarding application API test


    Scenario Outline: 1. Verify that PUT approve manufacturer onboarding application by an admin user returns 204

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

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | comment                                                 |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | API Test comment admin approve manufacturer application |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  |                                                         |


    Scenario Outline: 2. Verify that an admin user can PUT approve manufacturer onboarding application that is submitted by a manufacturer user returns 204

        Given the user has authentication token for "<mfrEmailId>" email id
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
        And the user has authentication token for "<adminEmailId>" email id
        And the user has authentication token for "<adminEmailId>" email id
        And the user has the manufacturer onboarding application json for the new organisation

        When the admin user send a PUT request to "/identity/organisations/approve" to approve the manufacturer onboarding application with authentication token with following comment:
            """
            API Test comment admin approve manufacturer application
            """

        Then the response status code should be 204

        Examples:
            | mfrEmailId                      | adminEmailId                      | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId |
            | triad.test.acc.5+mfr1@gmail.com | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  |


    Scenario Outline: 3. Verify that PUT approve manufacturer onboarding application by a manufacturer user returns 403

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
            API Test comment admin approve manufacturer application
            """

        Then the response status code should be 403
        
        Examples:
            | authEmailId                     | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId |
            | triad.test.acc.5+mfr1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  |


    Scenario Outline: 4. Verify that PUT approve manufacturer onboarding application without authentication token returns 401

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

        When the admin user send a PUT request to "/identity/organisations/approve" to approve the manufacturer onboarding application without authentication token with following comment:
            """
            API Test comment admin approve manufacturer application
            """
            
        Then the response status code should be 401
        
        Examples:
            | authEmailId                     | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId |
            | triad.test.acc.5+mfr1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  |
