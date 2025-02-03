@RegressionTest
Feature: Verify CHMM Manufacturer onboarding journey error messages feature
    CHMM Manufacturer onboarding journey error messages

    Background: Admin user login

        Given the manufacturer user log in to chmm system with "triad.test.acc.4+mfr1@gmail.com" email address


    Scenario: 1. Verify error messages on group of organisation page

        # Is this registration on behalf of a group of organisations? page 1
        When the user select "nothing" for group of organisations and continue
        Then the user should see "Select if this registration is on behalf of a group of organisations" message on the page

        # Registered name of Responsible Undertaking page 2
        When the user select "No" for group of organisations and continue
        And the user click on continue button
        Then the user should see the following text on the page:
            | text                                                                |
            | Enter name of the organisation which is the Responsible Undertaking |
            | Select if the Responsible Undertaking has a Companies House Number  |

        When the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |
            | Random           |               |
        Then the user should see "Enter Companies House number" message on the page

        # What is the registered office address? page 3
        When the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Random           |
        And the user click on continue button
        Then the user should see the following text on the page:
            | text                   |
            | Enter address line one |
            | Enter town or city     |
            | Enter postcode         |

        # Should this address be used for legal correspondence? page 4
        When the user submit registered office address with following data:
            | addressLine1 | town   | postcode |
            | Address 1    | London | MK7 8LF  |
        And the user click on continue button
        Then the user should see "Select if this address should be used for legal correspondence" message on the page

        When the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1 | addressLine2 | town | county | postcode |
        Then the user should see the following text on the page:
            | text                   |
            | Enter address line one |
            | Enter town or city     |
            | Enter postcode         |

        When the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK78LF   |
        Then the user should see "Enter a full UK postcode in the correct format, like SW1H 0NE" message on the page

        # Do you sell relevant fossil fuel boilers? page 5
        When the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user click on continue button
        Then the user should see "Select if you sell fossil fuel boilers" message on the page

        #  Do you sell heat pumps? page 6
        When the user select No for relevant fossil fuel boilers and continue
        And the user click on continue button
        Then the user should see "Select if you supply heat pumps" message on the page

        When the user select I do supply heat pumps and submit following heat pump brands:
            | heatPumpBrand |
        Then the user should see "Enter heat pump brand" message on the page

        # What are your details? page 7
        When the user select I do supply heat pumps and submit following heat pump brands:
            | heatPumpBrand |
            | Brand 1       |
        And the user click on continue button
        Then the user should see the following text on the page:
            | text                                                                                                                                                                                                             |
            | Enter full name                                                                                                                                                                                                  |
            | Enter job title                                                                                                                                                                                                  |
            | Enter email address                                                                                                                                                                                              |
            | Enter telephone number                                                                                                                                                                                           |
            | Confirm that you are directly employed by the organisation\n            registering with the Clean Heat Market Mechanism\n            scheme and/or have the authority to submit this\n            registration. |

        When the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress      | telephoneNumber |
            | QA TestFN | QA       | QATestexample.com | asdfggh         |
        Then the user should see the following text on the page:
            | text                                                                              |
            | Enter an email address in the correct format, like name@example.com               |
            | Enter a telephone number in the correct format, like 07700900982 or 4407700900982 |

        # Are you the Senior Responsible Officer for your organisation? page 8
        When the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | QATest@example.com | 123123123       |
        And the user click on continue button
        Then the user should see "Select if you are the Senior Responsible Officer for your organisation" message on the page

        When the user select No, I will provide their details for Senior Responsible Officer and submit the following data:
            | fullName | jobTitle | organisation | emailAddress | telephoneNumber |
        Then the user should see the following text on the page:
            | text                   |
            | Enter full name        |
            | Enter job title        |
            | Enter organisation     |
            | Enter email address    |
            | Enter telephone number |

        When the user select No, I will provide their details for Senior Responsible Officer and submit the following data:
            | fullName  | jobTitle | organisation | emailAddress      | telephoneNumber |
            | QA TestFN | QA       | Test Org     | QATestexample.com | adasddaads      |
        Then the user should see the following text on the page:
            | text                                                                              |
            | Enter an email address in the correct format, like name@example.com               |
            | Enter a telephone number in the correct format, like 07700900982 or 4407700900982 |

        # Would you like to opt-in to be contacted for credit transfers? page 9
        When the user select No, I will provide their details for Senior Responsible Officer and submit the following data:
            | fullName  | jobTitle | organisation | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | Test Org     | QATest@example.com | 123123123       |
        And the user click on continue button
        Then the user should see "Select if you would like to opt-in for credit transfers" message on the page

        When the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress | telephoneNumber |
        Then the user should see the following text on the page:
            | text                                    |
            | Enter contact name or department        |
            | Enter email address or telephone number |
            | Enter email address or telephone number |

        When the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress      | telephoneNumber |
            | QA TestFN   | QATestexample.com | asdasd          |
        Then the user should see the following text on the page:
            | text                                                                              |
            | Enter an email address in the correct format, like name@example.com               |
            | Enter a telephone number in the correct format, like 07700900982 or 4407700900982 |
