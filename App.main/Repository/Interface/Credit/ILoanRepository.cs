using Banking.Contracts.Response.Approvals;
using Banking.DomainObjects.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Banking.Contracts.Response.Credit.ApprovalObjs;
using static Banking.Contracts.Response.Credit.LoanApplicationObjs;
using static Banking.Contracts.Response.Credit.LoanObjs;
using static Banking.Contracts.Response.Credit.LoanScheduleObjs;

namespace Banking.Repository.Interface.Credit
{
    public interface ILoanRepository
    {
        Task<ApprovalRegRespObj> Disburse_Loan_By_Upload(int targetId);
        Task<LoanRespObj> AddLoanBooking(LoanObj entity);
        Task<LoanRespObj> UploadLoan(byte[] record, string createdBy);
        Task<byte[]> GenerateExportLoan();
        Task<LoanRespObj> GetLoanBookingAwaitingApproval(string userName);
        IEnumerable<LoanApplicationObj> GetAllLoanApplicationOfferLetterReviewed();
        IEnumerable<LoanApplicationObj> GetAllLoanApplicationOfferLetterReviewedById(int id);
        Task<LoanListObj> GetLoanDetailInformation(int loanId);
        IEnumerable<LoanListObj> GetManagedLoanInformation(int loanId);
        int GoForApproval(ApprovalObj entity);
        IEnumerable<LoanListObj> GetPaymentDueLoanInformation();
        IEnumerable<LoanListObj> GetPastDueLoanInformation();
        LoanPaymentScheduleInputObj GetScheduleInput(int loanApplicationId);
        ApprovalRegRespObj DisburseLoan(int targetId, int staffId, string createdBy, string comment);
        ApprovalRegRespObj DisburseLoanByUpload(int targetId, int staffId, string createdBy);
        LoanRepaymentRespObj repaymentWithFlutterWave(repaymentObj model);
        LoanRepaymentRespObj repaymentZeroWithFlutterWave(repaymentObj model);
        IEnumerable<credit_loan> GetAllCreditLoan(); 
        //Task<string> GenerateLoanReferenceNumber(int productId);


        #region Credit Loan Comment
        bool UpdateCreditLoanComment(CreditLoanCommentObj entity);
        IEnumerable<CreditLoanCommentObj> GetAllCreditLoanComment(int loanId, int loanscheduleId);
        CreditLoanCommentObj GetCreditLoanComment(int loanId, int loanscheduleId, int commentId);
        bool DeleteCreditLoanComment(int id);
        #endregion

        #region Credit Loan Decision
        bool UpdateCreditLoanDecision(CreditLoanDecisionObj entity);
        IEnumerable<CreditLoanDecisionObj> GetAllCreditLoanDecision(int loanId, int loanscheduleId);
        CreditLoanDecisionObj GetCreditLoanDecision(int loanId, int loanscheduleId);
        bool DeleteCreditLoanDecision(int id);
        #endregion

        Task<credit_offerletter> GetOfferLetterByLoanApplicationAsync(int applicationId);
        LoanChequeRespObj GetAllLoanChequeList(int loanChequeId);
        bool AddUpdateLoanCheque(loan_cheque_obj entity);
        LoanChequeRespObj DownloadCheque(loan_cheque_obj entity);
        LoanChequeRespObj UploadSingleCheque(loan_cheque_obj entity);
        LoanChequeRespObj UpdateChequeStatus(loan_cheque_obj entity);
        Task<LoanChequeRespObj> UploadChequeAmount(List<byte[]> record, int loanId);
        LoanChequeRespObj UpdateChequeAmount(loan_cheque_obj entity);
    }
}
