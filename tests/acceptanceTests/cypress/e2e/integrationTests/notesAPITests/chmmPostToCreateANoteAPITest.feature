@APITest
Feature: Verify CHMM POST to create a note for an organisation API works as expected
    CHMM POST to create a note for an organisation API test
    # NOTE: Organisation with Id = c7e522cc-8d46-4218-8e83-70b9a3d15f38 and email address = triad.test.acc.4+mfr1@gmail.com is present in the DB


    Background: Create a manufacturer account

        Given the user has authentication token for "triad.test.acc.2+admin1@gmail.com" email id
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


    Scenario Outline: 1. Verify that POST to create a note for an organisation by an admin user returns 201

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | details        | <note>         |

        Then the response status code should be 201
        And the user send a GET request to "/notes/manufacturer/organisationId/year/<schemeYearId>/notes" with authentication token
        And the response body array for get notes for "newRandom" organisation should contain the following data:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | details        | <note>         |

        Examples:
            | authEmailId                       | schemeYearId                         | note                                                                               |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | Admin API test notes !"£$%^"                                                       |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | adsdadhdhakhad akdhasjdasd ad sjjasdhakdhakdh asdhsahadjaf we2983423572905 1`123$% |


    Scenario Outline: 2. Verify that POST to create a note for an organisation by a manufacturer user returns 401

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | details        | <note>         |

        Then the response status code should be 403

        Examples:
            | authEmailId                     | schemeYearId                         | note                         |
            | triad.test.acc.4+mfr1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | Admin API test notes !"£$%^" |


    Scenario Outline: 3. Verify that POST to create a note with invalid organisationId/schemeYearId returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value                        |
            | organisationId | <organisationId>             |
            | schemeYearId   | <schemeYearId>               |
            | details        | Admin API test notes !"£$%^" |

        Then the response status code should be <statusCode>

        Examples:
            | authEmailId                       | schemeYearId                         | organisationId                       | statusCode |
            # Invalid Organisation Id
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | 400        |
            # Invalid schemeYearId
            | newRandom                         | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | c7e522cc-8d46-4218-8e83-70b9a3d15f38 | 403        |


    Scenario: 4. Verify that POST to create a note without authentication token returns 401

        When the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and without authentication token:
            | key            | value                                |
            | organisationId | c7e522cc-8d46-4218-8e83-70b9a3d15f38 |
            | schemeYearId   | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |
            | details        | Admin API test notes !"£$%^"         |

        Then the response status code should be 401


    Scenario Outline: 5. Verify that POST to create a note with text longer than 1200 characters returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | details        | <note>         |

        Then the response status code should be 400

        Examples:
            | authEmailId                       | schemeYearId                         | note                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 | adsdadhdhakhad akdhasjdasd ad sjjasdhakdhakdh asdhsahadjaf we2983423572905 1`12``jsdaoheskndsfj fpf ewp pappoeru vvbklak oairweishgsfnlrj oasfhfywe shs alkfhskdhgrwoetqowur wovnksdkbhqrlnvsa'a'ka[iafks;f  afk'akfwpq'a''f ffnf'a;;sd;afa[@@@d apfjfv'@f jfam fsysdfgsduyaf sdaahf ap@OHOHkiuaunn xcsugnk asnkvnht184725026630*$-81-8346j sfjofjo¬12`i_!*&£06386 sdbasdadla adakhao fffoahfash fdhflafjkdghfla dadjlafj fafofiaf vnxvbadllfjafgh kkfsdghoafhsdg d;jasofjfj gosgsidfjbh zxvyfbkb b bofhkvnkxvnlzjpurfgn vsfjfsihoafj [pfsogh f vk adjsidghofh b nbsdkfsfjgfddsfu sfdsifhs ierwieh whrh dhafkv uvgweruow394u94tyf 9ry2320ug oghsfg 39r340th sfohfshgnajvihiurnbpgh 99wyr0t7495305 fshfieosetw9we9w osiosfsy9s ohsdhg wrgoshihf sohso t04t0349y0uf sfoo s s wer89e s soihdfg w3reuh  w9w39ws s rhghisy9wryw9t s fos s st948ty syhsi sf9w3yt9y  sihfsi 9retywe98 sfhsfh 9wyr9 hs faooereyt s ao ig9erwo soa fg98y8w48 fos oiaohfigw9 kashdwieryehr398 soad afrw38w fhoahnvifdbvo zhchfewrhi3y4923 aohdaohfisfhajdpaweuq9y493479  uoj vdkher rivfshr oorheir 9werwethk hgtfhdkhfskd gweprtw35y gwuyr9320rusof fyw98yw48t p9wa[ah rpw49 wfap9 99weyw9wr gea rp8w9yr9pyt paryp pgp  wryerit 9reos gi f8weyhir9wyrpu1jhftyrytrtryyryryt |


    Scenario Outline: 6. Verify that POST to create a note for an organisation with empty details by an admin user returns 400

        Given the user has authentication token for "<authEmailId>" email id

        When the user send a POST request to "/notes/manufacturer/note" to create a note with the following data and with authentication token:
            | key            | value          |
            | organisationId | newRandom      |
            | schemeYearId   | <schemeYearId> |
            | details        | <note>         |

        Then the response status code should be 400

        Examples:
            | authEmailId                       | schemeYearId                         | note |
            | triad.test.acc.2+admin1@gmail.com | d525e380-4aee-40e9-a7f0-1784d8cb49d9 |      |
