@APITest
Feature: Verify CHMM Admin views manufacturer organisation API works as expected
    CHMM Admin view manufacturer organisations API test

# NOTE: User triad.test.acc.2+admin10@gmail.com is set to inactive. Later add steps to deactivate admin user
    
    Scenario Outline: 1. Verify that GET manufacturer organisation by an admin user returns the list of organisations with 200

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

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

        When the user send a GET request to "/identity/organisations" with authentication token

        Then the response status code should be 200
        And the response body array for organisations should contain the following json:
            """
            {
                "id": "newRandom",
                "name": "newRandom",
                "status": "Active"
            }
            """            

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | oil     | gas     |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 101     | 102     |
        

    Scenario: 2. Verify that GET manufacturer organisations by a manufacturer user returns the list of organisations with 403

        Given the user has authentication token for "triad.test.acc.5+mfr1@gmail.com" email id

        When the user send a GET request to "/identity/organisations" with authentication token

        Then the response status code should be 403


    Scenario: 3. Verify that GET manufacturer organisations without authentication token returns 401

        When the user send a GET request to "/identity/organisations"

        Then the response status code should be 401
    

    #  User triad.test.acc.2+admin10@gmail.com is set to inactive. Later add steps to deactivate admin user
    Scenario: 4. Verify that GET manufacturer organisation by an inactive admin user returns 403

        Given the user has authentication token for "triad.test.acc.2+admin10@gmail.com" email id

        When the user send a GET request to "/identity/organisations" with authentication token

        Then the response status code should be 403


    # The pending Admin status will be changed to Active as soon as tey intract with the system
    # Need add steps to invite an admin for this scenario. Commenting out this for now.
    # Scenario: 4. Verify that GET manufacturer organisation by a pending admin user returns 200
