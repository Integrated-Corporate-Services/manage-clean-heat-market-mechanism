@AccessibilityTest
Feature: Verify Accessibility violations on link licence holders journey pages


    Background: API call to unlink the licence holder if already linked and create a new organisation

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
        And the user navigates to "Licence holders" page for new "random" organisation created

    
    Scenario: 1. Check accessibility violation on link licence holders pages

        When the user validate the page for accessibility violation

        Then there should be no violation

        When the user link "24 Sun - Solar Systems, Ltd." licence holder
        And the user validate the page for accessibility violation

        Then there should be no violation
        And the user unlink "24 Sun - Solar Systems, Ltd." licence holder

