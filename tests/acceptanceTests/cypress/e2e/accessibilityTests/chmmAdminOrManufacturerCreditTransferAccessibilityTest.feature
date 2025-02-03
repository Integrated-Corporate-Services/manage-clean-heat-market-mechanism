@AccessibilityTest
Feature: Verify Accessibility violations in Admin/manufacturer credit transfer journey

Feature: Verify CHMM Admin/Manufacturer credit transfer feature

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



    
    Scenario:1. Check accessibility violation on transfer credit balance page for admin
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
        When the user selects the organisation "triad.test.acc.4+mfr" from the drop down
        And the user enter number of credits "10"
        And the user validate the page for accessibility violation
        Then there should be no violation
        #Unlink licence holder
        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        And the user send a POST request to "/identity/licenceholders/618a6a59-d159-456f-88f1-d949a02a2543/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """


    Scenario: 2.Check accessibility violation on transfer credit check your answer page for admin
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
        When the user selects the organisation "triad.test.acc.4+mfr" from the drop down
        And the user enter number of credits "10"
        And the user click on continue button
        And the user validate the page for accessibility violation
        Then there should be no violation
        #Unlink licence holder
        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        And the user send a POST request to "/identity/licenceholders/618a6a59-d159-456f-88f1-d949a02a2543/endlink/organisationId" to unlink licence holder to organisation with authentication token:
            """
            {
                "endDate": "2024-04-24",
                "organisationIdToTransfer": null
            }
            """



    Scenario: 3.Check accessibility violation on transfer credit confirmation page for admin
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
        When the user selects the organisation "triad.test.acc.4+mfr" from the drop down
        And the user enter number of credits "10"
        And the user click on continue button
        When the user click on confirm and complete transfer button
        And the user validate the page for accessibility violation
        Then there should be no violation


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


    Scenario Outline: 4. Check accessibility violation on transfer credit balance page for manufacturer
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
        When the user selects the organisation "newRandom" from the drop down
        When the user enter number of credits "5"
        And the user validate the page for accessibility violation
        Then there should be no violation

        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 15    |

   
    Scenario Outline: 5.Check accessibility violation on transfer credit check your answer page for manufacturer
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
        When the user selects the organisation "newRandom" from the drop down
        When the user enter number of credits "5"
        And the user click on continue button
        And the user validate the page for accessibility violation
        Then there should be no violation
        When the user click on confirm and complete transfer button
        Then the user should see "Credit Transfer complete" message on the page

        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 15    |


    Scenario Outline: 6.Check accessibility violation on transfer credit confirmation page for manufacturer
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
        When the user selects the organisation "newRandom" from the drop down
        When the user enter number of credits "5"
        And the user click on continue button
        When the user click on confirm and complete transfer button
        And the user validate the page for accessibility violation
        Then there should be no violation
        Examples:
            | authEmailId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 15    |







