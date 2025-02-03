@RegressionTest
Feature: Verify CHMM edit admin user feature
    CHMM invite edit admin user

    Background: Admin user login

        Given the admin user log in to View Admin user page with "triad.test.acc.1+admin1@gmail.com" email address
         When the admin user invites a new admin user with the following data:
            | name            | email  | 
            | Edit admin user | Random | 
        And the user navigates to the administrator accounts page

    @SmokeTest
    Scenario: 1. Verify that the edited admin user details are displayed correctly in the admin accounts table

        When the logged in admin user navigates to user details page for user "Edit admin user" with "Random" email
         And the admin user edits admin user with the following data:
            | name              | email  |
            | Edited admin user | Random | 
        And the user navigates to the administrator accounts page using back to adminstrator account link
        Then the user should see "Administrator accounts" message on the page
        Then the user should see the following new "Random" email admin user details in the administrator accounts table:
            | text                     |
            | Edited admin user        |
           

    
    Scenario: 2. Verify after clicking cancel on edit page, unedited admin data is displayed in the admin accounts table
       
        When the logged in admin user navigates to user details page for user "Edit admin user" with "Random" email
         And the user clicks cancel on edits admin page:
            | name              | email  | 
            | Edited admin user | Random | 
        Then the user should see "Administrator accounts" message on the page
        And the user should see the following new "Random" email admin user details in the administrator accounts table:
            | text               |
            | Edit admin user    |
           

    
    Scenario: 3. Verify after clicking cancel on Check the details before submitting, unedited admin data is displayed in the admin accounts table
        
        When the logged in admin user navigates to user details page for user "Edit admin user" with "Random" email
        And the user clicks cancel on Check the details before submitting page:
            | name              | email  | 
            | Edited admin user | Random | 
        Then the user should see "Administrator accounts" message on the page
        And the user should see the following new "Random" email admin user details in the administrator accounts table:
            | text               |
            | Edit admin user    |
            
