using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Enum
{
    public enum LoanReportEnum
    {
        LoanNo = 1,
        ProductName = 2,
        AccountNumber = 3,
        AccountName = 4,
        Industry = 5,
        DisbursementDate = 6,
        Maturity = 7,
        ApplicationAmount = 8,
        DisbursementAmount = 9,
        Tenor = 10,
        Rate = 11,
        TotalInterest = 12,
        DaysInOverdue = 13,
        LoanApplicationRef = 14,
        Fee = 15,
        AmortizedBalance = 16,
        LoanBalance = 17,
        LateRepaymentCharge = 18,
        InterestRecievable = 19,
        PD = 20,
        CreditScore = 21,
        OutstandingTenor = 22,
        EIR = 23,
        Collateral_YES_OR_NO = 24,
        CollateralPercentage = 25,
        Restructured = 26,
    }

    public enum CorporateCustomerReportEnum
    {
        CompanyName = 1,
        Email = 2,
        RegNumber = 3,
        DateofIncorporation = 4,
        PhoneNo = 5,
        Address = 6,
        PostalAddress = 7,
        Industry = 8,
        IncorporationCountry = 9,
        City = 10,
        AnnualTurnaover = 11,
        ShareholderFund = 12,
        CompanyWebsite = 13,
        BVN = 14,
        AccountNumber = 15,
        BankName = 16,
        DirectorName = 17,
        DirectorPosition = 18,
        DirectorEmail = 19,
        DirectorDOB = 20,
        DirectorPhone = 21,
        DirectorPercentageShare = 22,
        DirectorType = 23,
        DirectorPolicallyExposed = 24,
    }

    public enum IndividualCustomerReportEnum
    {
        FullName = 1,
        Gender = 2,
        Dob = 3,
        Email = 4,
        EmploymentStatus = 5,
        City = 6,
        Country = 7,
        Occupation = 8,
        PoliticallyExposed = 9,
        Phone = 10,
        MaritalStatus = 11,
        BVN = 12,
        AccountNumber = 13,
        BankName = 14,
        IDIssuer = 15,
        IDNumber = 16,
        NextOfKinName = 17,
        NextOfKinRelationship = 18,
        NextOfKinEmail = 19,
        NextOfKinPhoneNo = 20,
        NextOfKinAddress = 21
    }

}
