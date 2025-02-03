@APITest
Feature: Verify CHMM POST to amend obligations for an organisation API works as expected
    CHMM POST to amend obligations for an organisation API test


    Scenario Outline: 1. Verify that POST to amend a manufacturer obligations by an admin user returns 201

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

        When the user send a POST request to "/obligation/amend" to amend an obligation with authentication token and following data:
            | key            | value            |
            | organisationId | <organisationId> |
            | schemeYearId   | <schemeYearId>   |
            | value          | <value>          |

        Then the response status code should be 201
        And the response body should contain uuid
        And the user send a GET request to "/obligation/organisation/organisationId/year/<schemeYearId>/summary" with authentication token
        And the response json body should contain "obligationAmendments" element array with the following data:
            | key   | value   |
            | value | <value> |
        And the response json body should contain the following data:
            | key                       | value   |
            | generatedObligations      | 0       |
            | obligationsBroughtForward | 0       |
            | finalObligations          | <value> |
            | obligationsCarriedOver    | 0       |
            | obligationsPaidOff        | 0       |
            | remainingObligations      | <value> |

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | organisationId | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 100   |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | -100  |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 0     |


    Scenario Outline: 2. Verify that POST to amend a manufacturer obligations by a manufacturer user returns 403

        Given the user has authentication token for "<adminEmailId>" email id
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

        When the user send a POST request to "/obligation/amend" to amend an obligation with authentication token and following data:
            | key            | value            |
            | organisationId | <organisationId> |
            | schemeYearId   | <schemeYearId>   |
            | value          | <value>          |

        Then the response status code should be 403

        Examples:
            | adminEmailId                      | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | organisationId | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | newRandom      | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 100   |


    Scenario Outline: 3. Verify that POST to amend a manufacturer obligations by an admin user with decimal values/organisationId/schemeYearId returns 400

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

        When the user send a POST request to "/obligation/amend" to amend an obligation with authentication token and following data:
            | key            | value            |
            | organisationId | <organisationId> |
            | schemeYearId   | <schemeYearId>   |
            | value          | <value>          |

        Then the response status code should be 400

        Examples:
            | authEmailId                       | isOnBehalfOfGroup | isFossilFuelBoilerSeller | manufacturerUserEmailId | isResponsibleOfficer | manufacturerSROUserEmailId | creditTransferOptIn | creditTransferEmailId | mcsManufacturerId | mcsManufacturerName | organisationId                       | schemeYearId                         | value |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | newRandom                            | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 19.1  |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | c9ea5101-e45b-4f16-ac8e-e48c6d83ec88 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 19.1  |
            | triad.test.acc.2+admin1@gmail.com | false             | false                    | newRandom               | true                 | newRandom                  | false               | null                  | newRandom         | newRandom           | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 | d525e380-4aee-40e9-a7f0-1784d8cb49dd | 19.1  |


    Scenario: 4. Verify that POST to amend a manufacturer obligations without authentication token returns 401

        When the user send a POST request to "/obligation/amend" to amend an obligation without authentication token and following data:
            | key            | value                                |
            | organisationId | c9ea5101-e45b-4f16-ac8e-e48c6d83ec81 |
            | schemeYearId   | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            | value          | 100                                  |

        Then the response status code should be 401
