/// <reference types="Cypress" />

import CHMMAmendObligationsPage from "../../e2e/pageObjects/chmmAmendObligationsPage";
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";

/***************** Step definitions ********************/

When("the user amends obligation by {string} to adjust by {string}", function (amendType, amount) {
    var chmmAmendObligationPage = new CHMMAmendObligationsPage();
    chmmAmendObligationPage.clickOnAmendObligationButton();
    if (amendType.toLowerCase() === 'removing') {
        chmmAmendObligationPage.checkRemovingRadioButton();
    } else {
        chmmAmendObligationPage.checkAddingRadioButton();
    }
    chmmAmendObligationPage.enterObligationAmount(amount);
    chmmAmendObligationPage.clickOnContinueButton();
    chmmAmendObligationPage.clickOnAmendObligationButton();
})

Then("the user should see {string} obligations amended by Administrator", function (expectedAmount) {
    var chmmAmendObligationPage = new CHMMAmendObligationsPage();
    chmmAmendObligationPage.checkAmendedObligationAmount(expectedAmount);
})

When("the user click on Amend obligation button", function () {
    var chmmAmendObligationPage = new CHMMAmendObligationsPage();
    chmmAmendObligationPage.clickOnAmendObligationButton();
})

When("the user enters {string} for adjustment amount of obligations", function (amount) {
    var chmmAmendObligationPage = new CHMMAmendObligationsPage();
    chmmAmendObligationPage.enterObligationAmount(amount);
})

When("the user check {string} radio button to amount obligations", function (amendType) {
    var chmmAmendObligationPage = new CHMMAmendObligationsPage();
    if (amendType.toLowerCase() === 'removing') {
        chmmAmendObligationPage.checkRemovingRadioButton();
    } else {
        chmmAmendObligationPage.checkAddingRadioButton();
    }
})
