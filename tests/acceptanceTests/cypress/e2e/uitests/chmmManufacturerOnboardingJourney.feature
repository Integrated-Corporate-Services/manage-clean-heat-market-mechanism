@RegressionTest
Feature: Verify CHMM Manufacturer onboarding journey feature
    CHMM Manufacturer onboarding journey

    Background: Admin user login

        Given the manufacturer user log in to chmm system with "triad.test.acc.4+mfr1@gmail.com" email address


    @SmokeTest
    Scenario Outline: 1. Verify that a Manufacturer user can register to CHMM system
        When the user select "Yes" for group of organisations with uploading files "<files>" and continue

        And the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |
            | Random           | Random        |
        And the user submit registered office address with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        When the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        When the user select No for relevant fossil fuel boilers and continue
        When the user select I do supply heat pumps and submit following heat pump brands:
            | heatPumpBrand |
            | Brand 1       |
            | Brand 2       |
        When the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | QATest@example.com | 123123123       |
        When the user select No, I will provide their details for Senior Responsible Officer and submit the following data:
            | fullName  | jobTitle | organisation | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | Test Org     | QATest@example.com | 123123123       |
        When the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress       | telephoneNumber |
            | QA TestFN   | QATest@example.com | 123123123       |

        Examples:
            | files                      |
            | docx.docx                  |
            | xls_test_file.xls          |
            | docx.pdf                   |
            | xlsx_test_file.xlsx        |
            | file_example_JPG_500kB.jpg |
            | file-sample_100kB.doc      |
            | file_example_PNG_500kB.png |
            | sample_640×426.bmp         |
            | csv_test_file.csv          |
            | file_example_JPG_500kB.jpg |

   
    Scenario Outline: 2. Verify that a Manufacturer user can register to CHMM system with invalid files
        When the user select "Yes" for group of organisations with uploading files "<files>" and continue
        Then the user should see the following text on the page:
            | text                                                                                               |
            | The selected file must be a .doc, .docx, .pdf, .ppt, .pptx, .xls, .csv, .xlsx, .jpg, .png, or .bmp |
            
            Examples:
            | files        |
            | example.json |
            | eicar.txt    |


    @SmokeTest
    Scenario Outline: 3. Verify that a manufacturer can register as group of organisations with Companies House Number
        When the user select "Yes" for group of organisations with uploading files "<files>" and continue
        And the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |
            | Random           | Random        |
        And the user submit registered office address with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user select Yes, use this address for legal correspondence and continue
        And the user select Yes for relevant fossil fuel boilers and continue
        And the user select I do not supply heat pumps and continue
        And the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | QATest@example.com | 123123123       |
        And the user select Yes, I am the Senior Responsible Officer and continue
        And the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress       | telephoneNumber |
            | QA TestFN   | QATest@example.com | 123123123       |

        Examples:
            | files        |
            | docx.docx |


    Scenario Outline: 4. Verify that a manufacturer can register as a group of organisations without Companies House Number
        When the user select "Yes" for group of organisations with uploading files "<files>" and continue
        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Random           |
        And the user submit registered office address with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user select No for relevant fossil fuel boilers and continue
        And the user select I do supply heat pumps and submit following heat pump brands:
            | heatPumpBrand |
            | Brand 1       |
            | Brand 2       |
        And the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | QATest@example.com | 123123123       |
        And the user select No, I will provide their details for Senior Responsible Officer and submit the following data:
            | fullName  | jobTitle | organisation | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | Test Org     | QATest@example.com | 123123123       |
        And the user select No, I don't want to opt-in at this time and continue

        Examples:
            | files     |
            | docx.docx |


    Scenario: 5. Verify that a manufacturer can register as a single organisation with Companies House Number

        When the user select "No" for group of organisations and continue
        And the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |
            | Random           | Random        |
        And the user submit registered office address with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user select Yes, use this address for legal correspondence and continue
        And the user select Yes for relevant fossil fuel boilers and continue
        And the user select I do not supply heat pumps and continue
        And the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | QATest@example.com | 123123123       |
        And the user select Yes, I am the Senior Responsible Officer and continue
        And the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress       | telephoneNumber |
            | QA TestFN   | QATest@example.com | 123123123       |


    Scenario: 6. Verify that a manufacturer can register as a single organisation without Companies House Number

        When the user select "No" for group of organisations and continue
        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Random           |
        And the user submit registered office address with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user select No for relevant fossil fuel boilers and continue
        And the user select I do supply heat pumps and submit following heat pump brands:
            | heatPumpBrand |
            | Brand 1       |
            | Brand 2       |
        And the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | QATest@example.com | 123123123       |
        And the user select No, I will provide their details for Senior Responsible Officer and submit the following data:
            | fullName  | jobTitle | organisation | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | Test Org     | QATest@example.com | 123123123       |
        And the user select No, I don't want to opt-in at this time and continue