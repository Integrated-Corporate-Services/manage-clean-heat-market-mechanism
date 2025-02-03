@AccessibilityTest
Feature: Verify Accessibility violations in Admin amends credit journey
   
    Background: Create a manufacturer account
        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
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

            
    Scenario: 1. Check accessibility violation in amend credit balance page 

        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user navigates to manufacturers account page
        And the user navigates to new "newRandom" organisation created
        And the admin user clicks on amend credit balance button
        Then the user should see "Amend credit balance" message on the page
        And the user validate the page for accessibility violation
        Then there should be no violation


Scenario: 2. Check accessibility violation in check your answer page 
        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user navigates to manufacturers account page
        And the user navigates to new "newRandom" organisation created
        And the admin user clicks on amend credit balance button
        Then the user should see "Amend credit balance" message on the page
        When the user clicks on adding credit radio button on amend credit balance page
        And the user enter number of credits  on amend credit balance page "100"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        And the user validate the page for accessibility violation
        Then there should be no violation


Scenario: 3. Check accessibility violation in credit balance amended confirmation page 
        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user navigates to manufacturers account page
        And the user navigates to new "newRandom" organisation created
        And the admin user clicks on amend credit balance button
        Then the user should see "Amend credit balance" message on the page
        When the user clicks on adding credit radio button on amend credit balance page
        And the user enter number of credits  on amend credit balance page "100"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        When the user click on amend credit balance button
        Then the user should see "Credit balance amended" message on the page
        And the user validate the page for accessibility violation
        Then there should be no violation

    
    Scenario: 4. Check accessibility violation on scheme year summary page 
        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user navigates to manufacturers account page
        And the user navigates to new "newRandom" organisation created
        And the admin user clicks on amend credit balance button
        Then the user should see "Amend credit balance" message on the page
        When the user clicks on adding credit radio button on amend credit balance page
        And the user enter number of credits  on amend credit balance page "100"
        And the user click on cancel button
        Then the user should see "Scheme year summary" message on the page
         And the user validate the page for accessibility violation
        Then there should be no violation

   