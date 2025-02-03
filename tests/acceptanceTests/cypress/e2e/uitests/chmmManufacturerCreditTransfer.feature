@RegressionTest
Feature: Verify CHMM Manufacturer credit transfer feature
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



    #################### credit transfer as admin ############################
    @SmokeTest
    Scenario: 1. Verify that manufacturer user can transfer credit after linking licence holder to organisation
        #Link Licence holder
        And the user send a POST request to "/identity/licenceholders/618a6a59-d159-456f-88f1-d949a02a2543/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """

        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user is on credit transfer page with "/?dateTimeOverride=2024-10-03"
        And the user navigates to manufacturers account page
        And the user navigates to new "newRandom" organisation created

        And the user clicks on credit transfer button
        Then the user should see "Transfer credits" message on the page
        When the user selects the organisation "triad.test.acc.4+mfr" from the drop down
        And the user enter number of credits "10"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        When the user click on confirm and complete transfer button
        Then the user should see "Credit Transfer complete" message on the page
        When the user clicks on summary link
        Then the user should see below details on check credit balance calculation section
            | text                    | value |
            | Credits transferred out | -10   |

        #Unlink licence holder
        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        And the user send a POST request to "/identity/licenceholders/618a6a59-d159-456f-88f1-d949a02a2543/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

    Scenario: 2. Verify that manufacturer clicking on cancel on credit transfers page
        #Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        #Link licence holder
        And the user send a POST request to "/identity/licenceholders/618a6a59-d159-456f-88f1-d949a02a2543/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """

        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user is on credit transfer page with "/?dateTimeOverride=2024-10-03"
        And the user navigates to manufacturers account page

        And the user navigates to new "newRandom" organisation created
        And the user clicks on credit transfer button
        Then the user should see "Transfer credits" message on the page
        When the user selects the organisation "triad.test.acc.4+mfr" from the drop down
        And the user enter number of credits "10"
        And the user click on cancel button
        Then the user should see "Scheme year summary" message on the page
        #Should change this to 0 after the defect is fixed to take off - sign
        And the user should see below details on check credit balance calculation section
            | text                    | value |
            | Credits transferred out | -0    |

        #Unlink licence holder
        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        And the user send a POST request to "/identity/licenceholders/618a6a59-d159-456f-88f1-d949a02a2543/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """


    Scenario: 3. Verify error messages on transfer credit page
        #Link licence holder
        And the user send a POST request to "/identity/licenceholders/618a6a59-d159-456f-88f1-d949a02a2543/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """

        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user is on credit transfer page with "/?dateTimeOverride=2024-10-03"
        And the user navigates to manufacturers account page

        And the user navigates to new "newRandom" organisation created

        And the user clicks on credit transfer button
        Then the user should see "Transfer credits" message on the page
        And the user click on continue button
        And the user should see the following text on the page:
            | text                                    |
            | Select an organisation                  |
            | Enter the number of credits to transfer |

        #Unlink licence holder
        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        And the user send a POST request to "/identity/licenceholders/618a6a59-d159-456f-88f1-d949a02a2543/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """

    Scenario: 4. Verify that manufacturer user can edit transfer credit after linking licence holder to organisation
        #Unlink licence holder
        And the user send a POST request to "/identity/licenceholders/618a6a59-d159-456f-88f1-d949a02a2543/link-to/organisationId" to link licence holder to organisation with authentication token:
            """
            {
                "startDate": "2024-03-01"
            }
            """
            
        When the admin user log in to View Admin user page with "triad.test.acc.2+admin1@gmail.com" email address after creation of manufacturer
        And the user is on credit transfer page with "/?dateTimeOverride=2024-10-03"
        And the user navigates to manufacturers account page

        And the user navigates to new "newRandom" organisation created

        And the user clicks on credit transfer button
        Then the user should see "Transfer credits" message on the page
        When the user selects the organisation "triad.test.acc.4+mfr" from the drop down
        And the user enter number of credits "10"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        Given the user clicks on change link for editing on check your answer page "Organisation"
        When the user enter number of credits "20"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        When the user click on confirm and complete transfer button
        Then the user should see "Credit Transfer complete" message on the page

        #Unlink licence holder
        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        And the user send a POST request to "/identity/licenceholders/618a6a59-d159-456f-88f1-d949a02a2543/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """


    ################# Credit transfer as manufacturer ####################


    Scenario Outline: 5. Verify that manufacturer user can transfer credit
        Given the user has authentication token for "<authEmailId>" email id
        When the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value                                |
            | organisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId   | <schemeYearId>                       |
            | value          | <value>                              |

        Given the manufacturer user log in to chmm system with "triad.test.acc.4+mfr1@gmail.com"
        And the user is on credit transfer page with "/?dateTimeOverride=2024-10-03"
        When the user navigates to annual boiler sales summary page
        And the user clicks on summary link
        And the user clicks on credit transfer button
        Then the user should see "Transfer credits" message on the page
        When the user selects the organisation "newRandom" from the drop down
        When the user enter number of credits "5"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        When the user click on confirm and complete transfer button
        Then the user should see "Credit Transfer complete" message on the page

        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 15    |



    Scenario Outline: 6. Verify that manufacturer user can edit transfer credit on check your answer page
        Given the user has authentication token for "<authEmailId>" email id
        When the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value                                |
            | organisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId   | <schemeYearId>                       |
            | value          | <value>                              |

        Given the manufacturer user log in to chmm system with "triad.test.acc.4+mfr1@gmail.com"
        And the user is on credit transfer page with "/?dateTimeOverride=2024-10-03"
        When the user navigates to annual boiler sales summary page
        When the user clicks on summary link
        And the user clicks on credit transfer button
        Then the user should see "Transfer credits" message on the page
        When the user selects the organisation "newRandom" from the drop down
        When the user enter number of credits "5"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        When the user clicks on change link for editing on check your answer page "Organisation"
        When the user enter number of credits "20"
        And the user click on continue button
        Then the user should see "Check your answers" message on the page
        When the user click on confirm and complete transfer button
        Then the user should see "Credit Transfer complete" message on the page
        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 15    |



    Scenario Outline: 7. Verify error message for invalid value on credit transfer page
        Given the manufacturer user log in to chmm system with "triad.test.acc.4+mfr1@gmail.com"
        And the user is on credit transfer page with "/?dateTimeOverride=2024-10-03"
        When the user navigates to annual boiler sales summary page
        When the user clicks on summary link
        And the user clicks on credit transfer button
        Then the user should see "Transfer credits" message on the page
        When the user selects the organisation "newRandom" from the drop down
        When the user enter number of credits "-1"
        And the user click on continue button
        Then the user should see the following text on the page:
            | text                                                                       |
            | The number of credits to transfer must be a whole number greater than zero |

        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 15    |



