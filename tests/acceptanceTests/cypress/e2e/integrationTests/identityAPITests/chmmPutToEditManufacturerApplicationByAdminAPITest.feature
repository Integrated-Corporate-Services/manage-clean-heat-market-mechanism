@APITest
Feature: Verify CHMM Admin edit manufacturer onboarding application API works as expected
    CHMM admin edit manufacturer onboarding application API test
    # Manufacturer users are not allowed to edit the application once submitted


    Scenario Outline: 1. Verify that an admin user can edit manufacturer user details for manufacturer onboarding application returns 204

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
        And the user send a GET request to "/identity/organisations/organisationId" with authentication token
        And the user edit the manufacturer onboarding application json with the following data:
            | key                                  | value                            |
            | users[0].name                        | API Test user Edited             |
            | users[0].telephoneNumber             | 234234234                        |
            | users[0].jobTitle                    | API Test Job Edited              |
            | addresses[0].lineOne                 | YD Address1 Edited               |
            | addresses[0].lineTwo                 | YD Address2 Edited               |
            | addresses[0].city                    | YD Town1 Edited                  |
            | addresses[0].county                  | YD County1 Edited                |
            | addresses[0].postcode                | YD11 3ED                         |
            | addresses[1].lineOne                 | LC Address1 Edited               |
            | addresses[1].lineTwo                 | LC Address2 Edited               |
            | addresses[1].city                    | LC Town1 Edited                  |
            | addresses[1].county                  | LC County1 Edited                |
            | addresses[1].postcode                | LC11 3ED                         |
            | heatPumpBrands[0]                    | Brand1 Edited                    |
            | heatPumpBrands[1]                    | Brand2 Edited                    |
            | isOnBehalfOfGroup                    | <updateIsOnBehalfOfGroup>        |
            | isFossilFuelBoilerSeller             | <updateIsFossilFuelBoilerSeller> |
            | creditContactDetails.name            | CT API Test Edited               |
            | creditContactDetails.telephoneNumber | 234234234                        |
            | creditContactDetails.email           | CTAPITest@example.com            |

        When the user send a PUT request to "/identity/organisations/edit" to edit the organisation details with authentication token with following comment:
            """
            <comment>
            """

        Then the response status code should be 204
        And the user send a GET request to "/identity/organisations/organisationId" with authentication token
        And the response json body should contain the following data:
            | key                                  | value                            |
            | users[0].name                        | API Test user Edited             |
            | users[0].telephoneNumber             | 234234234                        |
            | users[0].jobTitle                    | API Test Job Edited              |
            | addresses[0].lineOne                 | YD Address1 Edited               |
            | addresses[0].lineTwo                 | YD Address2 Edited               |
            | addresses[0].city                    | YD Town1 Edited                  |
            | addresses[0].county                  | YD County1 Edited                |
            | addresses[0].postcode                | YD11 3ED                         |
            | addresses[1].lineOne                 | LC Address1 Edited               |
            | addresses[1].lineTwo                 | LC Address2 Edited               |
            | addresses[1].city                    | LC Town1 Edited                  |
            | addresses[1].county                  | LC County1 Edited                |
            | addresses[1].postcode                | LC11 3ED                         |
            | heatPumpBrands[0]                    | Brand1 Edited                    |
            | heatPumpBrands[1]                    | Brand2 Edited                    |
            | isOnBehalfOfGroup                    | <updateIsOnBehalfOfGroup>        |
            | isFossilFuelBoilerSeller             | <updateIsFossilFuelBoilerSeller> |
            | creditContactDetails.name            | CT API Test Edited               |
            | creditContactDetails.telephoneNumber | 234234234                        |
            | creditContactDetails.email           | CTAPITest@example.com            |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | updateIsOnBehalfOfGroup | isFossilFuelBoilerSeller | updateIsFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | comment                                                        |
            | triad.test.acc.2+admin1@gmail.com | false             | true                    | false                    | true                           | newRandom               | true                 | newRandom                  | true                | newRandom             | API Test Edited comment admin approve manufacturer application |
            | triad.test.acc.2+admin1@gmail.com | true              | false                   | true                     | false                          | newRandom               | true                 | newRandom                  | true                | newRandom             | API Test Edited comment admin approve manufacturer application |
            | triad.test.acc.2+admin1@gmail.com | true              | false                   | true                     | false                          | newRandom               | false                | newRandom                  | true                | newRandom             | API Test Edited comment admin approve manufacturer application |


    Scenario Outline: 2. Verify that an admin user can edit manufacturer user details and approve manufacturer onboarding application returns 204

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
        And the user edit the manufacturer onboarding application json with the following data:
            | key                                  | value                            |
            | users[0].name                        | API Test user Edited             |
            | users[0].telephoneNumber             | 234234234                        |
            | users[0].jobTitle                    | API Test Job Edited              |
            | addresses[0].lineOne                 | YD Address1 Edited               |
            | addresses[0].lineTwo                 | YD Address2 Edited               |
            | addresses[0].city                    | YD Town1 Edited                  |
            | addresses[0].county                  | YD County1 Edited                |
            | addresses[0].postcode                | YD11 3ED                         |
            | addresses[1].lineOne                 | LC Address1 Edited               |
            | addresses[1].lineTwo                 | LC Address2 Edited               |
            | addresses[1].city                    | LC Town1 Edited                  |
            | addresses[1].county                  | LC County1 Edited                |
            | addresses[1].postcode                | LC11 3ED                         |
            | heatPumpBrands[0]                    | Brand1 Edited                    |
            | heatPumpBrands[1]                    | Brand2 Edited                    |
            | isOnBehalfOfGroup                    | <updateIsOnBehalfOfGroup>        |
            | isFossilFuelBoilerSeller             | <updateIsFossilFuelBoilerSeller> |
            | creditContactDetails.name            | CT API Test Edited               |
            | creditContactDetails.telephoneNumber | 234234234                        |

        When the user send a PUT request to "/identity/organisations/edit" to edit the organisation details with authentication token with following comment:
            """
            <comment>
            """
        And the user send a GET request to "/identity/organisations/organisationId" with authentication token
        And the admin user send a PUT request to "/identity/organisations/approve" to approve the manufacturer onboarding application with authentication token with following comment:
            """
            <comment>
            """

        Then the response status code should be 204
        And the user send a GET request to "/identity/organisations/organisationId" with authentication token
        And the response json body should contain the following data:
            | key                                  | value                            |
            | users[0].name                        | API Test user Edited             |
            | users[0].telephoneNumber             | 234234234                        |
            | users[0].jobTitle                    | API Test Job Edited              |
            | addresses[0].lineOne                 | YD Address1 Edited               |
            | addresses[0].lineTwo                 | YD Address2 Edited               |
            | addresses[0].city                    | YD Town1 Edited                  |
            | addresses[0].county                  | YD County1 Edited                |
            | addresses[0].postcode                | YD11 3ED                         |
            | addresses[1].lineOne                 | LC Address1 Edited               |
            | addresses[1].lineTwo                 | LC Address2 Edited               |
            | addresses[1].city                    | LC Town1 Edited                  |
            | addresses[1].county                  | LC County1 Edited                |
            | addresses[1].postcode                | LC11 3ED                         |
            | heatPumpBrands[0]                    | Brand1 Edited                    |
            | heatPumpBrands[1]                    | Brand2 Edited                    |
            | isOnBehalfOfGroup                    | <updateIsOnBehalfOfGroup>        |
            | isFossilFuelBoilerSeller             | <updateIsFossilFuelBoilerSeller> |
            | creditContactDetails.name            | CT API Test Edited               |
            | creditContactDetails.telephoneNumber | 234234234                        |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | updateIsOnBehalfOfGroup | isFossilFuelBoilerSeller | updateIsFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | comment                                                        |
            | triad.test.acc.2+admin1@gmail.com | false             | true                    | false                    | true                           | newRandom               | true                 | newRandom                  | false               | null                  | API Test Edited comment admin approve manufacturer application |
            | triad.test.acc.2+admin1@gmail.com | true              | false                   | true                     | false                          | newRandom               | true                 | newRandom                  | false               | null                  | API Test Edited comment admin approve manufacturer application |


    Scenario Outline: 3. Verify that an admin user editing manufacturer user details to empty fields for manufacturer onboarding application returns 400

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
        And the user edit the manufacturer onboarding application json with the following data:
            | key                      | value |
            | users[0].name            |       |
            | users[0].telephoneNumber |       |
            | users[0].jobTitle        |       |
            | addresses[0].lineOne     |       |
            | addresses[0].lineTwo     |       |
            | addresses[0].city        |       |
            | addresses[0].county      |       |
            | addresses[0].postcode    |       |
            | addresses[1].lineOne     |       |
            | addresses[1].lineTwo     |       |
            | addresses[1].city        |       |
            | addresses[1].county      |       |
            | addresses[1].postcode    |       |
            | heatPumpBrands[0]        |       |
            | heatPumpBrands[1]        |       |

        When the user send a PUT request to "/identity/organisations/edit" to edit the organisation details with authentication token with following comment:
            """
            <comment>
            """

        Then the response status code should be 400

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | comment                                                 |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | API Test comment admin approve manufacturer application |


    Scenario Outline: 4. Verify that an admin user try to edit manufacturer user email id in manufacturer onboarding application returns 400

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
        And the user edit the manufacturer onboarding application json with the following data:
            | key   | value   |
            | <key> | <value> |

        When the user send a PUT request to "/identity/organisations/edit" to edit the organisation details with authentication token with following comment:
            """
            API Test comment admin approve manufacturer application
            """

        Then the response status code should be <status>

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | key                        | value               | status |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | false                | newRandom                  | true                | newRandom             | users[0].email             | @example.com        | 400    |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | false                | newRandom                  | true                | newRandom             | creditContactDetails.email | @example.com        | 400    |
            # https://triadgroupplc.atlassian.net/browse/CHMMBETA-641 it is currently returning 500 for the below data. Need to update the status code once the bug is fixed
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | false                | newRandom                  | true                | newRandom             | users[0].email             | APITest@example.com | 500    |


    Scenario Outline: 5. Verify that PUT to edit manufacturer onboarding application by a manufacturer user returns 403

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
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
        And the user edit the manufacturer onboarding application json with the following data:
            | key                      | value |
            | users[0].name            |       |
            | users[0].telephoneNumber |       |
            | users[0].jobTitle        |       |
            | addresses[0].lineOne     |       |
            | addresses[0].lineTwo     |       |
            | addresses[0].city        |       |
            | addresses[0].county      |       |
            | addresses[0].postcode    |       |
            | addresses[1].lineOne     |       |
            | addresses[1].lineTwo     |       |
            | addresses[1].city        |       |
            | addresses[1].county      |       |
            | addresses[1].postcode    |       |
            | heatPumpBrands[0]        |       |
            | heatPumpBrands[1]        |       |
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a PUT request to "/identity/organisations/edit" to edit the organisation details with authentication token with following comment:
            """
            <comment>
            """

        Then the response status code should be 403

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | comment                                                 |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | API Test comment admin approve manufacturer application |
