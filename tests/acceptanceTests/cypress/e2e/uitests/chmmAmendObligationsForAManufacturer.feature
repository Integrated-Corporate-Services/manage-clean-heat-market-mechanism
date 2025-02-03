@RegressionTest
Feature: Verify that an admin user can amend obligation for an organisation
    CHMM Administrator user can amend obligations for an oganisation


    Background: API call to create a new organisation

        Given the user has authentication token for "triad.test.acc.1+admin5@gmail.com" email id
        And the admin user creates an organisation with the following data:
            | key                        | value     |
            | isOnBehalfOfGroup          | false     |
            | organisationName           | newRandom |
            | companyNumber              | newRandom |
            | isFossilFuelBoilerSeller   | false     |
            | manufacturerUserEmailId    | newRandom |
            | manufacturerSROUserEmailId | newRandom |
            | isResponsibleOfficer       | true      |
            | creditTransferOptIn        | false     |
            | creditTransferEmailId      | null      |
        And the admin user log in to View Admin user page with "triad.test.acc.1+admin5@gmail.com" email address
        And the user navigates to manufacturers account page
        And the user navigates to "Summary" page for new "newRandom" organisation created


    Scenario Outline: 1. Verify that an admin user can Add obligations for an organisation

        When the user amends obligation by "<action>" to adjust by "<amount>"
        And the user navigates to "Summary" page from the navigation menu

        Then the user should see "<expectedAmount>" obligations amended by Administrator

        Examples:
            | action   | amount | expectedAmount |
            | Adding   | 111    | +111           |
            | Removing | 112    | -112           |


    @SmokeTest
    Scenario Outline: 2. Verify that an admin can add and remove obligations for same organisation

        When the user amends obligation by "<action1>" to adjust by "<amount1>"
        And the user navigates to "Summary" page from the navigation menu
        And the user amends obligation by "<action2>" to adjust by "<amount2>"
        And the user navigates to "Summary" page from the navigation menu

        Then the user should see "<expectedAmount1>" message on the page
        And the user should see "<expectedAmount2>" message on the page

        Examples:
            | action1 | amount1 | action2  | amount2 | expectedAmount1 | expectedAmount2 |
            | Adding  | 111     | Removing | 112     | +111            | -112            |

    
    Scenario: 3. Verify error messages in amend obligations page

        When the user click on Amend obligation button
        And the user click on continue button

        Then the user should see "Select if you are adding or removing from the obligation" message on the page
        And the user should see "Enter the amount to adjust the obligation by" message on the page

        When the user enters "12.2" for adjustment amount of obligations
        And the user click on continue button

        Then the user should see "The amount you want to adjust the obligation by must be a whole number greater than zero" message on the page
