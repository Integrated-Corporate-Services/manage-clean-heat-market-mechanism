@APITest

Feature: Verify CHMM Post to approve Annual boiler sales for an organisation API works as expected
    CHMM Post to approve annual boiler sales for an organisation API test
    
    Scenario Outline: 1. Verify that POST to approve annual boiler sales figures by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id
        And the admin user creates an organisation with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | <isOnBehalfOfGroup>          |
            | organisationName           | newRandom                    |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | <isFossilFuelBoilerSeller>   |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | isResponsibleOfficer       | <isResponsibleOfficer>       |
            | creditTransferOptIn        | <creditTransferOptIn>        |
            | creditTransferEmailId      | <creditTransferEmailId>      |

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to submit boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | oil            | <oil>          |
            | gas            | <gas>          |

        Then the response status code should be 201
        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/approve" to approve boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
        Then the response status code should be 200
        
        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | oil     | gas     |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 101     | 102     |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 0       | 0       |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 1010000 | 1020000 |


    
    Scenario Outline: 2. Verify that POST to approve annual boiler sales figures by a manufacturer user returns 403

        Given the user has authentication token for "<authEmailId>" email id
        And the admin user creates an organisation with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | <isOnBehalfOfGroup>          |
            | organisationName           | newRandom                    |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | <isFossilFuelBoilerSeller>   |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | isResponsibleOfficer       | <isResponsibleOfficer>       |
            | creditTransferOptIn        | <creditTransferOptIn>        |
            | creditTransferEmailId      | <creditTransferEmailId>      |
        And the user has authentication token for "<manufacturerUserEmailId>" manufacturer user email id

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to submit boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | oil            | <oil>          |
            | gas            | <gas>          |

        Then the response status code should be 201
        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/approve" to approve boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
        Then the response status code should be 403

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | oil | gas |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 101 | 102 |

    
    
    Scenario Outline: 3. Verify that POST to approve annual boiler sales with invalid organisationId/schemeYearQuarterId returns 404

        Given the user has authentication token for "<authEmailId>" email id
        And the admin user creates an organisation with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | <isOnBehalfOfGroup>          |
            | organisationName           | newRandom                    |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | <isFossilFuelBoilerSeller>   |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | isResponsibleOfficer       | <isResponsibleOfficer>       |
            | creditTransferOptIn        | <creditTransferOptIn>        |
            | creditTransferEmailId      | <creditTransferEmailId>      |

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to submit boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | oil            | <oil>          |
            | gas            | <gas>          |

        Then the response status code should be 201
        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual/approve" to approve boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value            |
            | organisationId | <organisationId> |
            | schemeYearId   | <schemeYearId>   |

        Then the response status code should be 404

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | oil | gas | organisationId                      |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 101 | 102 | c9ea5101-e45b-4f16-ac8e-e48c6d83ec8 |



  Scenario Outline: 4. Verify that POST to approve annual boiler sales figures without authentication token return 401

        Given the user has authentication token for "<authEmailId>" email id
        And the admin user creates an organisation with the following data:
            | key                        | value                        |
            | isOnBehalfOfGroup          | <isOnBehalfOfGroup>          |
            | organisationName           | newRandom                    |
            | companyNumber              | newRandom                    |
            | isFossilFuelBoilerSeller   | <isFossilFuelBoilerSeller>   |
            | manufacturerUserEmailId    | <manufacturerUserEmailId>    |
            | manufacturerSROUserEmailId | <manufacturerSROUserEmailId> |
            | isResponsibleOfficer       | <isResponsibleOfficer>       |
            | creditTransferOptIn        | <creditTransferOptIn>        |
            | creditTransferEmailId      | <creditTransferEmailId>      |

        When the user send a POST request to "/boilersales/organisation/organisationId/year/<schemeYearId>/annual" to submit boiler sales figures with date "2025-01-01", authentication token and following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | oil            | <oil>          |
            | gas            | <gas>          |

        Then the response status code should be 201
        When the user send a POST request to "/boilersales/organisation/<organisationId>/year/<schemeYearId>/annual/approve" to approve "annual" boiler sales figures without authentication token and following data:
            | key            | value            |
            | organisationId | <organisationId> |
            | schemeYearId   | <schemeYearId>   |

        Then the response status code should be 401

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | schemeYearId                         | oil | gas |organisationId                       |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 101 | 102 |c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 |
