@AccessibilityTest
Feature: Verify Accessibility violations in admin approve manufacturer users pages

    Background: Admin user login

        Given the manufacturer user log in to chmm system with "triad.test.acc.2@gmail.com" email address
        When the user select "No" for group of organisations and continue
        And the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |
            | random           | Random        |
        And the user submit registered office address with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user select Yes, use this address for legal correspondence and continue
        And the user select Yes for relevant fossil fuel boilers and continue
        And the user select I do not supply heat pumps and continue
        And the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress | telephoneNumber |
            | QA TestFN | QA       | Random       | 123123123       |
        And the user select Yes, I am the Senior Responsible Officer and continue
        And the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress | telephoneNumber |
            | QA TestFN   | Random       | 123123123       |
        And the user submits the application
        When the user navigates to manufacturers account page
        And the user navigates to new "random" organisation created

    Scenario: 1. Check accessibility violation on view organisation details page
        When the user validate the page for accessibility violation
        Then there should be no violation

    Scenario: 2. Check accessibility violation on confirm manufacturer accounts page
        When  the admin user clicks on approve button
        And the user validate the page for accessibility violation
        Then there should be no violation

    Scenario: 3. Check accessibility violation on confirm manufacturer accounts page
        When  the admin user clicks on approve button
        And the admin adds additional comments "Added additional comment"
        And the admin clicks yes approve on Confirm manufacturer account approval page
        And the user validate the page for accessibility violation
        Then there should be no violation

    Scenario: 4. Check accessibility violation on manufacturer accounts confirmation page
        When  the admin user clicks on approve button
        And the admin adds additional comments "Added additional comment"
        And the admin clicks yes approve on Confirm manufacturer account approval page
        And the admin clicks yes approve on are you sure page
        And the user validate the page for accessibility violation
        Then there should be no violation

