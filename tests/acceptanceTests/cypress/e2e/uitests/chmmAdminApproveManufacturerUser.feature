@RegressionTest
Feature: Verify CHMM Admin can approve Manufacturer user feature
    CHMM Manufacturer user approval 

    Background: Admin user submits manufacturer application

        Given the manufacturer user log in to chmm system with "triad.test.acc.2@gmail.com" email address

        When the user select "No" for group of organisations and continue
        And the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |
            | random           | Random        |
        And the user submit registered office address with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user select Yes, use this address for legal correspondence and continue
        And the user select Yes for relevant fossil fuel boilers and continue
        And the user select I do not supply heat pumps and continue
        And the user submit user details with the following data:
         | fullName  | jobTitle | emailAddress       | telephoneNumber |
           | QA TestFN | QA       | Random | 123123123       |
        And the user select Yes, I am the Senior Responsible Officer and continue
        And the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress       | telephoneNumber |
            | QA TestFN   | Random | 123123123       |
        And the user submits the application
        When the user navigates to manufacturers account page
        And the user navigates to new "random" organisation created


    @SmokeTest
    Scenario: 1. Verify that admin can approve manufacturer user
        When  the admin user clicks on approve button
        And the admin adds additional comments "Added additional comment"
        And the admin clicks yes approve on Confirm manufacturer account approval page
        And the admin clicks yes approve on are you sure page
        Then the user should see "Application approved" message on the page

   
    Scenario: 2. Verify when admin clicks back link on view organisations page takes back to manufacturer accounts page
         When the admin user clicks on back link
        Then the user should see "Manufacturer accounts" message on the page

        
    Scenario: 3. Verify when admin clicks cancel button on Confirm manufacturer account approval takes back to view organisations details page
        When  the admin user clicks on approve button
        And the admin adds additional comments "Added additional comment"
        And the admin clicks cancel on confirm manufacturer account approval page
        Then the user should see "View organisation details" message on the page

 
    Scenario: 4. Verify when admin clicks cancel button on are you sure page takes back to view organisations details page
        When  the admin user clicks on approve button
        And the admin adds additional comments "Added additional comment"
        And the admin clicks yes approve on Confirm manufacturer account approval page
        And the admin clicks cancel on are you sure page
        Then the user should see "View organisation details" message on the page
        
           
