@RegressionTest
Feature: Verify CHMM manufacturer view manage your page displayed as expected
    CHMM manufacturer view manage your page test

    
    @SmokeTest
    Scenario Outline: 1. Verify that a CHMM manufacturer user can view user details in view manage your page

        Given the user has authentication token for "triad.test.acc.1+admin5@gmail.com" email id
        And the admin user creates an organisation with the following data:
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

        When the manufacturer user log in to chmm view manage user page with "<manufacturerUserEmailId>" email address
        Then the user should see "Manage users" message on the page
        And the user should see the following user with "<manufacturerUserEmailId>" email has details in the manage users table:
            | text          |
            | API Test user |
            | API Test Job  |
            | Active        |

        Examples:
            | organisationName         | manufacturerUserEmailId         | manufacturerSROUserEmailId      |
            | triad.test.acc.4+mfr3Org | triad.test.acc.4+mfr3@gmail.com | triad.test.acc.4+mfr3@gmail.com |
