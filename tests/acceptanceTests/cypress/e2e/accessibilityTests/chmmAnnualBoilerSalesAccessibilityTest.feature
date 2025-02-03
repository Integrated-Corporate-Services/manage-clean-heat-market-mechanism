@AccessibilityTest
Feature: Verify Accessibility violations on Annual boiler sales journey pages


    Scenario: 1. Check accessibility violation in boiler sales summary page

        Given the admin user log in to CHMM system with "triad.test.acc.4+mfr2@gmail.com" email address
        When the user navigates to annual boiler sales summary page
        When the user validate the page for accessibility violation
        Then there should be no violation

    Scenario: 2. Check accessibility violation in annual boiler sales summary with status due page
        Given the admin user log in to CHMM system with "triad.test.acc.4+mfr2@gmail.com" email address
        When the user navigates to annual boiler sales summary page
        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        When the user validate the page for accessibility violation
        Then there should be no violation


    Scenario Outline: 3. Check accessibility violation on submit annual boiler sales page
        Given the admin user log in to CHMM system with "triad.test.acc.4+mfr2@gmail.com" email address
        When the user navigates to annual boiler sales summary page
        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        When the user clicks on submit now button on annual sales page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | 1000                   | 1000                   |
        And the user uploads verfication statements to annual boiler sales "<verificationStatement>":
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        When the user validate the page for accessibility violation
        Then there should be no violation
        Examples:
            | verificationStatement | supportingEvidence |
            | docx.docx             | xls_test_file.xls  |


    Scenario Outline: 4. Check accessibility violation on check annual boiler sales details page
        Given the admin user log in to CHMM system with "triad.test.acc.4+mfr2@gmail.com" email address
        When the user navigates to annual boiler sales summary page
        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        When the user clicks on submit now button on annual sales page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | 1000                   | 1000                   |
        And the user uploads verfication statements to annual boiler sales "<verificationStatement>":
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        When the user click on continue button
        When the user selects checkbox to confirm the details
        When the user validate the page for accessibility violation
        Then there should be no violation
        Examples:
            | verificationStatement | supportingEvidence |
            | docx.docx             | xls_test_file.xls  |


    Scenario Outline: 5. Check accessibility violation on boiler page
        Given the admin user log in to CHMM system with "triad.test.acc.4+mfr2@gmail.com" email address
        When the user navigates to annual boiler sales summary page
        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        When the user clicks on submit now button on annual sales page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | 1000                   | 1000                   |
        And the user uploads verfication statements to annual boiler sales "<verificationStatement>":
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        When the user click on continue button
        When the user selects checkbox to confirm the details
        When the user click on submit button
        When the user validate the page for accessibility violation
        Then there should be no violation
        Examples:
            | verificationStatement | supportingEvidence |
            | docx.docx             | xls_test_file.xls  |






