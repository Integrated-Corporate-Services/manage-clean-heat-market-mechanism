@RegressionTest
Feature: Verify that an admin user can link and unlink an admin user from an organisation
    CHMM Administrator user can link and unlink a licence holder


    Background: API call to unlink the licence holder if already linked and create a new organisation

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
        And the admin user log in to View Admin user page with "triad.test.acc.1+admin5@gmail.com" email address
        And the user navigates to manufacturers account page


    Scenario: 1. Verify that an admin user can see no licence holders linked message for an organisation

        When the user navigates to "Licence holders" page for new "random" organisation created

        Then the user should see "Currently no licence holders linked with this manufacturer" message on the page

    
    @SmokeTest
    Scenario: 2. Verify that an admin user can link a licence holder to an organisation
        
        Given the user navigates to "Licence holders" page for new "random" organisation created

        When the user link "24 Sun - Solar Systems, Ltd." licence holder

        Then the user should see "24 Sun - Solar Systems, Ltd." message on the page
        And the user should not see "Currently no licence holders linked with this manufacturer" message on the page
        And the user unlink "24 Sun - Solar Systems, Ltd." licence holder


    Scenario: 3. Verify the error messages on link licence holders page
       
        Given the user navigates to "Licence holders" page for new "random" organisation created

        When the user click on Link licence holder button
        And the user click on link button to link a licence holder

        Then the user should see "Select licence holder to link" message on the page
