@RegressionTest
Feature: Verify CHMM annual boiler sales journey feature
    CHMM Admin approves annual boiler sales of manufacturer

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


    ## This scenario should be changed after ticket 702 is done
    Scenario Outline: 1. Verify that manufacturer user can input and submit annual boilers sales data

        Given the manufacturer user log in to chmm system with "triad.test.acc.4+mfr1@gmail.com" email address
        When the user navigates to annual boiler sales summary page


        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        And the user clicks on submit now button on annual sales page
        And the user should see "Submit annual boiler sale" message on the page

        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | <gas>                  | <oil>                  |
        And the user uploads verfication statements to annual boiler sales "<verificationStatement>":
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        And the user click on continue button

        Then the user should see "Check your 2025 annual verified boiler sales" message on the page
        And the user selects checkbox to confirm the details
        And the user click on submit button
        And the user should see "Boiler sales submitted" message on the page

        Given The user signs out of the account

        ### Admin logs in ###
        And the admin user log in to View Admin user page with "triad.test.acc.1+admin3@gmail.com" email address
        And the user navigates to manufacturers account page
        When the user navigates to new "newRandom" organisation created

        When the user clicks on Boiler sales link
        And the user clicks on approve annual Boiler sales link
        And the admin clicks yes approve on are you sure page
        Then the user should see "These numbers have been approved" message on the page
        Examples:
            | verificationStatement | supportingEvidence | gas  | oil  |
            | docx.docx             | xls_test_file.xls  | 1001 | 1002 |


    @SmokeTest
    Scenario Outline: 2. Verify that an admin user can input and submit annual boilers sales data

        Given the admin user log in to View Admin user page with "triad.test.acc.1+admin3@gmail.com" email address
        And the user navigates to manufacturers account page
        
        When the user navigates to "Boiler sales" page for new "newRandom" organisation created

        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        And the user clicks on submit now button on annual sales page
        And the user should see "Submit annual boiler sale" message on the page

        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | <gas>                  | <oil>                  |
        And the user uploads verfication statements to annual boiler sales "<verificationStatement>":
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        And the user click on continue button

        Then the user should see "Check your 2025 annual verified boiler sales" message on the page
        And the user selects checkbox to confirm the details
        And the user click on submit button
        And the user should see "Boiler sales submitted" message on the page
        
        ### Admin logs in ###
        And the user navigates to manufacturers account page
        When the user navigates to new "newRandom" organisation created

        When the user clicks on Boiler sales link
        And the user clicks on approve annual Boiler sales link
        And the admin clicks yes approve on are you sure page
        Then the user should see "These numbers have been approved" message on the page
        Examples:
            | verificationStatement | supportingEvidence | gas  | oil  |
            | docx.docx             | xls_test_file.xls  | 1001 | 1002 |


    Scenario Outline: 3. Verify that an admin user clicks on cancel in submit annual sales page takes to boiler sales summary page
        
        Given the admin user log in to View Admin user page with "triad.test.acc.1+admin3@gmail.com" email address
        And the user navigates to manufacturers account page
        
        When the user navigates to "Boiler sales" page for new "newRandom" organisation created

        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        And the user clicks on submit now button on annual sales page
        And the user should see "Submit annual boiler sale" message on the page

        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | <gas>                  | <oil>                  |
        And the user uploads verfication statements to annual boiler sales "<verificationStatement>":
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        And the user click on cancel button
        And the user should see "Boiler sales summary" message on the page
         Examples:
            | verificationStatement | supportingEvidence | gas  | oil  |
            | docx.docx             | xls_test_file.xls  | 1001 | 1002 |
