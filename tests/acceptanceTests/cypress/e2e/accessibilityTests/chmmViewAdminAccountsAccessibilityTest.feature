Feature: Verify Accessibility violations in View Admin accounts page

    Background: Admin user login 

        Given the admin user log in to View Admin user page with "triad.test.acc.1@gmail.com" email address

    @AccessibilityTest
    Scenario: 1. Check accessibility violation in view Admin accounts page

        When the user navigates to the administrator accounts page
        And the user validate the page for accessibility violation
        Then there should be no violation 
