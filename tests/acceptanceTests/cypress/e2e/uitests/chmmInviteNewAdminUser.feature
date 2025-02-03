@RegressionTest
Feature: Verify CHMM invite new admin user feature
    CHMM invite new admin user

    Background: Admin user login

        Given the admin user log in to View Admin user page with "triad.test.acc.1+admin5@gmail.com" email address

    @SmokeTest
    Scenario: 1. Verify that an admn user can invite a new admin user

        When the admin user invites a new admin user with the following data:
            | name    | email  |
            | QA Test | Random |
        Then the user should see the following text on the page:
            | text           |
            | New user added |
            | Added QA Test  |


    @SmokeTest
    Scenario: 2. Verify that the account details are displayed correctly before submitting the invite for a new admin user

        When the admin user adds a new admin user with the following data:
            | name    | email  |
            | QA Test | Random |
        Then the user should see the following text on the page:
            | text                                          |
            | Check the user details before adding the user |
            | Account details                               |
            | QA Test                                       |
        And the user should see the invited "Random" email address on the page

    @SmokeTest
    Scenario: 3. Verify that the newly invited admin user details are displayed correctly in the admin accounts table with status Pending

        When the admin user invites a new admin user with the following data:
            | name    | email  |
            | QA Test | Random |
        And the user navigates to the administrator accounts page
        Then the user should see the following new "Random" email admin user details in the administrator accounts table:
            | text    |
            | QA Test |
            | Active  |
