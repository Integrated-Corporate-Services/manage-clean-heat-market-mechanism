@AccessibilityTest
Feature: Verify Accessibility violations in Edit Admin users pages

    Background: Admin user login

        Given the admin user log in to View Admin user page with "triad.test.acc.1@gmail.com" email address
        When the admin user invites a new admin user with the following data:
            | name            | email  | permission         |
            | Edit admin user | Random | Regulatory Officer |
        And the user navigates to the administrator accounts page


    Scenario: 1. Check accessibility violation in edit Admin user page
        When the logged in admin user navigates to user details page for user "Edit admin user" with "Random" email
        And the user validate the page for accessibility violation
        Then there should be no violation

    Scenario: 2. Check accessibility violation in Check the details before submitting page
        When the logged in admin user navigates to user details page for user "Edit admin user" with "Random" email
        And the admin user edits admin user page:
            | name              | email  | permission               |
            | Edited admin user | Random | Senior Technical Officer |
        And the user validate the page for accessibility violation
        Then there should be no violation


    Scenario: 3. Check accessibility violation in edit user confirmation page
        When the logged in admin user navigates to user details page for user "Edit admin user" with "Random" email
        And the admin user edits admin user with the following data:
            | name              | email  | permission               |
            | Edited admin user | Random | Senior Technical Officer |
        And the user validate the page for accessibility violation
        Then there should be no violation

