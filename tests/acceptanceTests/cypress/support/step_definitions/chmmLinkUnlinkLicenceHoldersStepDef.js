/// <reference types="Cypress" />

import CHMMLicenceHoldersPage from "../../e2e/pageObjects/chmmLicenceHoldersPage";

/***************** Step definitions ********************/


When("the user link {string} licence holder", function (licenceHolderName) {
    cy.wait(3000);
    var licenceHoldersPage = new CHMMLicenceHoldersPage();
    licenceHoldersPage.clickOnLinkLicenceHolderButton();
    licenceHoldersPage.selectLicenceHolder(licenceHolderName);
    licenceHoldersPage.clickOnLinkButton();
    cy.wait(3000);
})

When("the user unlink {string} licence holder", function (licenceHolderName) {
    var licenceHoldersPage = new CHMMLicenceHoldersPage();
    licenceHoldersPage.unlinkLicenceHolder(licenceHolderName);
})

When("the user click on Link licence holder button", function () {
    var licenceHoldersPage = new CHMMLicenceHoldersPage();
    licenceHoldersPage.clickOnLinkLicenceHolderButton();
})

When("the user click on link button to link a licence holder", function () {
    var licenceHoldersPage = new CHMMLicenceHoldersPage();
    licenceHoldersPage.clickOnLinkButton();
})