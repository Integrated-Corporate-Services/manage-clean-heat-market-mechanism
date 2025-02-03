@APITest
Feature: Verify CHMM POST to create a licence holder API works as expected
    CHMM POST to create a licence holder API test


    Scenario Outline: 1. Verify that POST to create a licence holder by an admin user returns 201

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/identity/licenceholders" to create a licence holder with authentication token and following data:
            | key                 | value                 |
            | mcsManufacturerId   | <mcsManufacturerId>   |
            | mcsManufacturerName | <mcsManufacturerName> |

        Then the response status code should be 201
        And the user send a GET request to "/identity/licenceholders/unlinked" with authentication token
        And the response body array for unlinked licence holder with name "newRandom" should contain the following data:
            | key               | value                 |
            | id                | uuid                  |
            | mcsManufacturerId | <mcsManufacturerId>   |
            | name              | <mcsManufacturerName> |

        Examples:
            | authEmailId                       | mcsManufacturerId | mcsManufacturerName |
            | triad.test.acc.2+admin1@gmail.com | newRandom         | newRandom           |


    Scenario Outline: 2. Verify that POST to create a licence holder by a manufacturer user returns 403

        Given the user has authentication token for "triad.test.acc.4+mfr1@gmail.com" manufacturer user email id

        When the user send a POST request to "/identity/licenceholders" to create a licence holder with authentication token and following data:
            | key                 | value                 |
            | mcsManufacturerId   | <mcsManufacturerId>   |
            | mcsManufacturerName | <mcsManufacturerName> |

        Then the response status code should be 403

        Examples:
            | authEmailId                     | mcsManufacturerId | mcsManufacturerName |
            | triad.test.acc.4+mfr1@gmail.com | newRandom         | newRandom           |


    Scenario Outline: 3. Verify that POST to create a duplicate licence holder by an admin user returns 200

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/identity/licenceholders" to create a licence holder with authentication token and following data:
            | key                 | value                 |
            | mcsManufacturerId   | <mcsManufacturerId>   |
            | mcsManufacturerName | <mcsManufacturerName> |

        Then the response status code should be 200
        
        Examples:
            | authEmailId                       | mcsManufacturerId | mcsManufacturerName       |
            | triad.test.acc.2+admin1@gmail.com | 3                 | 3S Swiss Solar Systems AG |


    Scenario Outline: 4.Verify that POST to create a licence holder without authentication token returns 401

        When the user send a POST request to "/identity/licenceholders" to create a licence holder without authentication token and following data:
            | key                 | value                 |
            | mcsManufacturerId   | <mcsManufacturerId>   |
            | mcsManufacturerName | <mcsManufacturerName> |

        Then the response status code should be 401

        Examples:
            | authEmailId                     | mcsManufacturerId | mcsManufacturerName |
            | triad.test.acc.4+mfr1@gmail.com | newRandom         | newRandom           |

