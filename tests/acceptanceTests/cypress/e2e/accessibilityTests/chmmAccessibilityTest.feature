Feature: Login page acessibility test

   @AccessibilityTest
 Scenario: Check accessibility violation in the CHMM landing page
    Given the user navigates to the CHMM landing page
    When the user validate the page for accessibility violation
    Then there should be no violation

   @AccessibilityTest
    Scenario:Check accessibility violation in CHMM Dashboard page 
   Given the admin user log in to CHMM dashboard page with "shruthi.jagadeesh@triad.co.uk" email address
   When the user validate the page for accessibility violation
   Then there should be no violation

   @AccessibilityTest
    Scenario:Check accessibility violation in view admin user page 
   Given the admin user log in to CHMM dashboard page with "shruthi.jagadeesh@triad.co.uk" email address
   When the user validate the page for accessibility violation
   Then there should be no violation