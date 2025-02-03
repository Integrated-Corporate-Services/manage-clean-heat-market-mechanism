/// <reference types="Cypress" />
import SummaryPage from "../../e2e/pageObjects/chmmSummaryPage";


Then("the user should see sales above threshold obligation calculation on summary page", function (datatable) {
    const summaryPage = new SummaryPage();
    summaryPage.CheckObligationCalculation(datatable);
})

Then("the user should see total sales for obligation calculation on summary page", function (datatable) {
    const summaryPage = new SummaryPage();
    summaryPage.CheckTotalSalesObligationCalculation(datatable);
})