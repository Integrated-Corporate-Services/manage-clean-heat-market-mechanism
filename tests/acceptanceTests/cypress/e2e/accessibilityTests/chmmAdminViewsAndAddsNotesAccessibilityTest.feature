@AccessibilityTest
Feature: Verify accessibility violation on admin adds notes journey
    CHMM Administrator views and adds nodes

     Background: API call to create a new organisation and navigate to notes page

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
        When the user navigates to new "newRandom" organisation created

   
    Scenario: 1. Check accessibility violation on notes page
        When the user clicks on notes link
        When the user validate the page for accessibility violation
        Then there should be no violation

   
    Scenario Outline: 2. Verify the message administrator can add notes
        When the user clicks on notes link
        And the user clicks on add notes button
        And the user should see "Add note" message on the page
        And the user add details "Adding details" in add notes
        When the user selects and upload files "<files>"
         When the user validate the page for accessibility violation
        Then there should be no violation
        Examples:
            | files             |
            | docx.docx         |
