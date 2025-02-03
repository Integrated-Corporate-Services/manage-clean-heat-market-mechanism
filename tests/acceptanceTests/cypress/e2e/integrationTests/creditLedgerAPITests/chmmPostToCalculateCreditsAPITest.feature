@APITest
Feature: Verify CHMM credits are calculated correctly for the installation data
    CHMM POST to Calculate credits API test


    Scenario Outline: 1. Verify that 0 Credits generated for New Built (value=1) and Over 70kwh

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a POST request to "/creditledger/calculate-credits" with authentication token and following data:
            """
            {
                "installations": [
                    {
                        "IsNewBuildID": <IsNewBuildID>,
                        "TotalCapacity": <TotalCapacity>,
                        "TechnologyTypeID": <TechnologyTypeID>,
                        "AirTypeTechnologyID": <AirTypeTechnologyID>,
                        "RenewableSystemDesignID": <RenewableSystemDesignID>,
                        "IsHybrid": <IsHybrid>,
                        "AlternativeHeatingSystemID": <AlternativeHeatingSystemID>,
                        "IsSystemSelectedAsMCSTechnology": <IsSystemSelectedAsMCSTechnology>,
                        "AlternativeHeatingFuelID": <AlternativeHeatingFuelID>,
                        "CommissioningDate": "2024-04-08T14:51:21.255Z",
                        "ID": 0,
                        "Products": [
                            {
                                "ID": 0,
                                "Code": "string",
                                "Name": "string",
                                "ManufacturerID": 0,
                                "Manufacturer": "string"
                            }
                        ],
                        "IsAlternativeHeatingSystemPresent": true,
                        "AlternativeHeatingAgeID": 0,
                        "MPAN": "string",
                        "HowManyCertificates": 0,
                        "Credit": 0
                    }
                ]
            }
            """

        Then the response body array should contain the following data:
            | key    | value    |
            | Credit | <Credit> |

        Examples:
            | IsNewBuildID | TotalCapacity | TechnologyTypeID | AirTypeTechnologyID | RenewableSystemDesignID | IsHybrid | AlternativeHeatingSystemID | IsSystemSelectedAsMCSTechnology | AlternativeHeatingFuelID | Credit |
            | 1            | 70            | 1                | 2                   | 1                       | true     | 1                          | false                           | 1                        | 0      |
            | 2            | 71            | 1                | 2                   | 1                       | true     | 1                          | false                           | 1                        | 0      |
            | 2            | 71            | 1                | 2                   | 1                       | false    | 1                          | false                           | 1                        | 0      |


    Scenario Outline: 2. Verify that 0 Credits generated for TechnologyTypeID other than (values=1, 14, 4 & 17)

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a POST request to "/creditledger/calculate-credits" with authentication token and following data:
            """
            {
                "installations": [
                    {
                        "IsNewBuildID": <IsNewBuildID>,
                        "TotalCapacity": <TotalCapacity>,
                        "TechnologyTypeID": <TechnologyTypeID>,
                        "AirTypeTechnologyID": <AirTypeTechnologyID>,
                        "RenewableSystemDesignID": <RenewableSystemDesignID>,
                        "IsHybrid": <IsHybrid>,
                        "AlternativeHeatingSystemID": <AlternativeHeatingSystemID>,
                        "IsSystemSelectedAsMCSTechnology": <IsSystemSelectedAsMCSTechnology>,
                        "AlternativeHeatingFuelID": <AlternativeHeatingFuelID>,
                        "CommissioningDate": "2024-04-08T14:51:21.255Z",
                        "ID": 0,
                        "Products": [
                            {
                                "ID": 0,
                                "Code": "string",
                                "Name": "string",
                                "ManufacturerID": 0,
                                "Manufacturer": "string"
                            }
                        ],
                        "IsAlternativeHeatingSystemPresent": true,
                        "AlternativeHeatingAgeID": 0,
                        "MPAN": "string",
                        "HowManyCertificates": 0,
                        "Credit": 0
                    }
                ]
            }
            """

        Then the response body array should contain the following data:
            | key    | value    |
            | Credit | <Credit> |

        Examples:
            | IsNewBuildID | TotalCapacity | TechnologyTypeID | AirTypeTechnologyID | RenewableSystemDesignID | IsHybrid | AlternativeHeatingSystemID | IsSystemSelectedAsMCSTechnology | AlternativeHeatingFuelID | Credit |
            | 2            | 70            | 2                | 2                   | 1                       | true     | 1                          | false                           | 1                        | 0      |
            | 2            | 69            | 3                | 2                   | 1                       | true     | 1                          | false                           | 1                        | 0      |


    Scenario Outline: 3. Verify that 0 Credits generated for AirTypeTechnologyID (Air to Air (value=1))

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a POST request to "/creditledger/calculate-credits" with authentication token and following data:
            """
            {
                "installations": [
                    {
                        "IsNewBuildID": <IsNewBuildID>,
                        "TotalCapacity": <TotalCapacity>,
                        "TechnologyTypeID": <TechnologyTypeID>,
                        "AirTypeTechnologyID": <AirTypeTechnologyID>,
                        "RenewableSystemDesignID": <RenewableSystemDesignID>,
                        "IsHybrid": <IsHybrid>,
                        "AlternativeHeatingSystemID": <AlternativeHeatingSystemID>,
                        "IsSystemSelectedAsMCSTechnology": <IsSystemSelectedAsMCSTechnology>,
                        "AlternativeHeatingFuelID": <AlternativeHeatingFuelID>,
                        "CommissioningDate": "2024-04-08T14:51:21.255Z",
                        "ID": 0,
                        "Products": [
                            {
                                "ID": 0,
                                "Code": "string",
                                "Name": "string",
                                "ManufacturerID": 0,
                                "Manufacturer": "string"
                            }
                        ],
                        "IsAlternativeHeatingSystemPresent": true,
                        "AlternativeHeatingAgeID": 0,
                        "MPAN": "string",
                        "HowManyCertificates": 0,
                        "Credit": 0
                    }
                ]
            }
            """

        Then the response body array should contain the following data:
            | key    | value    |
            | Credit | <Credit> |

        Examples:
            | IsNewBuildID | TotalCapacity | TechnologyTypeID | AirTypeTechnologyID | RenewableSystemDesignID | IsHybrid | AlternativeHeatingSystemID | IsSystemSelectedAsMCSTechnology | AlternativeHeatingFuelID | Credit |
            | 2            | 70            | 1                | 1                   | 1                       | true     | 1                          | false                           | 1                        | 0      |
            | 2            | 70            | 14               | 1                   | 1                       | true     | 1                          | false                           | 1                        | 0      |


    Scenario Outline: 4. Verify that 0 Credits generated for RenewableSystemDesignID (Another purpose only (value=7))

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a POST request to "/creditledger/calculate-credits" with authentication token and following data:
            """
            {
                "installations": [
                    {
                        "IsNewBuildID": <IsNewBuildID>,
                        "TotalCapacity": <TotalCapacity>,
                        "TechnologyTypeID": <TechnologyTypeID>,
                        "AirTypeTechnologyID": <AirTypeTechnologyID>,
                        "RenewableSystemDesignID": <RenewableSystemDesignID>,
                        "IsHybrid": <IsHybrid>,
                        "AlternativeHeatingSystemID": <AlternativeHeatingSystemID>,
                        "IsSystemSelectedAsMCSTechnology": <IsSystemSelectedAsMCSTechnology>,
                        "AlternativeHeatingFuelID": <AlternativeHeatingFuelID>,
                        "CommissioningDate": "2024-04-08T14:51:21.255Z",
                        "ID": 0,
                        "Products": [
                            {
                                "ID": 0,
                                "Code": "string",
                                "Name": "string",
                                "ManufacturerID": 0,
                                "Manufacturer": "string"
                            }
                        ],
                        "IsAlternativeHeatingSystemPresent": true,
                        "AlternativeHeatingAgeID": 0,
                        "MPAN": "string",
                        "HowManyCertificates": 0,
                        "Credit": 0
                    }
                ]
            }
            """

        Then the response body array should contain the following data:
            | key    | value    |
            | Credit | <Credit> |

        Examples:
            | IsNewBuildID | TotalCapacity | TechnologyTypeID | AirTypeTechnologyID | RenewableSystemDesignID | IsHybrid | AlternativeHeatingSystemID | IsSystemSelectedAsMCSTechnology | AlternativeHeatingFuelID | Credit |
            | 2            | 70            | 1                | 2                   | 7                       | true     | 1                          | false                           | 1                        | 0      |
            | 2            | 70            | 14               | 2                   | 7                       | true     | 1                          | false                           | 1                        | 0      |
            | 2            | 70            | 4                | 2                   | 7                       | true     | 1                          | false                           | 1                        | 0      |
            | 2            | 70            | 17               | 2                   | 7                       | true     | 1                          | false                           | 1                        | 0      |


    Scenario Outline: 5. Verify that 1 Credit generated for IsHybrid = false

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a POST request to "/creditledger/calculate-credits" with authentication token and following data:
            """
            {
                "installations": [
                    {
                        "IsNewBuildID": <IsNewBuildID>,
                        "TotalCapacity": <TotalCapacity>,
                        "TechnologyTypeID": <TechnologyTypeID>,
                        "AirTypeTechnologyID": <AirTypeTechnologyID>,
                        "RenewableSystemDesignID": <RenewableSystemDesignID>,
                        "IsHybrid": <IsHybrid>,
                        "AlternativeHeatingSystemID": <AlternativeHeatingSystemID>,
                        "IsSystemSelectedAsMCSTechnology": <IsSystemSelectedAsMCSTechnology>,
                        "AlternativeHeatingFuelID": <AlternativeHeatingFuelID>,
                        "CommissioningDate": "2024-04-08T14:51:21.255Z",
                        "ID": 0,
                        "Products": [
                            {
                                "ID": 0,
                                "Code": "string",
                                "Name": "string",
                                "ManufacturerID": 0,
                                "Manufacturer": "string"
                            }
                        ],
                        "IsAlternativeHeatingSystemPresent": true,
                        "AlternativeHeatingAgeID": 0,
                        "MPAN": "string",
                        "HowManyCertificates": 0,
                        "Credit": 0
                    }
                ]
            }
            """

        Then the response body array should contain the following data:
            | key    | value    |
            | Credit | <Credit> |

        Examples:
            | IsNewBuildID | TotalCapacity | TechnologyTypeID | AirTypeTechnologyID | RenewableSystemDesignID | IsHybrid | AlternativeHeatingSystemID | IsSystemSelectedAsMCSTechnology | AlternativeHeatingFuelID | Credit |
            # TechnologyTypeID=1, AirTypeTechnologyID=2, RenewableSystemDesignID=1,2,3,4,5,6
            | 2            | 69            | 1                | 2                   | 1                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 1                | 2                   | 2                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 1                | 2                   | 3                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 1                | 2                   | 4                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 1                | 2                   | 5                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 1                | 2                   | 6                       | false    | 1                          | false                           | 1                        | 1      |
            # TechnologyTypeID=14, AirTypeTechnologyID=2, RenewableSystemDesignID=1,2,3,4,5,6
            | 2            | 69            | 14               | 2                   | 1                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 14               | 2                   | 2                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 14               | 2                   | 3                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 14               | 2                   | 4                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 14               | 2                   | 5                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 14               | 2                   | 6                       | false    | 1                          | false                           | 1                        | 1      |
            # TechnologyTypeID=4, AirTypeTechnologyID=2, RenewableSystemDesignID=1,2,3,4,5,6
            | 2            | 69            | 4                | 2                   | 1                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 4                | 2                   | 2                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 4                | 2                   | 3                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 4                | 2                   | 4                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 4                | 2                   | 5                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 4                | 2                   | 6                       | false    | 1                          | false                           | 1                        | 1      |
            # TechnologyTypeID=17, AirTypeTechnologyID=2, RenewableSystemDesignID=1,2,3,4,5,6
            | 2            | 69            | 17               | 2                   | 1                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 17               | 2                   | 2                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 17               | 2                   | 3                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 17               | 2                   | 4                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 17               | 2                   | 5                       | false    | 1                          | false                           | 1                        | 1      |
            | 2            | 70            | 17               | 2                   | 6                       | false    | 1                          | false                           | 1                        | 1      |



    Scenario Outline: 6. Verify that Credits are generated correctly for TechnologyTypeID=1, AirTypeTechnologyID, RenewableSystemDesignID=1 and different values for AlternativeHeatingSystemID, IsSystemSelectedAsMCSTechnology & AlternativeHeatingFuelID

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id

        When the user send a POST request to "/creditledger/calculate-credits" with authentication token and following data:
            """
            {
                "installations": [
                    {
                        "IsNewBuildID": <IsNewBuildID>,
                        "TotalCapacity": <TotalCapacity>,
                        "TechnologyTypeID": <TechnologyTypeID>,
                        "AirTypeTechnologyID": <AirTypeTechnologyID>,
                        "RenewableSystemDesignID": <RenewableSystemDesignID>,
                        "IsHybrid": <IsHybrid>,
                        "AlternativeHeatingSystemID": <AlternativeHeatingSystemID>,
                        "IsSystemSelectedAsMCSTechnology": <IsSystemSelectedAsMCSTechnology>,
                        "AlternativeHeatingFuelID": <AlternativeHeatingFuelID>,
                        "CommissioningDate": "2024-04-08T14:51:21.255Z",
                        "ID": 0,
                        "Products": [
                            {
                                "ID": 0,
                                "Code": "string",
                                "Name": "string",
                                "ManufacturerID": 0,
                                "Manufacturer": "string"
                            }
                        ],
                        "IsAlternativeHeatingSystemPresent": true,
                        "AlternativeHeatingAgeID": 0,
                        "MPAN": "string",
                        "HowManyCertificates": 0,
                        "Credit": 0
                    }
                ]
            }
            """

        Then the response body array should contain the following data:
            | key    | value    |
            | Credit | <Credit> |

        Examples:
            | IsNewBuildID | TotalCapacity | TechnologyTypeID | AirTypeTechnologyID | RenewableSystemDesignID | IsHybrid | AlternativeHeatingSystemID | IsSystemSelectedAsMCSTechnology | AlternativeHeatingFuelID | Credit |
            # TechnologyTypeID=1, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=1, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=1, Credit=0.5
            | 2            | 69            | 1                | 2                   | 1                       | true     | 1                          | false                           | 1                        | 0.5    |
            # TechnologyTypeID=1, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=1, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=6, Credit=1
            | 2            | 69            | 1                | 2                   | 1                       | true     | 1                          | false                           | 6                        | 1      |
            # TechnologyTypeID=1, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=1, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=10, Credit=0
            | 2            | 69            | 1                | 2                   | 1                       | true     | 1                          | false                           | 10                       | 0      |
            # TechnologyTypeID=1, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=1, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=11, Credit=1
            | 2            | 69            | 1                | 2                   | 1                       | true     | 1                          | false                           | 11                       | 1      |
            # TechnologyTypeID=1, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=4, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=3, Credit=1
            | 2            | 69            | 1                | 2                   | 1                       | true     | 4                          | false                           | 3                        | 1      |
            # TechnologyTypeID=1, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=5, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=8, Credit=0.5
            | 2            | 69            | 1                | 2                   | 1                       | true     | 5                          | false                           | 8                        | 0.5    |
            # TechnologyTypeID=1, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=9, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=5, Credit=0.5
            | 2            | 69            | 1                | 2                   | 1                       | true     | 9                          | false                           | 5                        | 0.5    |
            # TechnologyTypeID=1, AirTypeTechnologyID=2, RenewableSystemDesignID=2, IsHybrid=true, AlternativeHeatingSystemID=7, IsSystemSelectedAsMCSTechnology=true, AlternativeHeatingFuelID=1, Credit=0.5
            | 2            | 69            | 1                | 2                   | 2                       | true     | 7                          | true                            | 1                        | 0.5    |
            # TechnologyTypeID=1, AirTypeTechnologyID=2, RenewableSystemDesignID=6, IsHybrid=true, AlternativeHeatingSystemID=4, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=7, Credit=0.5
            | 2            | 69            | 1                | 2                   | 6                       | true     | 4                          | false                           | 7                        | 0      |
            # TechnologyTypeID=4, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=1, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=1, Credit=0.5
            | 2            | 69            | 4                | 2                   | 1                       | true     | 1                          | false                           | 1                        | 0.5    |
            # TechnologyTypeID=4, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=2, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=3, Credit=1
            | 2            | 69            | 4                | 2                   | 1                       | true     | 2                          | false                           | 3                        | 1      |
            # TechnologyTypeID=4, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=3, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=10, Credit=0
            | 2            | 69            | 4                | 2                   | 1                       | true     | 3                          | false                           | 10                       | 0      |
            # TechnologyTypeID=4, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=5, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=9, Credit=0.5
            | 2            | 69            | 4                | 2                   | 1                       | true     | 5                          | false                           | 9                        | 0.5    |
            # TechnologyTypeID=4, AirTypeTechnologyID=2, RenewableSystemDesignID=5, IsHybrid=true, AlternativeHeatingSystemID=5, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=1, Credit=0.5
            | 2            | 69            | 4                | 2                   | 5                       | true     | 5                          | false                           | 1                        | 0.5    |
            # TechnologyTypeID=14, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=1, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=3, Credit=1
            | 2            | 69            | 14               | 2                   | 1                       | true     | 1                          | false                           | 3                        | 1      |
            # TechnologyTypeID=14, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=2, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=10, Credit=0
            | 2            | 69            | 14               | 2                   | 1                       | true     | 2                          | false                           | 10                       | 0      |
            #  TechnologyTypeID=14, AirTypeTechnologyID=2, RenewableSystemDesignID=3, IsHybrid=true, AlternativeHeatingSystemID=18, IsSystemSelectedAsMCSTechnology=true, AlternativeHeatingFuelID=15, Credit=0.5
            | 2            | 69            | 14               | 2                   | 3                       | true     | 18                         | true                            | 15                       | 0      |
            # TechnologyTypeID=14, AirTypeTechnologyID=2, RenewableSystemDesignID=5, IsHybrid=true, AlternativeHeatingSystemID=13, IsSystemSelectedAsMCSTechnology=true, AlternativeHeatingFuelID=14, Credit=1
            | 2            | 69            | 14               | 2                   | 5                       | true     | 13                         | true                            | 14                       | 1      |
            # TechnologyTypeID=17, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=7, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=16, Credit=1
            | 2            | 69            | 17               | 2                   | 1                       | true     | 7                          | false                           | 16                       | 1      |
            # TechnologyTypeID=17, AirTypeTechnologyID=2, RenewableSystemDesignID=1, IsHybrid=true, AlternativeHeatingSystemID=10, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=2, Credit=0
            | 2            | 69            | 17               | 2                   | 1                       | true     | 10                         | false                           | 2                        | 0      |
            # TechnologyTypeID=17, AirTypeTechnologyID=2, RenewableSystemDesignID=2, IsHybrid=true, AlternativeHeatingSystemID=7, IsSystemSelectedAsMCSTechnology=false, AlternativeHeatingFuelID=7, Credit=0.5
            | 2            | 69            | 17               | 2                   | 2                       | true     | 7                          | false                           | 7                        | 0.5    |
            # TechnologyTypeID=17, AirTypeTechnologyID=2, RenewableSystemDesignID=6, IsHybrid=true, AlternativeHeatingSystemID=18, IsSystemSelectedAsMCSTechnology=true, AlternativeHeatingFuelID=14, Credit=1
            | 2            | 69            | 17               | 2                   | 6                       | true     | 18                         | true                            | 14                       | 0      |
