@RegressionTest
Feature: Verify CHMM Manufacturer onboarding application information feature
    CHMM Manufacturer checking onboarding application information

    Background: Admin user adds manufacturer user data until submit

        ########## steps to use existing manufacturer user #########

        # Given the admin user log in to CHMM system with "triad.test.acc.1+admin2@gmail.com" email address
        #     And the admin user navigates to manufacturer accounts page
        #     And the user naviagtes to "A-One-LTD1" organisation details page

        Given the manufacturer user log in to chmm system with "triad.test.acc.6+mfr1@gmail.com" email address
        When the user select "No" for group of organisations and continue
        And the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |
            | Test Org         | Test company  |
        And the user submit registered office address with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        When the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        When the user select No for relevant fossil fuel boilers and continue
        When the user select I do supply heat pumps and submit following heat pump brands:
            | heatPumpBrand |
            | Brand 1       |
            | Brand 2       |
        When the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | QATest@example.com | 123123123       |
        When the user select No, I will provide their details for Senior Responsible Officer and submit the following data:
            | fullName  | jobTitle | organisation | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | Test Org     | QATest@example.com | 123123123       |
        When the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress       | telephoneNumber |
            | QA TestFN   | QATest@example.com | 123123123       |

    # Add file upload for group of organisations

    @SmokeTest
    Scenario: 1. Verify that a Manufacturer checking onboarding application full information

        #Organisation structure section
        Then the user should see below details in "Organisation structure" card on check your answers page
            | text                                                       | value |
            | Is this registration on behalf of a group of organisations | No    |

        #Organisation details section
        Then the user should see below details in "Organisation details" card on check your answers page
            | text                                                            | value        |
            | Name of the organisation which is the Responsible Undertaking   | Test Org     |
            | Does the Responsible Undertaking have a Companies House number? | Yes          |
            | Companies House number                                          | Test company |

        #Organisation address section
        Then the user should see below details in "Registered office address" card on check your answers page
            | text             | value     |
            | Address line one | Address 1 |
            | Address line two | Address 2 |
            | City             | London    |
            | County           | MK        |
            | Postcode         | MK7 8LF   |

        #legal correspondence address section
        Then the user should see below details in "Legal correspondence address" card on check your answers page
            | text                                                   | value                   |
            | Which address should be used for legal correspondence? | Use a different address |
            | Address line one                                       | Address 1               |
            | Address line two                                       | Address 2               |
            | City                                                   | London                  |
            | County                                                 | MK                      |
            | Postcode                                               | MK7 8LF                 |

        #Fossils fuel section
        Then the user should see below details in "Fossil fuel boilers" card on check your answers page
            | text                                      | value |
            | Do you sell relevant fossil fuel boilers? | No    |

        #Heat pumps section
        Then the user should see below details in "Heat pumps" card on check your answers page
            | text                   | value   |
            | Do you sell heat pumps | Yes     |
            | Heat pump brands       | Brand 1 |
            | Heat pump brands       | Brand 2 |

        #User details section
        Then the user should see below details in "Applicant details" card on check your answers page
            | text             | value              |
            | Full name        | QA TestFN          |
            | Job title        | QA                 |
            | Email address    | QATest@example.com |
            | Telephone number | 123123123          |

        #Senior responsible officer section
        Then the user should see below details in "Senior Responsible Officer details" card on check your answers page
            | text                                                                   | value              |
            | Is the applicant the Senior Responsible Officer for your organisation? | No                 |
            | Full name                                                              | QA TestFN          |
            | Job title                                                              | QA                 |
            | Email address                                                          | QATest@example.com |
            | Telephone number                                                       | 23123123           |

        #Opt in to be contacted for credit transfer section
        Then the user should see below details in "Opt in to be contacted for credit transfer" card on check your answers page
            | text                                                           | value              |
            | Would you like to opt-in to be contacted for credit transfers? | Yes                |
            | Full name                                                      | QA TestFN          |
            | Email address                                                  | QATest@example.com |
            | Telephone number                                               | 123123123          |


    @SmokeTest
    Scenario: 2. Verify that a Manufacturer checking onboarding application full information after editing organisation details
        Given the user clicks on change link for editing manufacturer card "Organisation details"

        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Test Org1        |
        Then the user should see below details on check your answers page
            | text                                                            | value     |
            | Name of the organisation which is the Responsible Undertaking   | Test Org1 |
            | Does the Responsible Undertaking have a Companies House number? | No        |



    Scenario: 3. Verify that a Manufacturer checking onboarding application full information after editing register office address
        Given the user clicks on change link for editing manufacturer card "Registered office address"
        And the user submit registered office address with following data:
            | addressLine1   | addressLine2   | town          | county | postcode |
            | Address Edited | Address Edited | London Edited | MK1    | MK7 8LG  |
        Then the user should see below details in "Registered office address" card on check your answers page
            | text             | value          |
            | Address line one | Address Edited |
            | Address line two | Address Edited |
            | City             | London Edited  |
            | County           | MK1            |
            | Postcode         | MK7 8LG        |


    Scenario: 4. Verify that a Manufacturer checking onboarding application full information after editing legal office address with same address
        Given the user clicks on change link for editing manufacturer card "Legal correspondence address"
        And the user select Yes, use this address for legal correspondence and continue
        Then the user should see below details on check your answers page
            | text                                                   | value                             |
            | Which address should be used for legal correspondence? | Use the registered office address |


    Scenario: 5. Verify that a Manufacturer checking onboarding application full information after editing legal office address with different address
        Given the user clicks on change link for editing manufacturer card "Legal correspondence address"
        When the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1   | addressLine2   | town          | county | postcode |
            | Address Edited | Address Edited | London Edited | MK1    | MK7 8LG  |
        Then the user should see below details in "Legal correspondence address" card on check your answers page
            | text                                                   | value                   |
            | Which address should be used for legal correspondence? | Use a different address |
            | Address line one                                       | Address Edited          |
            | Address line two                                       | Address Edited          |
            | City                                                   | London Edited           |
            | County                                                 | MK1                     |
            | Postcode                                               | MK7 8LG                 |



    Scenario: 6. Verify that a Manufacturer checking onboarding application full information after editing fossil fuels boilers
        Given the user clicks on change link for editing manufacturer card "Fossil fuel boilers"
        And the user select Yes for relevant fossil fuel boilers and continue
        Then the user should see below details on check your answers page
            | text                                      | value |
            | Do you sell relevant fossil fuel boilers? | Yes   |


    Scenario: 7. Verify that a Manufacturer checking onboarding application full information after editing heat pumps brands
        Given the user clicks on change link for editing manufacturer card "Heat pumps"
        And the user select I do not supply heat pumps and continue
        Then the user should see below details on check your answers page
            | text                   | value |
            | Do you sell heat pumps | No    |

    @SmokeTest
    Scenario: 8. Verify that a Manufacturer checking onboarding application full information after editing user details
        Given the user clicks on change link for editing manufacturer card "Applicant details"
        When the user submit user details with the following data:
            | fullName      | jobTitle | emailAddress        | telephoneNumber |
            | QA TestEdited | QAEdited | QATest1@example.com | 123123124       |
        Then the user should see below details in "Applicant details" card on check your answers page
            | text             | value               |
            | Full name        | QA TestEdited       |
            | Job title        | QAEdited            |
            | Email address    | QATest1@example.com |
            | Telephone number | 123123124           |




    Scenario: 9. Verify that a Manufacturer checking onboarding application full information after editing for credit transfer
        Given the user clicks on change link for editing manufacturer card "Opt in to be contacted for credit transfer"
        When the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName      | emailAddress        | telephoneNumber |
            | QA TestFN Edited | QATest1@example.com | 123123124       |
        Then the user should see below details in "Opt in to be contacted for credit transfer" card on check your answers page
            | text                                                           | value               |
            | Would you like to opt-in to be contacted for credit transfers? | Yes                 |
            | Full name                                                      | QA TestFN Edited    |
            | Email address                                                  | QATest1@example.com |
            | Telephone number                                               | 123123124           |



    Scenario: 10. Verify that a Manufacturer checking onboarding application full information after editing for credit transfer details seleting No
        Given the user clicks on change link for editing manufacturer card "Opt in to be contacted for credit transfer"
        And the user select No, I don't want to opt-in at this time and continue
        Then the user should see below details on check your answers page
            | text                                                           | value |
            | Would you like to opt-in to be contacted for credit transfers? | No    |



