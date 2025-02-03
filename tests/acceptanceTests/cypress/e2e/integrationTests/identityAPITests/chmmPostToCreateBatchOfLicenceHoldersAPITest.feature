# @APITest
Feature: Verify CHMM POST to create batch of licence holder API works as expected
    CHMM POST to create batch of licence holder API test

    @APITest
    Scenario Outline: 1. Verify that POST to create a batch of licence holders by admin user returns 201

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/identity/licenceholders/batch" to create a licence holder with authentication token and following data:
            | id | key                 | value                 |
            | 1  | mcsManufacturerId   | <mcsManufacturerId>   |
            | 1  | mcsManufacturerName | <mcsManufacturerName> |
            | 2  | mcsManufacturerId   | <mcsManufacturerId>   |
            | 2  | mcsManufacturerName | <mcsManufacturerName> |

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


    @APITest
    Scenario Outline: 2. Verify that POST to create a licence holder by a manufacturer user returns 403

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/identity/licenceholders/batch" to create a licence holder with authentication token and following data:
            | id | key                 | value                 |
            | 1  | mcsManufacturerId   | <mcsManufacturerId>   |
            | 1  | mcsManufacturerName | <mcsManufacturerName> |
            | 2  | mcsManufacturerId   | <mcsManufacturerId>   |
            | 2  | mcsManufacturerName | <mcsManufacturerName> |

        Then the response status code should be 403

        Examples:
            | authEmailId                     | mcsManufacturerId | mcsManufacturerName |
            | triad.test.acc.4+mfr1@gmail.com | newRandom         | newRandom           |


    # The below scenario is returning 400 when done using postman but it is returning 204 with cypress. Need to investigate this.
    Scenario Outline: 3. Verify that POST to create a duplicate licence holder by an admin user returns 400

        Given the user has authentication token for "<authEmailId>" email id
        And the user send a POST request to "/identity/licenceholders/batch" to create a licence holder with authentication token and following data:
            | id | key                 | value                 |
            | 1  | mcsManufacturerId   | <mcsManufacturerId>   |
            | 1  | mcsManufacturerName | <mcsManufacturerName> |
            | 2  | mcsManufacturerId   | <mcsManufacturerId>   |
            | 2  | mcsManufacturerName | <mcsManufacturerName> |

        When the user send a POST request to "/identity/licenceholders/batch" to create a licence holder with authentication token and following data:
            | id | key                 | value     |
            | 1  | mcsManufacturerId   | duplicate |
            | 1  | mcsManufacturerName | duplicate |
            | 2  | mcsManufacturerId   | duplicate |
            | 2  | mcsManufacturerName | duplicate |

        Then the response status code should be 400

        Examples:
            | authEmailId                       | mcsManufacturerId | mcsManufacturerName |
            | triad.test.acc.2+admin1@gmail.com | newRandom         | newRandom           |


    @APITest
    Scenario: 4.Verify that POST to create a licence holder without authentication token returns 401

        When the user send a POST request to "/identity/licenceholders/batch" to create a licence holder without authentication token and following data:
            | id | key                 | value     |
            | 1  | mcsManufacturerId   | newRandom |
            | 1  | mcsManufacturerName | newRandom |
            | 2  | mcsManufacturerId   | newRandom |
            | 2  | mcsManufacturerName | newRandom |

        Then the response status code should be 401
