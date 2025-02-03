#@SmokeTest @RegressionTest
Feature: Verify CHMM Admin views and edits manufacturer details feature
    CHMM Admin views and edits manufacturer details

    Background: Manufacturer user submits the application

        Given the manufacturer user log in to chmm system with "triad.test.acc.2@gmail.com" email address

        When the user select "No" for group of organisations and continue
        And the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |
            | Random           | Test company  |
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
            | QA TestFN | QA       | Random | 123123123       |
        When the user select No, I will provide their details for Senior Responsible Officer and submit the following data:
            | fullName  | jobTitle | organisation | emailAddress | telephoneNumber |
            | QA TestFN | QA       | Test Org     | Random       | 123123123       |
        When the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress | telephoneNumber |
            | QA TestFN   | Random       | 123123123       |
        And the user submits the application
        Given The user signs out of the account

        ### Admin logs in ###
        And the admin user log in to View Admin user page with "triad.test.acc.1+admin3@gmail.com" email address
        And the user navigates to manufacturers account page
        When the user navigates to new "random" organisation created

@SmokeTest
    Scenario: 1. Verify that a Admin checking onboarding application after editing organisation details
        Given the user clicks on change link for editing manufacturer "Name of the organisation which is the Responsible Undertaking"
        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Test Org1        |
        Then the user should see below details on check your answers page
            | text                                                            | value     |
            | Name of the organisation which is the Responsible Undertaking   | Test Org1 |
            | Does the Responsible Undertaking have a Companies House number? | No        |

@SmokeTest
    Scenario: 2. Verify that a Admin checking onboarding application after editing register office address
        Given the user clicks on change link for editing manufacturer "Registered office address"
        And the user submit registered office address with following data:
            | addressLine1   | addressLine2   | town          | county | postcode |
            | Address Edited | Address Edited | London Edited | MK1    | MK7 8LG  |
        Then the user should see below details on check your answers page
            | text                      | value          |
            | Registered office address | Address Edited |
            | Registered office address | Address Edited |
            | Registered office address | London Edited  |
            | Registered office address | MK1            |
            | Registered office address | MK7 8LG        |


    Scenario: 3. Verify that a Admin checking onboarding application after editing legal office address with same address
        Given the user clicks on change link for editing manufacturer "Should this address be used for legal correspondence?"
        And the user select Yes, use this address for legal correspondence and continue
        Then the user should see below details on check your answers page
            | text                                                  | value |
            | Should this address be used for legal correspondence? | Yes   |


    Scenario: 4. Verify that a Admin checking onboarding application after editing legal office address with different address
        Given the user clicks on change link for editing manufacturer "Should this address be used for legal correspondence?"
        When the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1   | addressLine2   | town          | county | postcode |
            | Address Edited | Address Edited | London Edited | MK1    | MK7 8LG  |
        Then the user should see below details on check your answers page
            | text                                                  | value          |
            | Should this address be used for legal correspondence? | No             |
            | Address for legal correspondence                      | Address Edited |
            | Address for legal correspondence                      | Address Edited |
            | Address for legal correspondence                      | London Edited  |
            | Address for legal correspondence                      | MK1            |
            | Address for legal correspondence                      | MK7 8LG        |


    Scenario: 5. Verify that a Admin checking onboarding application after editing fossil fuels boilers
        Given the user clicks on change link for editing manufacturer "Do you sell relevant fossil fuel boilers?"
        And the user select Yes for relevant fossil fuel boilers and continue
        Then the user should see below details on check your answers page
            | text                                      | value |
            | Do you sell relevant fossil fuel boilers? | Yes   |


    Scenario: 6. Verify that a Admin checking onboarding application after editing heat pumps brands
        Given the user clicks on change link for editing manufacturer "Do you sell heat pumps?"
        And the user select I do not supply heat pumps and continue
        Then the user should see below details on check your answers page
            | text                   | value |
            | Do you sell heat pumps | No    |


    Scenario: 7. Verify that a Admin checking onboarding application after editing user details
        Given the user clicks on change link for editing manufacturer "Your details"
        When the user edits user details with the following data:
            | fullName      | jobTitle |   telephoneNumber |
            | QA TestEdited | QAEdited |    123123124       |
        Then the user should see below details on check your answers page
            | text         | value               |
            | Your details | QA TestEdited       |
            | Your details | QAEdited            |
            | Your details | 123123124           |


    Scenario: 8. Verify that a Admin checking onboarding application after editing senior responsible officer details
        Given the user clicks on change link for editing manufacturer "Details of the Senior Responsible Officer for your organisation"
        And the user edits Senior Responsible Officer with the following data:
            | fullName          | jobTitle  | organisation    |  telephoneNumber |
            | QA TestFN  Edited | QA Edited | Test Org Edited | 123123124       |
        Then the user should see below details on check your answers page
            | text                                                            | value               |
            | Are you the Senior Responsible Officer for your organisation?   | No                  |
            | Details of the Senior Responsible Officer for your organisation | QA TestFN  Edited   |
            | Details of the Senior Responsible Officer for your organisation | QA Edited           |
            | Details of the Senior Responsible Officer for your organisation | Test Org Edited     |
            | Details of the Senior Responsible Officer for your organisation | 123123124           |


    Scenario: 9. Verify that a Admin checking onboarding application after editing for credit transfer
         Given the user clicks on change link for editing manufacturer "Would you like to opt-in to be contacted for credit transfers?"
        When the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName      | emailAddress        | telephoneNumber |
            | QA TestFN Edited | QATest1@example.com | 123123124       |
        Then the user should see below details on check your answers page
            | text                                                           | value               |
            | Would you like to opt-in to be contacted for credit transfers? | Yes                 |
            | Contact details                                                | QA TestFN Edited    |
            | Contact details                                                | QATest1@example.com |
            | Contact details                                                | 123123124           |

   
    Scenario: 10. Verify that a Admin checking onboarding application after editing for credit transfer details seleting No
         Given the user clicks on change link for editing manufacturer "Would you like to opt-in to be contacted for credit transfers?"
        And the user select No, I don't want to opt-in at this time and continue
        Then the user should see below details on check your answers page
            | text                                                           | value |
            | Would you like to opt-in to be contacted for credit transfers? | No    |
