﻿using Banking.Contracts.Response.Credit;
using Banking.DomainObjects.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Repository.Interface.Credit
{
    public interface ILoanCustomerRepository
    {
        LoanCustomerRespObj UpdateLoanCustomer(LoanCustomerObj entity);
        LoanCustomerRespObj UpdateLoanCustomerByCustomer(LoanCustomerObj entity);
        bool UpdateLoanCustomerFromWebsite(LoanCustomerObj entity);
        Task<LoanCustomerRespObj> UploadCorporateCustomer(List<byte[]> record, string createdBy);
        Task<LoanCustomerRespObj> UploadIndividualCustomer(List<byte[]> record, string createdBy);
        Task<byte[]> GenerateExportLoanCustomer();
        Task<byte[]> GenerateExportCorporateLoanCustomer();
        IEnumerable<LoanCustomerObj> GetAllLoanCustomer();
        IEnumerable<LoanCustomerObj> GetLoanCustomerCASA(int customerId);
        IEnumerable<LoanCustomerLiteObj> GetAllLoanCustomerLite();
        IEnumerable<LoanCustomerLiteObj> GetCustomerLiteBySearch(string fullname, string email, string acctName);
        LoanCustomerObj GetLoanCustomer(int loanCustomerId);
        bool DeleteLoanCustomer(int loanCustomerId);
        StartLoanApplicationCustomerObj GetStartLoanApplicationCustomerById(int id);
        IEnumerable<StartLoanApplicationCustomerObj> GetStartLoanCustomerBySearch(string fullname, string email, string acctName);
        Task<LoanCustomerObj> Login(string username, string password);
        bool verifyEmailAccount(int customerId);
        bool EmailExists(string email);
        LoanCustomerObj GetWebLoanCustomerCASA(int customerId);
        IEnumerable<GLTransactionObj> GetWebCustomerTransactionDetails(string accountNumber);
        bool ForgotPassword(string email);
        bool ResetPassword(ResetPasswordViewModel entity);
        Task<credit_loancustomer> GetCustomerByEmailAsync(string email);

        bool UpdateLoanCustomerDirector(LoanCustomerDirectorObj entity);
        Task<LoanCustomerRespObj> UploadDirectors(List<byte[]> record, string createdBy);
        IEnumerable<LoanCustomerDirectorObj> GetAllLoanCustomerDirector();
        LoanCustomerDirectorObj GetLoanCustomerDirector(int loanCustomerDirectorId);
        bool DeleteLoanCustomerDirector(int loanCustomerDirectorId);
        IEnumerable<LoanCustomerDirectorObj> GetLoanCustomerDirectorByLoanCustomer(int loanCustomerId);
        LoanCustomerDirectorObj GetLoanCustomerDirectorByCustomer(int customerId);
        LoanCustomerDirectorObj GetDirectorSignature(int DirectorId, int customerId);


        bool UpdateLoanCustomerIdentityDetails(LoanCustomerIdentityDetailsObj entity);
        Task<LoanCustomerRespObj> UploadCustomerIdentityDetails(List<byte[]> record, string createdBy);
        IEnumerable<LoanCustomerIdentityDetailsObj> GetAllLoanCustomerIdentityDetails();
        LoanCustomerIdentityDetailsObj GetLoanCustomerIdentityDetails(int loanCustomerIdentityDetailsId);
        bool DeleteLoanCustomerIdentityDetails(int loanCustomerIdentityDetailsId);
        IEnumerable<LoanCustomerIdentityDetailsObj> GetLoanCustomerIdentityDetailsByLoanCustomer(int loanCustomerId);
        LoanCustomerIdentityDetailsObj GetLoanCustomerIdentityDetailsByCustomer(int customerId);


        bool UpdateLoanCustomerNextOfKin(LoanCustomerNextOfKinObj entity);
        Task<LoanCustomerRespObj> UploadCustomerNextOfKin(List<byte[]> record, string createdBy);
        IEnumerable<LoanCustomerNextOfKinObj> GetAllLoanCustomerNextOfKin();
        LoanCustomerNextOfKinObj GetLoanCustomerNextOfKin(int loanCustomerNextOfKinId);
        bool DeleteLoanCustomerNextOfKin(int loanCustomerNextOfKinId);
        IEnumerable<LoanCustomerNextOfKinObj> GetLoanCustomerNextOfKinByLoanCustomer(int loanCustomerId);
        LoanCustomerNextOfKinObj GetLoanCustomerNextOfKinByCustomer(int customerId);


        bool UpdateLoanCustomerDocument(LoanCustomerDocumentObj entity);
        IEnumerable<LoanCustomerDocumentObj> GetAllLoanCustomerDocument();
        LoanCustomerDocumentObj GetLoanCustomerDocument(int loanCustomerDocumentId);
        bool DeleteLoanCustomerDocument(int loanCustomerDocumentId);
        IEnumerable<LoanCustomerDocumentObj> GetLoanCustomerDocumentByLoanCustomer(int loanCustomerId);
        LoanCustomerDocumentObj GetLoanCustomerDocumentByCustomer(int customerId);


        LoanCustomerIdentityRespObj UpdateLoanCustomerBankDetails(LoanCustomerBankDetailsObj entity);
        Task<LoanCustomerRespObj> UploadCustomerBankDetails(List<byte[]> record, string createdBy);
        IEnumerable<LoanCustomerBankDetailsObj> GetAllLoanCustomerBankDetails();
        LoanCustomerBankDetailsObj GetLoanCustomerBankDetails(int loanCustomerBankDetailsId);
        bool DeleteLoanCustomerBankDetails(int loanCustomerBankDetailsId);
        IEnumerable<LoanCustomerBankDetailsObj> GetLoanCustomerBankDetailsByLoanCustomer(int loanCustomerId);
        LoanCustomerBankDetailsObj GetLoanCustomerBankDetailsByCustomer(int customerId);

        LoanCustomerIdentityRespObj UpdateLoanCustomerCardDetails(LoanCustomerCardDetailsObj entity);
        Task<LoanCustomerRespObj> UploadCustomerCardDetails(List<byte[]> record, string createdBy);
        LoanCustomerCardDetailsObj GetLoanCustomerCardDetailsByLoanCustomer(int loanCustomerId);

        bool UpdateLoanCustomerDirectorShareHolder(LoanCustomerDirectorShareHolderObj entity);
        IEnumerable<LoanCustomerDirectorShareHolderObj> GetAllLoanCustomerDirectorShareHolder();
        LoanCustomerDirectorShareHolderObj GetLoanCustomerDirectorShareHolder(int loanCustomerDirectorShareHolderId);
        bool DeleteLoanCustomerDirectorShareHolder(int loanCustomerDirectorShareHolderId);
        IEnumerable<LoanCustomerDirectorShareHolderObj> GetLoanCustomerDirectorShareHolderByLoanCustomer(int loanCustomerId);
        LoanCustomerDirectorShareHolderObj GetLoanCustomerDirectorShareHolderByCustomer(int customerId);

        IEnumerable<StartLoanApplicationCustomerObj> GetStartLoanApplicationCustomer();

        bool AddUpdateDocumentType(DocumentTypeObj entity);
        bool UploadDocumentType(byte[] record, string createdBy);
        byte[] GenerateExportDocumentType();
        IEnumerable<DocumentTypeObj> GetAllDocumentType();
        bool DeleteDocumentType(int documentTypeId);

        Task<IEnumerable<LoanCustomerObj>> GetLoanCustomersAsync();
        Task<IEnumerable<credit_loancustomerdocument>> GetCustomerDocumentsAsync(int customerId);
    }
}
