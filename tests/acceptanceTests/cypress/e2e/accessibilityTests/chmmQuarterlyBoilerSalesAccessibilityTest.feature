@AccessibilityTest
Feature: Verify Accessibility violations on quarterly boiler sales journey pages

Background: Manufacturer user login and navigates to annual boiler sales summary page
  Given the admin user log in to CHMM system with "triad.test.acc.4+mfr2@gmail.com" email address
        When the user navigates to annual boiler sales summary page


    Scenario: 1. Check accessibility violation on quarterly boiler sales summary page
        Given the user is on boiler sales summary screen with status "?showAnnual=true&showQuarter=Quarter%201"
        When the user validate the page for accessibility violation
        Then there should be no violation


    Scenario Outline: 2. Check accessibility violation on quarterly boiler sales page
        Given the user is on boiler sales summary screen with status "?showAnnual=true&showQuarter=Quarter%201"
        And the user clicks on submit now button on annual sales page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | <gas>                  | <oil>                  |
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        When the user validate the page for accessibility violation
        Then there should be no violation

        Examples:
            | supportingEvidence | gas  | oil  |
            | docx.docx          | 1001 | 1002 |


    Scenario Outline: 3. Check accessibility violation on check your answers for quarter boiler sales page
        Given the user is on boiler sales summary screen with status "?showAnnual=true&showQuarter=Quarter%201"
        And the user clicks on submit now button on annual sales page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | <gas>                  | <oil>                  |
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        And the user click on continue button
        And the user selects checkbox to confirm the details
        When the user validate the page for accessibility violation
        Then there should be no violation

        Examples:
            | supportingEvidence | gas  | oil  |
            | docx.docx          | 1001 | 1002 |


    Scenario Outline: 3. Check accessibility violation on boiler sales submitted page
        Given the user is on boiler sales summary screen with status "?showAnnual=true&showQuarter=Quarter%201"
        And the user clicks on submit now button on annual sales page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | <gas>                  | <oil>                  |
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        And the user click on continue button
        And the user selects checkbox to confirm the details
        And the user click on submit button
        When the user validate the page for accessibility violation
        Then there should be no violation

        Examples:
            | supportingEvidence | gas  | oil  |
            | docx.docx          | 1001 | 1002 |