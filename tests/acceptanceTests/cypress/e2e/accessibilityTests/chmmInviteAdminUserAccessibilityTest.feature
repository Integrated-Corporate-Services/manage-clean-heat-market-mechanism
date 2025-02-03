@AccessibilityTest
Feature: Verify Accessibility violations on Invite Admin users pages

    Background: Admin user login 

        Given the admin user log in to View Admin user page with "triad.test.acc.1@gmail.com" email address


    Scenario: 1. Check accessibility violation on add Admin user pages

        When the user navigates to the add new user page
        And the user validate the page for accessibility violation
        Then there should be no violation


    Scenario: 2. Check accessibility violation on check details before submitting page 

        When the admin user adds a new admin user with the following data:
            | name    | email  | permission         |
            | QA Test | Random | Regulatory Officer |
        And the user validate the page for accessibility violation
        Then there should be no violation


    Scenario: 3. Check accessibility violation on acknowledge page

        When the admin user invites a new admin user with the following data:
            | name    | email  | permission         |
            | QA Test | Random | Regulatory Officer |
        And the user validate the page for accessibility violation
        Then there should be no violation




    
