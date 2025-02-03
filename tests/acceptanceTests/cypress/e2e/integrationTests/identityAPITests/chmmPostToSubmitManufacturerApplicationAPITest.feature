@APITest
Feature: Verify CHMM manufacturer users submit onboarding application API works as expected
    CHMM manufacturer users submit onboarding application API test

    
    # Verify that POST manufacturer onboarding application details by an admin user and manufacturer user returns 200
    Scenario Outline: 1. Verify that POST manufacturer onboarding application by an admin/manufacturer user returns the list of users with 200

        Given the user has authentication token for "<authEmailId>" email id
        And the user have a manufacturer onboarding application request body json with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | <isOnBehalfOfGroup>          |
            | organisationName           | newRandom                    |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | <isFossilFuelBoilerSeller>   |
            | isNonSchemeParticipant     | true                         |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | isResponsibleOfficer       | <isResponsibleOfficer>       |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | creditTransferOptIn        | <creditTransferOptIn>        |
            | creditTransferEmailId      | <creditTransferEmailId>      |
            | legalAddressType           | <legalAddressType>           |

        When the user send a POST request to "/identity/organisations/onboard" with json body and authentication token to submit manufacturer onboarding application

        Then the response status code should be 201
        And the response body should contain user id in the form of uuid

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | legalAddressType                |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | Use Registered Office           |
            | triad.test.acc.2+admin1@gmail.com | true              | false                    | newRandom               | true                 | newRandom                  | false               | null                  | Use Specified Address           |
            | triad.test.acc.2+admin1@gmail.com | true              | true                     | newRandom               | false                | newRandom                  | true                | newrandom             | No Legal Correspondence Address |
            # manufacturer user
            | triad.test.acc.5+mfr1@gmail.com   | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | Use Registered Office           |
            | triad.test.acc.5+mfr1@gmail.com   | true              | false                    | newRandom               | true                 | newRandom                  | false               | null                  | Use Specified Address           |
            | triad.test.acc.5+mfr1@gmail.com   | true              | true                     | newRandom               | false                | newRandom                  | true                | newrandom             | No Legal Correspondence Address |


    Scenario Outline: 2. Verify that POST manufacturer onboarding application without authentication token return 401

        Given the user have a manufacturer onboarding application request body json with the following data:
            | key                      | value                      |
            | isOnBehalfOfGroup        | <isOnBehalfOfGroup>        |
            | organisationName         | newRandom                  |
            | companyNumber            | newRandom                  |
            | isFossilFuelBoilerSeller | <isFossilFuelBoilerSeller> |
            | manufacturerUserEmailId  | <manufacturerUserEmailId>  |
            | isResponsibleOfficer     | <isResponsibleOfficer>     |
            | creditTransferOptIn      | <creditTransferOptIn>      |
            | creditTransferEmailId    | <creditTransferEmailId>    |

        When the user send a POST request to "/identity/organisations/onboard" with json body

        Then the response status code should be 401

        Examples:
            | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | creditTransferOptIn | creditTransferEmailId |
            | false             | false                    | newRandom               | true                 | false               | null                  |


    # Verify that POST duplicate manufacturer onboarding application by an admin user and a manufacturer user returns 400
    Scenario Outline: 3. Verify that POST duplicate manufacturer onboarding application by an admin/manufacturer user returns 400

        Given the user has authentication token for "<authEmailId>" email id
        And the user have a manufacturer onboarding application request body json with the following data:
            | key                      | value                      |
            | isOnBehalfOfGroup        | <isOnBehalfOfGroup>        |
            | organisationName         | newRandom                  |
            | companyNumber            | newRandom                  |
            | isFossilFuelBoilerSeller | <isFossilFuelBoilerSeller> |
            | manufacturerUserEmailId  | <manufacturerUserEmailId>  |
            | isResponsibleOfficer     | <isResponsibleOfficer>     |
            | creditTransferOptIn      | <creditTransferOptIn>      |
            | creditTransferEmailId    | <creditTransferEmailId>    |
        And the user send a POST request to "/identity/organisations/onboard" with json body and authentication token to submit manufacturer onboarding application
        # And the user send a GET request to "/identity/organisations/OrganisationId" with authentication token
        And the user has the manufacturer onboarding application json for the new organisation

        When the user send a POST request to "/identity/organisations/onboard" with json body and authentication token to submit manufacturer onboarding application

        Then the response status code should be 400

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | creditTransferOptIn | creditTransferEmailId |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | false               | null                  |
            # manufacturer user
            | triad.test.acc.5+mfr1@gmail.com   | false             | false                    | newRandom               | true                 | false               | null                  |


    # Verify POST manufacturer onboarding application by an admin user and manufacturer user without user email id returns 400
    Scenario Outline: 4. Verify that POST manufacturer onboarding application by an admin/manufacturer user without email id returns 400

        Given the user has authentication token for "<authEmailId>" email id
        And the user have a manufacturer onboarding application request body json with the following data:
            | key                      | value                      |
            | isOnBehalfOfGroup        | <isOnBehalfOfGroup>        |
            | organisationName         | newRandom                  |
            | companyNumber            | newRandom                  |
            | isFossilFuelBoilerSeller | <isFossilFuelBoilerSeller> |
            | manufacturerUserEmailId  | <manufacturerUserEmailId>  |
            | isResponsibleOfficer     | <isResponsibleOfficer>     |
            | creditTransferOptIn      | <creditTransferOptIn>      |
            | creditTransferEmailId    | <creditTransferEmailId>    |

        When the user send a POST request to "/identity/organisations/onboard" with json body and authentication token to submit manufacturer onboarding application

        Then the response status code should be 400

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | creditTransferOptIn | creditTransferEmailId |
            # Admin user
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | null                    | true                 | false               | null                  |
            # manufacturer user
            | triad.test.acc.5+mfr1@gmail.com   | false             | false                    | null                    | true                 | false               | null                  |


    # Verify POST manufacturer onboarding application with invalid data
    Scenario Outline: 5. Verify that POST manufacturer onboarding application by an admin/manufacturer user with invalid data returns 400

        Given the user has authentication token for "<authEmailId>" email id
        And the user have a manufacturer onboarding application request body json with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | <isOnBehalfOfGroup>          |
            | organisationName           | newRandom                    |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | <isFossilFuelBoilerSeller>   |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | isResponsibleOfficer       | <isResponsibleOfficer>       |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | creditTransferOptIn        | <creditTransferOptIn>        |
            | creditTransferEmailId      | <creditTransferEmailId>      |

        When the user send a POST request to "/identity/organisations/onboard" with json body and authentication token to submit manufacturer onboarding application

        Then the response status code should be 400

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId |
            # Admin user with invalid isOnBehalfOfGroup field
            | triad.test.acc.2+admin1@gmail.com | ""                | false                    | newrandom               | true                 | newrandom                  | false               | newRandom             |
            | triad.test.acc.2+admin1@gmail.com | false             | ""                       | newrandom               | true                 | newrandom                  | false               | newRandom             |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newrandom               | ""                   | newrandom                  | false               | newRandom             |
            # manufacturer user
            | triad.test.acc.5+mfr1@gmail.com   | ""                | false                    | newrandom               | true                 | newrandom                  | false               | newRandom             |
            | triad.test.acc.5+mfr1@gmail.com   | false             | ""                       | newrandom               | true                 | newrandom                  | false               | newRandom             |
            | triad.test.acc.5+mfr1@gmail.com   | false             | false                    | newrandom               | ""                   | newrandom                  | false               | newRandom             |
            | triad.test.acc.5+mfr1@gmail.com   | false             | false                    | @example.com            | false                | newrandom                  | false               | newRandom             |
            | triad.test.acc.5+mfr1@gmail.com   | false             | false                    | newrandom               | false                | @example.com               | false               | newRandom             |
