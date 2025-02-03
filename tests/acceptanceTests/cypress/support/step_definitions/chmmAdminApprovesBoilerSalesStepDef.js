/// <reference types="Cypress" />

import MenuPage from "../../e2e/pageObjects/chmmMenuPage";
import AnnualBoilerSalesSummaryPage from "../../e2e/pageObjects/chmmAnnualBoilerSalesSummaryPage";


When( "the user clicks on Boiler sales link", function() {
const menuPage = new MenuPage();
menuPage.clickOnBoilerSalesLink();
})


When("the user clicks on approve annual Boiler sales link", function() {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.clickOnAnnualBoilerSalesApproveButton();
    })
