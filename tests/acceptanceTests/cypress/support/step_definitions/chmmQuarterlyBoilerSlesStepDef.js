/// <reference types="Cypress" />
import QuarterlyBoilerSalesSummaryPage from "../../e2e/pageObjects/chmmQuarterlyBoilerSalesSummaryPage";


When("the user should see {string} for quarterly {string} sales", function (expectedValue, boilerType) {
    const quarterlyBoilerSalesSummaryPage = new QuarterlyBoilerSalesSummaryPage();
    switch (boilerType.toLowerCase()) {
        case 'gas':
            quarterlyBoilerSalesSummaryPage.checkQuarterlyGasBoilerSalesValues(expectedValue);
            break;
        case 'oil':
            quarterlyBoilerSalesSummaryPage.checkQuarterlyOilBoilerSalesValues(expectedValue);
            break;
    }    
})