@RegressionTest
Feature: Verify CHMM Admin can amends credit balance feature
    CHMM Manufacturer credit transfer information
    # NOTE: Licence holder 'API LicenceHolder5' id = 618a6a59-d159-456f-88f1-d949a02a2543 is present in the DB
    # Organisation with Id = c7e522cc-8d46-4218-8e83-70b9a3d15f38 and email address = triad.test.acc.4+mfr1@gmail.com is present in the DB


    Background: Create a manufacturer account
        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
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

 @SmokeTest
    Scenario: 1. Verify that manufacturer user can transfer credit after admin adds credits

        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user navigates to manufacturers account page
        And the user navigates to new "newRandom" organisation created
        And the admin user clicks on amend credit balance button
        Then the user should see "Amend credit balance" message on the page
        When the user clicks on adding credit radio button on amend credit balance page
        And the user enter number of credits  on amend credit balance page "100"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        When the user click on amend credit balance button
        Then the user should see "Credit balance amended" message on the page
         When the user clicks on summary link
        Then the user should see below details on check credit balance calculation section
            | text                    | value |
            | Credits amended by administrator | 100   |


    Scenario: 2. Verify manufacturer clicking on cancel button on amend credit transfer page

        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user navigates to manufacturers account page
        And the user navigates to new "newRandom" organisation created
        And the admin user clicks on amend credit balance button
        Then the user should see "Amend credit balance" message on the page
        When the user clicks on adding credit radio button on amend credit balance page
        And the user enter number of credits  on amend credit balance page "100"
        And the user click on cancel button
        Then the user should see "Scheme year summary" message on the page


    Scenario: 3. Verify error messages on amend credit page
         When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user navigates to manufacturers account page
        And the user navigates to new "newRandom" organisation created
        And the admin user clicks on amend credit balance button
        Then the user should see "Amend credit balance" message on the page
        And the user click on continue button
        Then the user should see the following text on the page:
            | text                                             |
            | Select if you are adding or removing credits     |
            | Enter the amount to adjust the credit balance by |

@SmokeTest
    Scenario: 4. Verify that manufacturer user can edit data on check your answer screen
        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user navigates to manufacturers account page
        And the user navigates to new "newRandom" organisation created
        And the admin user clicks on amend credit balance button
        Then the user should see "Amend credit balance" message on the page
        When the user clicks on adding credit radio button on amend credit balance page
        And the user enter number of credits  on amend credit balance page "100"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        Given the user clicks on change link for editing on check your answer page "Are you adding or removing credits?"
        When the user clicks on removing credit radio button on amend credit balance page
        And the user enter number of credits  on amend credit balance page "0"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        When the user click on amend credit balance button
        Then the user should see "Credit balance amended" message on the page