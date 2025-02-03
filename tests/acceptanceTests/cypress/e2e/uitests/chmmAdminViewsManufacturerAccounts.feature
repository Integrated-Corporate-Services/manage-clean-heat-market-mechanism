@RegressionTests
Feature: Verify CHMM Administrator view manufacturer accounts list
    CHMM Administrator view manufacturer accounts list

    Background: Admin user login

        Given the user has authentication token for "triad.test.acc.1+admin2@gmail.com" email id


    @SmokeTest
    Scenario Outline: 1. Verify that an admin user can see a pending manufacturer account

        Given the user submits a manufacturer onboarding application via API with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | false                        |
            | organisationName           | <organisationName>           |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | false                        |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | isResponsibleOfficer       | true                         |
            | creditTransferOptIn        | false                        |
            | creditTransferEmailId      | null                         |

        When the admin user log in to CHMM system with "triad.test.acc.1+admin2@gmail.com" email address
        And the admin user navigates to manufacturer accounts page

        Then the user should see the following data in manufacturer accounts table:
            | name               | status  |
            | <organisationName> | Pending |

        When the user search for "<organisationName>" organisation

        Then the user should see the following data in manufacturer accounts table:
            | name               | status  |
            | <organisationName> | Pending |

        Examples:
            | organisationName        | manufacturerUserEmailId           | manufacturerSROUserEmailId        |
            | QAAutomation11117777Org | QAAutomation_11117777@example.com | QAAutomation_11117777@example.com |


    @SmokeTest
    Scenario Outline: 2. Verify that an admin user can see an active manufacturer account

        Given the admin user creates an organisation with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | false                        |
            | organisationName           | <organisationName>           |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | false                        |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | isResponsibleOfficer       | true                         |
            | creditTransferOptIn        | false                        |
            | creditTransferEmailId      | null                         |

        When the admin user log in to CHMM system with "triad.test.acc.1+admin2@gmail.com" email address
        And the admin user navigates to manufacturer accounts page

        Then the user should see the following data in manufacturer accounts table:
            | name               | status |
            | <organisationName> | Active |

        When the user search for "<organisationName>" organisation

        Then the user should see the following data in manufacturer accounts table:
            | name               | status |
            | <organisationName> | Active |

        Examples:
            | organisationName        | manufacturerUserEmailId           | manufacturerSROUserEmailId        |
            | QAAutomation22228888Org | QAAutomation_22228888@example.com | QAAutomation_22228888@example.com |


    Scenario: 3. Verify that retired manufacturer accounts are not displayed on the first vist of the page

        Given the admin user log in to CHMM system with "triad.test.acc.1+admin2@gmail.com" email address

        When the admin user navigates to manufacturer accounts page

        Then the user should not see retired manufacturer accounts


    Scenario: 4. Verify that retired manufacturer accounts are displayed when show retired is checked

        When the user check show retired manufacturer accounts

        Then the user should see the following data in manufacturer accounts table:
            | name             | status  |
            | A Organisation 1 | Retired |


    Scenario Outline: 5. Verify that admin can search for retired organisation

        When the user check show retired manufacturer accounts
        And the user search for "<orgName>" organisation

        Then the user should see the following data in the first row of manufacturer accounts table:
            | name      | status   |
            | <orgName> | <status> |
        Examples:
            | orgName          | status  |
            | A Organisation 1 | Retired |
