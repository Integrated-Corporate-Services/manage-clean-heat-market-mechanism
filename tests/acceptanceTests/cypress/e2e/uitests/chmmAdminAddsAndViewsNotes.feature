@RegressionTest
Feature: Verify that an admin can add and view notes
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
        And the user selects scheme year 2024


    Scenario: 1. Verify the message administrator will see when there are no notes added
        When the user clicks on notes link
        And the user should see "There are currently no notes to display for this manufacturer." message on the page

    @SmokeTest
    Scenario Outline: 2. Verify the message administrator can add notes
        When the user clicks on notes link
        And the user clicks on add notes button
        And the user should see "Add note" message on the page
        And the user add details "Adding details" in add notes
        When the user selects and upload files "<files>"
        And the user clicks on Add note button
        And the user should see "Note added by Triad Test 1 + 5" message on the page
        Examples:
            | files             |
            | docx.docx         |
            | xls_test_file.xls |
            | docx.pdf          |

    
    Scenario Outline: 3. Verify that when user clicks on cancel button navigates to notes page
        When the user clicks on notes link
        And the user clicks on add notes button
        And the user should see "Add note" message on the page
        And the user add details "Adding details" in add notes
        When the user selects and upload files "<files>"
        And the user click on cancel button
        And the user should see "Notes" message on the page

        Examples:
            | files     |
            | docx.docx |

    
    Scenario Outline: 4. Verify that when user uploads invalid file or large files
        When the user clicks on notes link
        And the user clicks on add notes button
        And the user should see "Add note" message on the page
        And the user add details "Adding details" in add notes
        When the user selects and upload files "<files>"
        
        Then the user should see the following text on the page:
            | text            |
            | <errorMessage> |

        Examples:
            | files     | errorMessage                                                                                                         |
            | eicar.txt | The selected file: eicar.txt must be a .doc, .docx, .pdf, .ppt, .pptx, .xls, .csv, .xlsx, .jpg, .png, or .bmp |
            | very_large_file.docx          | The selected file: very_large_file.docx must be smaller than 5MB                                              |  