@AccessibilityTest
Feature: Verify Accessibility violations on Manufacturer Onboarding journey pages

    Background: Admin user login 

        Given the manufacturer user log in to chmm system with "triad.test.acc.2@gmail.com" email address

    
    Scenario: 1. Check accessibility violation on group of organisations pages

        When the user validate the page for accessibility violation

        Then there should be no violation

    
    Scenario: 2. Check accessibility violation on Responsible Undertaking page

        Given the user select "No" for group of organisations and continue
        And the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |

        When the user validate the page for accessibility violation

        Then there should be no violation

    
    Scenario: 3. Check accessibility violation on Registered office address page

        Given the user select "No" for group of organisations and continue
        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Random           |

        When the user validate the page for accessibility violation

        Then there should be no violation

    
    Scenario: 4. Check accessibility violation on Legal correspondence address page

        Given the user select "No" for group of organisations and continue
        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Random           |
        And the user submit registered office address with following data:
            | addressLine1 | town   | postcode |
            | Address 1    | London | MK7 8LF  |
        And the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |

        When the user validate the page for accessibility violation

        Then there should be no violation

    
    Scenario: 5. Check accessibility violation on relevant fossil fuel boilers page

        Given the user select "No" for group of organisations and continue
        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Random           |
        And the user submit registered office address with following data:
            | addressLine1 | town   | postcode |
            | Address 1    | London | MK7 8LF  |
        And the user select Yes, use this address for legal correspondence and continue

        When the user validate the page for accessibility violation

        Then there should be no violation

    
    Scenario: 6. Check accessibility violation on Heat pumps brands page

        Given the user select "No" for group of organisations and continue
        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Random           |
        And the user submit registered office address with following data:
            | addressLine1 | town   | postcode |
            | Address 1    | London | MK7 8LF  |
        And the user select Yes, use this address for legal correspondence and continue
        And the user select No for relevant fossil fuel boilers and continue
        And the user select I do supply heat pumps and submit following heat pump brands:
            | heatPumpBrand |

        When the user validate the page for accessibility violation

        Then there should be no violation

    
    Scenario: 7. Check accessibility violation on Your details page

        Given the user select "No" for group of organisations and continue
        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Random           |
        And the user submit registered office address with following data:
            | addressLine1 | town   | postcode |
            | Address 1    | London | MK7 8LF  |
        And the user select Yes, use this address for legal correspondence and continue
        And the user select No for relevant fossil fuel boilers and continue
        And the user select I do not supply heat pumps and continue

        When the user validate the page for accessibility violation

        Then there should be no violation

    
    Scenario: 8. Check accessibility violation on Senior Responsible Officer page

        Given the user select "No" for group of organisations and continue
        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Random           |
        And the user submit registered office address with following data:
            | addressLine1 | town   | postcode |
            | Address 1    | London | MK7 8LF  |
        And the user select Yes, use this address for legal correspondence and continue
        And the user select No for relevant fossil fuel boilers and continue
        And the user select I do not supply heat pumps and continue
        And the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | QATest@example.com | 123123123       |
        And the user select No, I will provide their details for Senior Responsible Officer and submit the following data:
            | fullName  | jobTitle | organisation | emailAddress       | telephoneNumber |

        When the user validate the page for accessibility violation

        Then there should be no violation

    
    Scenario: 9. Check accessibility violation on Opt-in for Credit transfers page

        Given the user select "No" for group of organisations and continue
        And the user select "No" for responsible undertaking and continue by entering the following data:
            | organisationName |
            | Random           |
        And the user submit registered office address with following data:
            | addressLine1 | town   | postcode |
            | Address 1    | London | MK7 8LF  |
        And the user select Yes, use this address for legal correspondence and continue
        And the user select No for relevant fossil fuel boilers and continue
        And the user select I do not supply heat pumps and continue
        And the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | QATest@example.com | 123123123       |
        And the user select Yes, I am the Senior Responsible Officer and continue
        And the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress       | telephoneNumber |

        When the user validate the page for accessibility violation

        Then there should be no violation


Scenario: 10. Check accessibility violation on maufacuturer application journey check application information page
        
        Given the user select "No" for group of organisations and continue
        And the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |
            | random           | Random        |
        And the user submit registered office address with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |

         When the user select No, I will provide another address for legal correspondence and submit with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user select Yes for relevant fossil fuel boilers and continue
        When the user select I do supply heat pumps and submit following heat pump brands:
            | heatPumpBrand |
            | Brand 1       |
            | Brand 2       | 
        And the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress | telephoneNumber |
            | QA TestFN | QA       | Random       | 123123123       |
        When the user select No, I will provide their details for Senior Responsible Officer and submit the following data:
            | fullName  | jobTitle | organisation | emailAddress       | telephoneNumber |
            | QA TestFN | QA       | Test Org     | QATest@example.com | 123123123       |
        And the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress | telephoneNumber |
            | QA TestFN   | Random       | 123123123       |
        
        When the user validate the page for accessibility violation

        Then there should be no violation

    
    Scenario: 11. Check accessibility violation on maufacuturer application journey confiramtion page
        
        Given the user select "No" for group of organisations and continue
        And the user select "Yes" for responsible undertaking and continue by entering the following data:
            | organisationName | companyNumber |
            | random           | Random        |
        And the user submit registered office address with following data:
            | addressLine1 | addressLine2 | town   | county | postcode |
            | Address 1    | Address 2    | London | MK     | MK7 8LF  |
        And the user select Yes, use this address for legal correspondence and continue
        And the user select Yes for relevant fossil fuel boilers and continue
        And the user select I do not supply heat pumps and continue
        And the user submit user details with the following data:
            | fullName  | jobTitle | emailAddress | telephoneNumber |
            | QA TestFN | QA       | Random       | 123123123       |
        And the user select Yes, I am the Senior Responsible Officer and continue
        And the user select Yes, I will provide contact details for credit transfers and submit the following data:
            | contactName | emailAddress | telephoneNumber |
            | QA TestFN   | Random       | 123123123       |
        And the user submits the application
        
        When the user validate the page for accessibility violation

        Then there should be no violation
