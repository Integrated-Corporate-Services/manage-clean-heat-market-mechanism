@AccessibilityTest
Feature: Verify Accessibility violations on Admin views manufacturer accounts page


    Scenario: 1. Check accessibility violation in manufacturer accounts pages

        Given the admin user log in to CHMM system with "triad.test.acc.1+admin2@gmail.com" email address        
        And the admin user navigates to manufacturer accounts page

        When the user validate the page for accessibility violation
        
        Then there should be no violation