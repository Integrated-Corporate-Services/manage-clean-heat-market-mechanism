import { createAction, props } from '@ngrx/store';
import { AnnualBoilerSalesDto } from './dtos/annual-boiler-sales';
import { QuarterlyBoilerSalesDto } from './dtos/quarterly-boiler-sales';
import { BoilerSales } from 'src/app/stores/boiler-sales/models/BoilerSales';
import { boilerSalesStatus } from 'src/app/types/chmm-types';
import { SchemeYearDto } from '../organisation-summary/dtos/scheme-year.dto';

export const getSchemeYear = createAction('[Boiler Sales] Get scheme year');

// Annual boiler sales actions

// Get
export const getBoilerSalesSummary = createAction(
  '[Boiler Sales] Get boiler sales summary',
  props<{ organisationId: string }>()
);

export const onErrorGetBoilerSalesSummary = createAction(
  '[Boiler Sales] On Error: Get boiler sales summary',
  props<{ error: any }>()
);

export const clearBoilerSalesSummary = createAction(
  '[Boiler Sales] Clear boiler sales summary'
);

export const copyAnnualBoilerSalesFilesForEditing = createAction(
  '[Boiler Sales] Copy annual boiler sales files for editing',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onCopyAnnualBoilerSalesFilesForEditing = createAction(
  '[Boiler Sales] On copy annual boiler sales files for editing'
);

export const copyQuarterlyBoilerSalesFilesForEditing = createAction(
  '[Boiler Sales] Copy quarterly boiler sales files for editing',
  props<{ organisationId: string; schemeYearId: string, schemeYearQuarterId: string }>()
);

export const onCopyQuarterlyBoilerSalesFilesForEditing = createAction(
  '[Boiler Sales] On copy quarterly boiler sales files for editing'
);

export const getAnnualBoilerSales = createAction(
  '[Boiler Sales] Get annual boiler sales',
  props<{
    organisationId: string;
    annualStatus?: boilerSalesStatus;
    schemeYearId: string;
  }>()
);

export const onGetAnnualBoilerSales = createAction(
  '[Boiler Sales] On: Get annual boiler sales',
  props<{
    annualBoilerSales: AnnualBoilerSalesDto;
    annualStatus?: boilerSalesStatus;
    schemeYear: SchemeYearDto;
  }>()
);

export const onErrorGetAnnualBoilerSales = createAction(
  '[Boiler Sales] On Error: Get annual boiler sales',
  props<{ error: any }>()
);

// Submit or edit
export const storeAnnualBoilerSales = createAction(
  '[Boiler Sales] Store annual boiler sales',
  props<{
    organisationId: string;
    schemeYearId: string;
    boilerSales: BoilerSales;
    isEditing: boolean;
  }>()
);

export const submitAnnualBoilerSales = createAction(
  '[Boiler Sales] Submit annual boiler sales',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onSubmitAnnualBoilerSales = createAction(
  '[Boiler Sales] On: Submit annual boiler sales',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onSubmitAnnualBoilerSalesError = createAction(
  '[Boiler Sales] On Error: Submit annual boiler sales',
  props<{ error?: string | null }>()
);

// Approve
export const approveAnnualBoilerSales = createAction(
  '[Boiler Sales] Approve annual boiler sales',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onApproveAnnualBoilerSales = createAction(
  '[Boiler Sales] On Error: Approve annual boiler sales',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onApproveAnnualBoilerSalesError = createAction(
  '[Boiler Sales] On Error: Approve annual boiler sales',
  props<{ error?: string | null }>()
);

// Verification statement
export const getAnnualVerificationStatement = createAction(
  '[Boiler Sales] Get annual verification statement files',
  props<{ organisationId: string; schemeYearId: string; isEditing: boolean }>()
);

export const onGetAnnualVerificationStatement = createAction(
  '[Boiler Sales] On: Get annual verification statement files',
  props<{ fileNames: string[] }>()
);

export const onAnnualVerificationStatementError = createAction(
  '[Boiler Sales] On Error: Annual verification statement files',
  props<{ error?: string | null }>()
);

export const uploadAnnualVerificationStatement = createAction(
  '[Boiler Sales] Upload annual verification statement files',
  props<{
    organisationId: string;
    schemeYearId: string;
    fileList: FileList;
    isEditing: boolean;
  }>()
);

export const deleteAnnualVerificationStatement = createAction(
  '[Boiler Sales] Delete annual verification statement file',
  props<{
    organisationId: string;
    schemeYearId: string;
    fileName: string;
    isEditing: boolean;
  }>()
);

// Supporting evidence
export const getAnnualSupportingEvidence = createAction(
  '[Boiler Sales] Get annual supporting evidence files',
  props<{ organisationId: string; schemeYearId: string; isEditing: boolean }>()
);

export const onGetAnnualSupportingEvidence = createAction(
  '[Boiler Sales] On: Get annual supporting evidence files',
  props<{ fileNames: string[] }>()
);

export const onAnnualSupportingEvidenceError = createAction(
  '[Boiler Sales] On Error: Annual supporting evidence files',
  props<{ error?: string | null }>()
);

export const uploadAnnualSupportingEvidence = createAction(
  '[Boiler Sales] Upload annual supporting evidence files',
  props<{
    organisationId: string;
    schemeYearId: string;
    fileList: FileList;
    isEditing: boolean;
  }>()
);

export const deleteAnnualSupportingEvidence = createAction(
  '[Boiler Sales] Delete annual supporting evidence file',
  props<{
    organisationId: string;
    schemeYearId: string;
    fileName: string;
    isEditing: boolean;
  }>()
);

// Quarterly boiler sales actions

// Get
export const getQuarterlyBoilerSales = createAction(
  '[Boiler Sales] Get quarterly boiler sales',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onGetQuarterlyBoilerSales = createAction(
  '[Boiler Sales] On: Get quarterly boiler sales',
  props<{
    quarterlyBoilerSales: QuarterlyBoilerSalesDto[];
    schemeYear: SchemeYearDto;
  }>()
);

export const onErrorGetQuarterlyBoilerSales = createAction(
  '[Boiler Sales] On Error: Get quarterly boiler sales',
  props<{ error: any }>()
);

// Submit
export const storeQuarterlyBoilerSales = createAction(
  '[Boiler Sales] Store quarterly boiler sales',
  props<{
    organisationId: string;
    schemeYearId: string;
    schemeYearQuarterId: string;
    boilerSales: BoilerSales;
    isEditing: boolean;
  }>()
);

export const submitQuarterlyBoilerSales = createAction(
  '[Boiler Sales] Submit quarterly boiler sales',
  props<{
    organisationId: string;
    schemeYearId: string;
    schemeYearQuarterId: string;
  }>()
);

export const onSubmitQuarterlyBoilerSales = createAction(
  '[Boiler Sales] On: Submit quarterly boiler sales',
  props<{
    organisationId: string;
    schemeYearId: string;
    schemeYearQuarterId: string;
  }>()
);

export const onSubmitQuarterlyBoilerSalesError = createAction(
  '[Boiler Sales] On Error: Submit quarterly boiler sales',
  props<{ error?: string | null }>()
);

// Supporting evidence
export const getQuarterlySupportingEvidence = createAction(
  '[Boiler Sales] Get quarterly supporting evidence files',
  props<{
    organisationId: string;
    schemeYearId: string;
    schemeYearQuarterId: string;
    isEditing: boolean;
  }>()
);

export const onGetQuarterlySupportingEvidence = createAction(
  '[Boiler Sales] On: Get quarterly supporting evidence files',
  props<{ fileNames: string[] }>()
);

export const onGetQuarterlySupportingEvidenceError = createAction(
  '[Boiler Sales] On Error: Get quarterly supporting evidence files',
  props<{ error?: string | null }>()
);

export const uploadQuarterlySupportingEvidence = createAction(
  '[Boiler Sales] Upload quarterly supporting evidence files',
  props<{
    organisationId: string;
    schemeYearId: string;
    schemeYearQuarterId: string;
    fileList: FileList;
    isEditing: boolean;
  }>()
);

export const deleteQuarterlySupportingEvidence = createAction(
  '[Boiler Sales] Delete quarterly supporting evidence file',
  props<{
    organisationId: string;
    schemeYearId: string;
    schemeYearQuarterId: string;
    fileName: string;
    isEditing: boolean;
  }>()
);

export const downloadQuarterlySupportingEvidence = createAction(
  '[Boiler Sales] Download quarterly supporting evidence',
  props<{
    organisationId: string;
    schemeYearQuarterId: string;
    fileName: string;
  }>()
);
