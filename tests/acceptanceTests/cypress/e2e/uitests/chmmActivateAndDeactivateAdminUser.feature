@RegressionTest
Feature: Verify CHMM Activate and Deactivate an admin user feature
    CHMM activate and deactivate an admin user

    Background: Admin user login

        Given the user has authentication token for "triad.test.acc.1+admin3@gmail.com" email id

    @SmokeTest
    Scenario Outline: 1. Verify that an admin user with active status can be deactivated

        Given the user send a PUT request to "/identity/users/admin/activate" to activate with "<adminEmail2>" email
        And the admin user log in to View Admin user page with "<adminEmail1>" email address

        When the logged in admin user deactivates another admin user "<name>" with "<adminEmail2>" email
        And the user navigates to the administrator accounts page

        Then the user should see the following new "<adminEmail2>" email admin user details in the administrator accounts table:
            | text                        |
            | <name>                      |
            | Inactive                    |
        And the user send a PUT request to "/identity/users/admin/activate" to activate with "<adminEmail2>" email
        Examples:
            | name             | adminEmail1                       | adminEmail2                       |
            | Triad Test 1 + 4 | triad.test.acc.1+admin3@gmail.com | triad.test.acc.1+admin4@gmail.com |

    
    Scenario Outline: 2. Verify that an admin user with active status and start deactivate but return to the list will remain in the active state

        Given the user send a PUT request to "/identity/users/admin/activate" to activate with "<adminEmail2>" email
        And the admin user log in to View Admin user page with "<adminEmail1>" email address
        And the logged in admin user navigates to user details page for user "<name>" with "<adminEmail2>" email

        When the admin user click on deactivate button
        And the admin user cancel the deactivate process by clicking on No return to list button

        Then the user should see the following new "<adminEmail2>" email admin user details in the administrator accounts table:
            | text                        |
            | <name>                      |
            | Active                      |
        Examples:
            | name             | adminEmail1                       | adminEmail2                       |
            | Triad Test 1 + 4 | triad.test.acc.1+admin3@gmail.com | triad.test.acc.1+admin4@gmail.com |


    @SmokeTest
    Scenario Outline: 3. Verify that an admin user with inactive status can be activated

        Given the user send a PUT request to "/identity/users/admin/deactivate" to deactivate with "<adminEmail2>" email
        And the admin user log in to View Admin user page with "<adminEmail1>" email address

        When the logged in admin user activates another admin user "<name>" with "<adminEmail2>" email
        And the user navigates to the administrator accounts page

        Then the user should see the following new "<adminEmail2>" email admin user details in the administrator accounts table:
            | text                        |
            | <name>                      |
            | Active                      |
        And the user send a PUT request to "/identity/users/admin/activate" to activate with "<adminEmail2>" email
        Examples:
            | name             | adminEmail1                       | adminEmail2                       |
            | Triad Test 1 + 4 | triad.test.acc.1+admin3@gmail.com | triad.test.acc.1+admin4@gmail.com |

    
    Scenario Outline: 4. Verify that an admin user with inactive status and start activate but return to the list will remain in the inactive state

        Given the user send a PUT request to "/identity/users/admin/deactivate" to deactivate with "<adminEmail2>" email
        And the admin user log in to View Admin user page with "<adminEmail1>" email address
        And the logged in admin user navigates to user details page for user "<name>" with "<adminEmail2>" email

        When the admin user click on activate button
        And the admin user cancel the activate process by clicking on No return to list button

        Then the user should see the following new "<adminEmail2>" email admin user details in the administrator accounts table:
            | text                        |
            | <name>                      |
            | Inactive                    |
        And the user send a PUT request to "/identity/users/admin/activate" to activate with "<adminEmail2>" email
        Examples:
            | name             | adminEmail1                       | adminEmail2                       |
            | Triad Test 1 + 4 | triad.test.acc.1+admin3@gmail.com | triad.test.acc.1+admin4@gmail.com |
