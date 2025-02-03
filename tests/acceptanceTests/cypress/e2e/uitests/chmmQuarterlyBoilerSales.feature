@RegressionTest
Feature: Verify CHMM quarterly boiler sales journey feature
    CHMM Manufacturer quarterly boiler sales journey

    Background: Manufacturer user login
        Given the admin user log in to CHMM system with "triad.test.acc.4+mfr2@gmail.com" email address
        When the user navigates to annual boiler sales summary page


    # @SmokeTest
    Scenario Outline: 1. Verify that manufacturer user can input and submit quarterly boilers sales data

        Given the user is on boiler sales summary screen with status "?showAnnual=true&showQuarter=Quarter%201"
        And the user clicks on submit now button on annual sales page
        And the user should see "Submit quarter 1 boiler sales for 2025" message on the page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | <gas>                  | <oil>                  |
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        And the user click on continue button

        Then the user should see "Check your quarter 1 2025 boiler sales" message on the page
        And the user selects checkbox to confirm the details
        And the user click on submit button
        And the user should see "Boiler sales submitted" message on the page
        When the user navigates to annual boiler sales summary page
        And the user should see "<gas>" for quarterly "gas" sales
        And the user should see "<oil>" for quarterly "oil" sales

        Examples:
            | supportingEvidence         | gas  | oil  |
            | docx.docx                  | 1001 | 1002 |
            | docx.pdf                   | 1001 | 1002 |
            | file_example_JPG_500kB.jpg | 1001 | 1002 |
            | file_example_PNG_500kB.png | 1001 | 1002 |
            | csv_test_file.csv          | 1001 | 1002 |
            | xls_test_file.xls          | 1001 | 1002 |
            | xlsx_test_file.xlsx        | 1001 | 1002 |
            | sample_640×426.bmp         | 1001 | 1002 |
            | file-sample_100kB.doc      | 1001 | 1002 |
            | file_example_JPG_500kB.jpg | 1001 | 1002 |



    Scenario Outline: 2. Verify that manufacturer user sees error message for large files and invalid files in quartery boilers sales data
        Given the user is on boiler sales summary screen with status "?showAnnual=true&showQuarter=Quarter%201"
        When the user clicks on submit now button on annual sales page
        Then the user should see "Submit quarter 1 boiler sales for 2025" message on the page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | 1000                   | 1000                   |
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        Then the user should see the following text on the page:
            | text           |
            | <errorMessage> |

        Examples:
            | supportingEvidence   | errorMessage                                                                                                  |
            | very_large_file.docx | The selected file: very_large_file.docx must be smaller than 5MB                                              |
            | eicar.txt            | The selected file: eicar.txt must be a .doc, .docx, .pdf, .ppt, .pptx, .xls, .csv, .xlsx, .jpg, .png, or .bmp |



    Scenario: 3. Verify the error messages on manufacturer quarterly boilers sales data
        Given the user is on boiler sales summary screen with status "?showAnnual=true&showQuarter=Quarter%201"
        When the user clicks on submit now button on annual sales page
        And the user click on continue button
        Then the user should see the following text on the page:
            | text                             |
            | Enter number of gas boiler sales |
            | Enter number of oil boiler sales |


    Scenario: 4. Verify clicking cancel on submit quarterly boiler sales page
        Given the user is on boiler sales summary screen with status "?showAnnual=true&showQuarter=Quarter%201"
        When the user clicks on submit now button on annual sales page
        And the user click on cancel button
        Then the user should see "Boiler sales summary" message on the page


    # @SmokeTest
    Scenario Outline: 5. Verify that manufacturer user can change input data and quarterly annual boilers sales
        Given the user is on boiler sales summary screen with status "?showAnnual=true&showQuarter=Quarter%201"
        When the user clicks on submit now button on annual sales page
        Then the user should see "Submit quarter 1 boiler sales for 2025" message on the page
        When the user enter annual sales with the following data:
            | numberOfGasBoilerSales | numberOfOilBoilerSales |
            | 1000                   | 1000                   |
        And the user uploads supporting evidence to annual boiler sales "<supportingEvidence>":
        When the user click on continue button
        Then the user should see "Check your quarter 1 2025 boiler sales---" message on the page
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
            | supportingEvidence |
            | docx.docx          |
