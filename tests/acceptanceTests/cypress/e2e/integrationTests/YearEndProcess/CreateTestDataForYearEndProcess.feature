
# NOTE: Use this feature file only to create test data for manual testing of Year End Process. Run the tests once only from your local. Do not run them from Github Actions
# Calculation Mural - https://app.mural.co/t/triadgroupplc1873/m/triadgroupplc1873/1705660705465/e6e065218ab191f73ebbb4e02416e874b72a7302?sender=44a5558d-edbe-46c5-bc2e-4fb0c78e1c72

# @APITest
Feature: CHMM create test data for Year end process testing
    CHMM create test data for year end process


    # Background: Create a manufacturer account

    #     Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
    #     And the admin user creates an organisation with the following data:
    #         | key                        | value     |
    #         | isOnBehalfOfGroup          | false     |
    #         | organisationName           | newRandom |
    #         | companyNumber              | newRandom |
    #         | isFossilFuelBoilerSeller   | false     |
    #         | manufacturerUserEmailId    | newRandom |
    #         | manufacturerSROUserEmailId | newRandom |
    #         | isResponsibleOfficer       | true      |
    #         | creditTransferOptIn        | false     |
    #         | creditTransferEmailId      | null      |


    # Scenario Outline: Create Users

    #     Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
    #     Then the response status code should be 204

    #     Examples:
    #         | key | value |
    #         | key | value |
    #         | key | value |
    #         | key | value |
    #         | key | value |
    #         | key | value |
    #         | key | value |
    #         | key | value |
    #         | key | value |
    #         | key | value |
    #         | key | value |


    Scenario Outline: 1. Create 30 organisations, submit boiler sales and add credits for the organisations

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
        # Create organisation
        And the admin user creates an organisation with the following data:
            | key                        | value           |
            | isOnBehalfOfGroup          | false           |
            | organisationName           | <orgName>       |
            | companyNumber              | <companyNumber> |
            | isFossilFuelBoilerSeller   | false           |
            | manufacturerUserEmailId    | <userEmail>     |
            | manufacturerSROUserEmailId | <userEmail>     |
            | isResponsibleOfficer       | true            |
            | creditTransferOptIn        | false           |
            | creditTransferEmailId      | null            |
        # Submit and approve boiler sales
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to submit boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | oil            | <oil>          |
            | gas            | <gas>          |
        And the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/approve" to approve boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |

        # Amend obligations
        And the user send a POST request to "/obligation/amend" to amend an obligation with authentication token and following data:
            | key            | value                  |
            | organisationId | newRandom              |
            | schemeYearId   | <schemeYearId>         |
            | value          | <amendObligationValue> |

        # Add credits
        And the user send a POST request to "/creditledger/adjust-credits" to adjust credits with the following data and authentication token:
            | key            | value               |
            | organisationId | newRandom           |
            | schemeYearId   | <schemeYearId>      |
            | value          | <adjustCreditvalue> |

        Examples:
            | orgName      | companyNumber       | userEmail                 | schemeYearId                         | gas   | oil   | amendObligationValue | adjustCreditvalue |
            # Scenario 1: Oil only Boiler sales=998, Obligations=0, Credits=0, Obligations carry forward=0
            | 01_APIMfrOrg | 01_APICompanyNumber | 01_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 0     | 998   | 0                    | 0                 |
         
            # Scenario 2: Oil only Boiler sales=3500, Generated Obligations=100, Admin added obligations=20, Total obligations=120, Total Credits=80, Obligations carry over=-40, Obligations paid off with credits=-80, Payment in lieu=0
            | 02_APIMfrOrg | 02_APICompanyNumber | 02_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 0     | 3500  | 20                   | 80                |
           
            # Scenario 3: Oil only Boiler sales=4000, Generated Obligations=120, Admin added obligations=23, Total obligations=143, Total Credits=30, Obligations carry over=-50, Obligations paid off with credits=-30, Payment in lieu=63
            | 03_APIMfrOrg | 03_APICompanyNumber | 03_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 0     | 4000  | 23                   | 30                |
          
            # Scenario 4: Oil only Boiler sales=5849, Generated Obligations=194, Admin removed obligations=-23, Total obligations=171, Total Credits=20, Obligations carry over=-60, Obligations paid off with credits=-20, Payment in lieu=91
            | 04_APIMfrOrg | 04_APICompanyNumber | 04_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 0     | 5849  | -23                  | 20                |
          
            # Scenario 5: Oil only Boiler sales=8000, Generated Obligations=280, Admin added obligations=6, Total obligations=286, Total Credits=200, Obligations carry over=-86, Obligations paid off with credits=-200, Payment in lieu=0
            | 05_APIMfrOrg | 05_APICompanyNumber | 05_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 0     | 8000  | 6                    | 200               |
         
            # Scenario 6: Gas=19998, Oil=0 Generated Obligations=0, Admin added obligations=0, Total obligations=0, Total Credits=0, Obligations carry over=0, Obligations paid off with credits=0, Payment in lieu=0
            | 06_APIMfrOrg | 06_APICompanyNumber | 06_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 19998 | 0     | 0                    | 0                 |
         
            # Scenario 7: Gas=47000, Oil=0 Generated Obligations=1080, Admin added obligations=63, Total obligations=1143, Total Credits=844, Obligations carry over=-299, Obligations paid off with credits=-844, Payment in lieu=0
            | 07_APIMfrOrg | 07_APICompanyNumber | 07_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 47000 | 0     | 63                   | 844               |
          
            # Scenario 8: Gas=42000, Oil=0 Generated Obligations=880, Admin removed obligations=-26, Total obligations=854, Total Credits=454, Obligations carry over=-300, Obligations paid off with credits=-454, Payment in lieu=100
            | 08_APIMfrOrg | 08_APICompanyNumber | 08_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 42000 | 0     | -26                  | 454               |
          
            # Scenario 9: Gas=44999, Oil=0 Generated Obligations=1000, Admin added obligations=0, Total obligations=1000, Total Credits=600, Obligations carry over=-350, Obligations paid off with credits=-600, Payment in lieu=50
            | 09_APIMfrOrg | 09_APICompanyNumber | 09_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 44999 | 0     | 0                    | 600               |
          
            # Scenario 10: Gas=56000, Oil=0 Generated Obligations=1440, Admin removed obligations=-11, Total obligations=1429, Total Credits=1029, Obligations carry over=-400, Obligations paid off with credits=-1029, Payment in lieu=0
            | 10_APIMfrOrg | 10_APICompanyNumber | 10_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 56000 | 0     | -11                  | 1029              |
          
            # Scenario 11: Gas=19999, Oil=999 Generated Obligations=0, Admin removed obligations=0, Total obligations=0, Total Credits=0, Obligations carry over=0, Obligations paid off with credits=0, Payment in lieu=0
            | 11_APIMfrOrg | 11_APICompanyNumber | 11_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 19999 | 999   | 0                    | 0                 |
          
            # Scenario 12: Gas=49000, Oil=999 Generated Obligations=1160, Admin removed obligations=-17, Total obligations=1143, Total Credits=844, Obligations carry over=-299, Obligations paid off with credits=-844, Payment in lieu=0
            | 12_APIMfrOrg | 12_APICompanyNumber | 12_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 49000 | 999   | -17                  | 844               |
         
            # Scenario 13: Gas=41000, Oil=999 Generated Obligations=840, Admin added obligations=14, Total obligations=854, Total Credits=454, Obligations carry over=-300, Obligations paid off with credits=-454, Payment in lieu=100
            | 13_APIMfrOrg | 13_APICompanyNumber | 13_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 41000 | 999   | 14                   | 454               |
         
            # Scenario 14: Gas=45500, Oil=999 Generated Obligations=1020, Admin removed obligations=20, Total obligations=1000, Total Credits=600, Obligations carry over=-350, Obligations paid off with credits=-600, Payment in lieu=50
            | 14_APIMfrOrg | 14_APICompanyNumber | 14_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 45500 | 999   | -20                  | 600               |
         
            # Scenario 15: Gas=19999, Oil=29600 Generated Obligations=1144, Admin added obligations=0, Total obligations=1144, Total Credits=845, Obligations carry over=-299, Obligations paid off with credits=-845, Payment in lieu=0
            | 15_APIMfrOrg | 15_APICompanyNumber | 15_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 19999 | 29600 | 0                    | 845               |
         
            # Scenario 16: Gas=19999, Oil=22000 Generated Obligations=840, Admin added obligations=17, Total obligations=857, Total Credits=457, Obligations carry over=-300, Obligations paid off with credits=-457, Payment in lieu=100
            | 16_APIMfrOrg | 16_APICompanyNumber | 16_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 19999 | 22000 | 17                   | 457               |
         
            # Scenario 17: Gas=19999, Oil=26500 Generated Obligations=1020, Admin removed obligations=-20, Total obligations=1000, Total Credits=600, Obligations carry over=-350, Obligations paid off with credits=-600, Payment in lieu=50
            | 17_APIMfrOrg | 17_APICompanyNumber | 17_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 19999 | 26500 | -20                  | 600               |
         
            # Scenario 18: Gas=40000, Oil=9600 Generated Obligations=1144, Admin added obligations=0, Total obligations=1144, Total Credits=845, Obligations carry over=-299, Obligations paid off with credits=-845, Payment in lieu=0
            | 18_APIMfrOrg | 18_APICompanyNumber | 18_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 40000 | 9600  | 0                    | 845               |
         
            # Scenario 19: Gas=38000, Oil=3430 Generated Obligations=817, Admin added obligations=40, Total obligations=857, Total Credits=457, Obligations carry over=-300, Obligations paid off with credits=-457, Payment in lieu=100
            | 19_APIMfrOrg | 19_APICompanyNumber | 19_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 38000 | 3430  | 40                   | 457               |
         
            # Scenario 20: Gas=40000, Oil=6500 Generated Obligations=1020, Admin removed obligations=-20, Total obligations=1000, Total Credits=600, Obligations carry over=-350, Obligations paid off with credits=-600, Payment in lieu=50
            | 20_APIMfrOrg | 20_APICompanyNumber | 20_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 40000 | 6500  | -20                  | 600               |
         
            # Scenario 21: Gas=22000, Oil=2000 Generated Obligations=120, Admin removed obligations=-20, Total obligations=100, Total Credits=100, Obligations carry over=0, Obligations paid off with credits=-100, Payment in lieu=0
            | 21_APIMfrOrg | 21_APICompanyNumber | 21_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 22000 | 2000  | -20                  | 100               |
         
            # Scenario 22: Gas=22000, Oil=2000 Generated Obligations=120, Admin removed obligations=-70, Total obligations=50, Total Credits=100, Obligations carry over=0, Obligations paid off with credits=-50, Payment in lieu=0, Credits carry forward=-10, Credits expired=-40
            | 22_APIMfrOrg | 22_APICompanyNumber | 22_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 22000 | 2000  | -70                  | 100               |
         
            # Scenario 23: Gas=21000, Oil=1500 Generated Obligations=60, Admin added obligations=40, Total obligations=100, Total Credits=50, Obligations carry over=-50, Obligations paid off with credits=0, Payment in lieu=0, Credits carry forward=0, Credits expired=-
            | 23_APIMfrOrg | 23_APICompanyNumber | 23_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 21000 | 1500  | 40                   | 50                |
         
            # Scenario 24: Gas=21000, Oil=1500 Generated Obligations=60, Admin added obligations=35, Total obligations=95, Total Credits=100, Obligations carry over=0, Obligations paid off with credits=-95, Payment in lieu=0, Credits carry forward=-5, Credits expired=0
            | 24_APIMfrOrg | 24_APICompanyNumber | 24_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 21000 | 1500  | 35                   | 100               |
         
            # Scenario 25: Gas=19999, Oil=999 Generated Obligations=0, Admin added obligations=0, Total obligations=0, Total Credits=100, Obligations carry over=0, Obligations paid off with credits=0, Payment in lieu=0, Credits carry forward=-10, Credits expired=-90
            | 25_APIMfrOrg | 25_APICompanyNumber | 25_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 19999 | 999   | 0                    | 100               |
         
            # Scenario 26: Gas=19999, Oil=999 Generated Obligations=0, Admin added obligations=857, Total obligations=857, Total Credits=457, Obligations carry over=-300, Obligations paid off with credits=-457, Payment in lieu=100, Credits carry forward=0, Credits expired=0
            | 26_APIMfrOrg | 26_APICompanyNumber | 26_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 19999 | 999   | 857                  | 457               |
         
            # Scenario 27: Gas=25999, Oil=999 Generated Obligations=240, Admin added obligations=0, Total obligations=240, Total Credits=600, Obligations carry over=0, Obligations paid off with credits=-240, Payment in lieu=0, Credits carry forward=-60, Credits expired=-300
            | 27_APIMfrOrg | 27_APICompanyNumber | 27_apimfruser@example.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 25999 | 999   | 0                    | 600               |


# Add licence holders
# Credit transfers
