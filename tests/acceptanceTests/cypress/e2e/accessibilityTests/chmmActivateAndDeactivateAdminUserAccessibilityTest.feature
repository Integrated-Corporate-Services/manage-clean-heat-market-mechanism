@AccessibilityTest
Feature: Verify Accessibility violations in Activate and Deactivate Admin users pages

    Background: Admin user login 

        Given the admin user log in to View Admin user page with "triad.test.acc.1@gmail.com" email address

    
    Scenario: 1. Check accessibility violation in deactivate Admin user pages

        Given the admin user "Triad Test Account3" with "triad.test.acc.3@gmail.com" email is in active state
        And the logged in admin user navigates to user details page for user "Triad Test Account3" with "triad.test.acc.3@gmail.com" email
        When the admin user click on deactivate button
        And the user validate the page for accessibility violation
        Then there should be no violation


    Scenario: 2. Check accessibility violation in activate Admin user pages

        Given the admin user "Triad Test Account3" with "triad.test.acc.3@gmail.com" email is in inactive state
        And the logged in admin user navigates to user details page for user "Triad Test Account3" with "triad.test.acc.3@gmail.com" email
        When the admin user click on activate button
        And the user validate the page for accessibility violation
        Then there should be no violation
        And the logged in admin user activates another admin user "Triad Test Account3" with "triad.test.acc.3@gmail.com" email