@APITest
Feature: Verify CHMM manufacturer users API works as expected
    CHMM view manufacturer users API test
    
    
    Scenario Outline: 1. Verify that GET manufacturer user by same organisation user returns the list of users with 200

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
        
        When the user send a GET request to "/identity/users/manufacturer/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array for the manufacturer users should contain the following json:
            | key      | value         |
            | jobTitle | API Test Job  |
            | name     | API Test user |
            | email    | newRandom     |
            | status   | Active        |
            
        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | itemCount |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 1         |

   
    Scenario Outline: 2. Verify that GET manufacturer user by admin user returns the list of users with 200

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
        
        When the user send a GET request to "/identity/users/manufacturer/organisationId" with authentication token

        Then the response status code should be 200
        And the response body array for the manufacturer users should contain the following json:
            | key      | value         |
            | jobTitle | API Test Job  |
            | name     | API Test user |
            | email    | newRandom     |
            | status   | Active        |
            
        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | itemCount |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 1         |


    Scenario: 3. Verify that GET manufacturer user by a user without organisation id returns 405

        Given the user has authentication token for "triad.test.acc.5+mfr1@gmail.com" email id

        When the user send a GET request to "/identity/users/manufacturer/" with authentication token

        Then the response status code should be 405


    Scenario: 4. Verify that GET manufacturer user by an admin user without organisation id returns 405 

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a GET request to "/identity/users/manufacturer/" with authentication token

        Then the response status code should be 405


    Scenario Outline: 5. Verify that GET manufacturer user by a user from different organisation returns 403

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
        And the user has authentication token for "triad.test.acc.5+mfr1@gmail.com" manufacturer user email id
        
        When the user send a GET request to "/identity/users/manufacturer/organisationId" with authentication token

        Then the response status code should be 403
        
        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | itemCount |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | 1         |


    Scenario: 6. Verify that GET manufacturer user by a user with invalid organisation id returns 403

        Given the user has authentication token for "triad.test.acc.5+mfr1@gmail.com" email id

        When the user send a GET request to "/identity/users/manufacturer/1fc2729a-8022-4894-bf10-1f70f8f73722" with authentication token

        Then the response status code should be 403


    Scenario: 7. Verify that GET manufacturer user by an admin user with invalid organisation id returns 404

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a GET request to "/identity/users/manufacturer/1fc2729a-8022-4894-bf10-1f70f8f7372" with authentication token

        Then the response status code should be 404
