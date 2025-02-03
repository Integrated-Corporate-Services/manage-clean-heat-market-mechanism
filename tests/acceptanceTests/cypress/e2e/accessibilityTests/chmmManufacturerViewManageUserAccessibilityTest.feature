Feature: Verify Accessibility violations on manufacturer view manage your page


@AccessibilityTest
Scenario: 1. Check accessibility violation on group of organisations pages
        Given the manufacturer user log in to chmm view manage user page with "triad.test.acc.4@gmail.com" email address
        When the user validate the page for accessibility violation
        Then there should be no violation
