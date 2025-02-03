@AccessibilityTest
Feature: Verify Accessibility violations on Amend Obligations journey pages


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
        And the user click on Amend obligation button


    Scenario: 1. Check accessibility violation on amend obligations pages

        When the user validate the page for accessibility violation

        Then there should be no violation

        When the user check "Adding" radio button to amount obligations
        And the user enters "12.2" for adjustment amount of obligations
        And the user click on continue button
        And the user validate the page for accessibility violation

        Then there should be no violation
