@RegressionTest
Feature: Verify CHMM annual boiler sales journey feature
    CHMM Manufacturer annual boiler sales journey

    Background: Manufacturer user login
        Given the admin user log in to CHMM system with "triad.test.acc.4+mfr2@gmail.com" email address
        When the user navigates to annual boiler sales summary page


    #@SmokeTest
    Scenario Outline: 1. Verify that manufacturer user can input and submit annual boilers sales data

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
        # Commented out these steps as currently the values are displayed due an issue with scheme year id. CHMMBETA-701. Need to uncomment this once the bug is fixed
        # And the user navigates to annual boiler sales summary page
        # And the user should see "<gas>" for annual "gas" sales
        # And the user should see "<oil>" for annual "oil" sales

        Examples:
            | verificationStatement      | supportingEvidence         | gas  | oil  |
            | docx.docx                  | xls_test_file.xls          | 1001 | 1002 |
            | docx.pdf                   | xlsx_test_file.xlsx        | 1001 | 1002 |
            | file_example_JPG_500kB.jpg | file-sample_100kB.doc      | 1001 | 1002 |
            | file_example_PNG_500kB.png | sample_640×426.bmp         | 1001 | 1002 |
            | csv_test_file.csv          | file_example_JPG_500kB.jpg | 1001 | 1002 |


    Scenario Outline: 2. Verify that manufacturer user sees error message for large files in input annual boilers sales data
        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        When the user clicks on submit now button on annual sales page
        Then the user should see "Submit annual boiler sale" message on the page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | 1000                   | 1000                   |
        And the user uploads verfication statements to annual boiler sales "<verificationStatement>":
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        Then the user should see the following text on the page:
            | text                                                                                                             |
            | The selected file: very_large_file.docx must be smaller than 5MB                                                 |
            
        Examples:
            | verificationStatement | supportingEvidence   |
            | very_large_file.docx  | very_large_file.docx |
        

  
    Scenario Outline: 2. Verify that manufacturer user sees error message for invalid files in input annual boilers sales data
        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        When the user clicks on submit now button on annual sales page
        Then the user should see "Submit annual boiler sale" message on the page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | 1000                   | 1000                   |
        And the user uploads verfication statements to annual boiler sales "<verificationStatement>":
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        Then the user should see the following text on the page:
            | text                                                                                                             |
            | The selected file: eicar.txt must be a .doc, .docx, .pdf, .ppt, .pptx, .xls, .csv, .xlsx, .jpg, .png, or .bmp |

        Examples:
            | verificationStatement | supportingEvidence |
            | eicar.txt             | eicar.txt          |



    Scenario: 3. Verify the error messages on manufacturer input annual boilers sales data
        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        When the user clicks on submit now button on annual sales page
        And the user click on continue button
        Then the user should see the following text on the page:
            | text                                                               |
            | Enter number of gas boiler sales                                   |
            | Enter number of oil boiler sales                                   |
            | Choose at least one file to upload for your verification statement |
            | Choose at least one file to upload as supporting evidence          |


    Scenario: 4. Verify clicking cancel on submit annual boiler sales page
        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        When the user clicks on submit now button on annual sales page
        And the user click on cancel button
        Then the user should see "Boiler sales summary" message on the page


    # @SmokeTest
    Scenario Outline: 5. Verify that manufacturer user can change input data and submit annual boilers sales
        Given the user is on boiler sales summary screen with status "?annualStatus=Due"
        When the user clicks on submit now button on annual sales page
        Then the user should see "Submit annual boiler sale" message on the page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | 1000                   | 1000                   |
        And the user uploads verfication statements to annual boiler sales "<verificationStatement>":
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        When the user click on continue button
        Then the user should see "Check your 2025 annual verified boiler sales" message on the page
        Given the user clicks on change link for editing manufacturer "Gas"
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | 2000                   | 3000                   |
        When the user click on continue button
        Then the user should see below details on check your annual boiler sales page
            | text | value |
            | Gas  | 2,000 |
            | Oil  | 3,000 |
        Examples:
            | verificationStatement | supportingEvidence |
            | docx.docx             | xls_test_file.xls  |


