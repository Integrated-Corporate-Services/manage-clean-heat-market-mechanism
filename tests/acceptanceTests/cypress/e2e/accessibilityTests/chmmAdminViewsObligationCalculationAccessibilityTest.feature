@AccessibilityTest
Feature: Verify Accessibility violations in  view obligation calculation page

    Background: API call to create a new organisation

        Given the user has authentication token for "triad.test.acc.1+admin5@gmail.com" email id
        And the admin user creates an organisation with the following data:
            | key                        | value     |
            | isOnBehalfOfGroup          | false     |
            | organisationName           | newRandom |
            | companyNumber              | newRandom |
            | isFossilFuelBoilerSeller   | false     |
            | manufacturerUserEmailId    | newRandom |
            | manufacturerSROUserEmailId | newRandom |
            | isResponsibleOfficer       | true      |
            | creditTransferOptIn        | false     |
            | creditTransferEmailId      | null      |


    Scenario Outline: Check accessibility violation in Admin summary page

        Given the admin user log in to View Admin user page with "triad.test.acc.1+admin3@gmail.com" email address after creation of manufacturer
        And the user navigates to manufacturers account page

        When the user navigates to "Boiler sales" page for new "newRandom" organisation created


        And the user is on credit transfer page with "?dateTimeOverride=2025-10-02"
        And the user clicks on Boiler sales link
        And the user clicks on submit now button on annual sales page

        And the user should see "Submit annual boiler sale" message on the page

        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | <gas>                  | <oil>                  |
        And the user uploads verfication statements to annual boiler sales "<verificationStatement>":
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        And the user click on continue button


        And the user selects checkbox to confirm the details
        And the user click on submit button

        When the user clicks on Boiler sales link
        And the user clicks on approve annual Boiler sales link

        And the admin clicks yes approve on are you sure page
        And the user clicks on summary link

        And the user validate the page for accessibility violation
        Then there should be no violation

        Examples:
            | verificationStatement | supportingEvidence | gas   | oil   |
            | docx.docx             | xls_test_file.xls  | 30000 | 10000 |
