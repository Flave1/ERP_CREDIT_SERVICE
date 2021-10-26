using Banking.Contracts.Response.Credit;
using Banking.Data;
using Banking.DomainObjects.Credit;
using Banking.Helpers;
using Banking.Requests;
using GOSLibraries.GOS_API_Response;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Banking.Contracts.Response.Credit.CollateralCustomerObjs;
using static Banking.Contracts.Response.Credit.CollateralTypeObjs;
using static Banking.Contracts.Response.Credit.LoanApplicationCollateralObjs;
using static Banking.Contracts.Response.Credit.LoanObjs;

namespace Banking.Repository.Implement.Credit.Collateral
{
    public class CollateralRepository : ICollateralRepository
    {
        private readonly DataContext _dataContext;
        private readonly IIdentityServerRequest _serverRequest;
        public CollateralRepository(DataContext dataContext, IIdentityServerRequest serverRequest)
        {
            _dataContext = dataContext;
            _serverRequest = serverRequest;
        }

        #region COLLATERAL_TYPE

        public bool AddUpdateCollateralType(CollateralTypeObj entity)
        {
            try
            {
                if (entity == null) return false;
                credit_collateraltype collateralType = null;
                if (entity.CollateralTypeId > 0)
                {
                    collateralType = _dataContext.credit_collateraltype.Find(entity.CollateralTypeId);
                    if (collateralType != null)
                    {
                        collateralType.Details = entity.Details;
                        collateralType.RequireInsurancePolicy = entity.RequireInsurancePolicy;
                        collateralType.Name = entity.Name;
                        collateralType.HairCut = entity.HairCut;
                        collateralType.ValuationCycle = entity.ValuationCycle;
                        collateralType.AllowSharing = entity.AllowSharing;
                        collateralType.Active = true;
                        collateralType.Deleted = false;
                        collateralType.CreatedBy = entity.CreatedBy;
                        collateralType.CreatedOn = DateTime.Now;
                        collateralType.UpdatedBy = entity.CreatedBy;
                        collateralType.UpdatedOn = DateTime.Now;
                    }
                }
                else
                {
                    collateralType = new credit_collateraltype
                    {
                        Details = entity.Details,
                        RequireInsurancePolicy = entity.RequireInsurancePolicy,
                        Name = entity.Name,
                        HairCut = entity.HairCut,
                        ValuationCycle = entity.ValuationCycle,
                        AllowSharing = entity.AllowSharing,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = entity.CreatedBy,
                        UpdatedOn = DateTime.Now,
                    };
                    _dataContext.credit_collateraltype.Add(collateralType);
                }

                var response = _dataContext.SaveChanges() > 0;

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteCollateralType(int collateralTypeId)
        {
            var itemToDelete = _dataContext.credit_collateraltype.Find(collateralTypeId);
            itemToDelete.Deleted = true;
            _dataContext.Entry(itemToDelete).CurrentValues.SetValues(itemToDelete);
            return _dataContext.SaveChanges() > 0;
        }

        public byte[] GenerateExportCollateralType()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Details");
            dt.Columns.Add("RequireInsurancePolicy");
            dt.Columns.Add("AllowSharing");
            dt.Columns.Add("HairCut");
            dt.Columns.Add("ValuationCycle");

            var collateraltype = (from a in _dataContext.credit_collateraltype
                                  where a.Deleted == false
                                  select
                                  new CollateralTypeObj
                                  {
                                      CollateralTypeId = a.CollateralTypeId,
                                      Name = a.Name,
                                      Details = a.Details,
                                      RequireInsurancePolicy = a.RequireInsurancePolicy,
                                      HairCut = a.HairCut,
                                      ValuationCycle = a.ValuationCycle,
                                      AllowSharing = a.AllowSharing,
                                  }).ToList();

            foreach (var kk in collateraltype)
            {
                var row = dt.NewRow();
                row["Name"] = kk.Name;
                row["Details"] = kk.Details;
                row["RequireInsurancePolicy"] = kk.RequireInsurancePolicy;
                row["AllowSharing"] = kk.AllowSharing;
                row["HairCut"] = kk.HairCut;
                row["ValuationCycle"] = kk.ValuationCycle;

                dt.Rows.Add(row);
            }
            Byte[] fileBytes = null;

            if (collateraltype != null)
            {
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("collateraltype");
                    ws.DefaultColWidth = 20;
                    ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                    fileBytes = pck.GetAsByteArray();
                }
            }
            return fileBytes;
        }
        public List<CollateralTypeObj> GetAllCollateralType()
        {
            try
            {
                var collateraltype = (from a in _dataContext.credit_collateraltype
                                      where a.Deleted == false
                                      select

                                      new CollateralTypeObj
                                      {
                                          CollateralTypeId = a.CollateralTypeId,
                                          Name = a.Name,
                                          Details = a.Details,
                                          RequireInsurancePolicy = a.RequireInsurancePolicy,
                                          HairCut = a.HairCut,
                                          ValuationCycle = a.ValuationCycle,
                                          AllowSharing = a.AllowSharing,
                                      }).ToList();

                return collateraltype;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public CollateralTypeObj GetCollateralType(int collateralTypeId)
        {
            try
            {
                var collateraltype = (from a in _dataContext.credit_collateraltype
                                      where a.Deleted == false && a.CollateralTypeId == collateralTypeId
                                      select

                                     new CollateralTypeObj
                                     {
                                         CollateralTypeId = a.CollateralTypeId,
                                         Name = a.Name,
                                         Details = a.Details,
                                         RequireInsurancePolicy = a.RequireInsurancePolicy,
                                         HairCut = a.HairCut,
                                         ValuationCycle = a.ValuationCycle,
                                         AllowSharing = a.AllowSharing,
                                     }).FirstOrDefault();

                return collateraltype;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool UploadCollateralType(List<byte[]> record, string createdBy)
        {
            try
            {
                if (record == null) return false;
                List<credit_collateraltype> uploadedRecord = new List<credit_collateraltype>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                foreach (var byteItem in record)
                {
                    using (MemoryStream stream = new MemoryStream(byteItem))
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        //Use first sheet by default
                        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                        int totalRows = workSheet.Dimension.Rows;
                        //First row is considered as the header
                        for (int i = 2; i <= totalRows; i++)
                        {
                            int.TryParse(workSheet.Cells[i, 5].Value.ToString(), out int hairCut);
                            int.TryParse(workSheet.Cells[i, 6].Value.ToString(), out int valuationCycle);

                            uploadedRecord.Add(new credit_collateraltype
                            {
                                Name = workSheet.Cells[i, 1].Value.ToString(),
                                Details = workSheet.Cells[i, 2].Value.ToString(),
                                RequireInsurancePolicy = bool.Parse(workSheet.Cells[i, 3].Value.ToString()),
                                AllowSharing = bool.Parse(workSheet.Cells[i, 4].Value.ToString()),
                                HairCut = hairCut,
                                ValuationCycle = valuationCycle,
                            });
                        }
                    }
                }
                if (uploadedRecord.Count > 0)
                {
                    foreach (var item in uploadedRecord)
                    {
                        var category = _dataContext.credit_collateraltype.Where(x => x.Name == item.Name && x.Deleted == false).FirstOrDefault();
                        if (category != null)
                        {
                            category.Name = item.Name;
                            category.Details = item.Details;
                            category.RequireInsurancePolicy = item.RequireInsurancePolicy;
                            category.AllowSharing = item.AllowSharing;
                            category.HairCut = item.HairCut;
                            category.ValuationCycle = item.ValuationCycle;
                            category.Active = true;
                            category.Deleted = false;
                            category.UpdatedBy = createdBy;
                            category.UpdatedOn = DateTime.Now;
                        }
                        else
                        {
                            var structure = new credit_collateraltype
                            {
                                CollateralTypeId = item.CollateralTypeId,
                                Name = item.Name,
                                Details = item.Details,
                                RequireInsurancePolicy = item.RequireInsurancePolicy,
                                Active = true,
                                Deleted = false,
                                CreatedBy = createdBy,
                                CreatedOn = DateTime.Now,
                            };
                            _dataContext.credit_collateraltype.Add(structure);
                        }
                    }
                }

                var response = _dataContext.SaveChanges() > 0;
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //public async Task<bool> UploadCollateralTypeAsync(List<byte[]> record, string createdBy)
        //{
        //    try
        //    {
        //        if (record == null) return false;
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //        List<credit_fee> uploadedRecord = new List<credit_fee>();
        //        if (record.Count() > 0)
        //        {
        //            foreach (var byteItem in record)
        //            {
        //                using (MemoryStream stream = new MemoryStream(byteItem))
        //                using (ExcelPackage excelPackage = new ExcelPackage(stream))
        //                {
        //                    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
        //                    int totalRows = workSheet.Dimension.Rows;

        //                    for (int i = 2; i <= totalRows; i++)
        //                    {
        //                        var item = new credit_fee
        //                        {
        //                            FeeName = workSheet.Cells[i, 1].Value.ToString(),
        //                            IsIntegral = bool.Parse(workSheet.Cells[i, 2].Value.ToString()),
        //                        };
        //                        uploadedRecord.Add(item);
        //                    }
        //                }
        //            }

        //        }
        //        if (uploadedRecord.Count > 0)
        //        {
        //            foreach (var item in uploadedRecord)
        //            {
        //                var category = _dataContext.credit_fee.Where(x => x.FeeName == item.FeeName && x.Deleted == false).FirstOrDefault();
        //                if (category != null)
        //                {
        //                    category.FeeName = item.FeeName;
        //                    category.IsIntegral = (bool)item.IsIntegral;
        //                    category.Active = true;
        //                    category.Deleted = false;
        //                    category.UpdatedBy = createdBy;
        //                    category.UpdatedOn = DateTime.Now;
        //                }
        //                else
        //                {
        //                    var structure = new credit_fee
        //                    {
        //                        FeeName = item.FeeName,
        //                        IsIntegral = (bool)item.IsIntegral,
        //                        Active = true,
        //                        Deleted = false,
        //                        CreatedBy = createdBy,
        //                        CreatedOn = DateTime.Now,
        //                    };
        //                    //structures.Add(structure);
        //                    await _dataContext.credit_fee.AddAsync(structure);
        //                }
        //            }
        //        }

        //        var response = _dataContext.SaveChanges() > 0;
        //        return response;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public async Task<IList<CollateralTypeObj>> GetAllowableCollateralByLoanApplicationId(int loanApplicationId)
        {
            var loanApplication = await _dataContext.credit_loanapplication.FirstOrDefaultAsync(x => x.LoanApplicationId == loanApplicationId);
            var allowableCollateralTypes = await _dataContext.cor_allowable_collateraltype.Where(x => x.ProductId == loanApplication.ProposedProductId).ToListAsync();
            var allowableCollateralTypesIds = allowableCollateralTypes.Select(x => x.CollateralTypeId);
            var collateralTypes = await _dataContext.credit_collateraltype.Where(x => allowableCollateralTypesIds.Contains(x.CollateralTypeId) && x.Deleted != true).ToListAsync();
            var collateralTypesViewModel = new List<CollateralTypeObj>();

            foreach (var collateralType in collateralTypes)
            {
                collateralTypesViewModel.Add(new CollateralTypeObj
                {
                    Name = collateralType.Name,
                    CollateralTypeId = collateralType.CollateralTypeId,
                    ValuationCycle = collateralType.ValuationCycle,
                    AllowSharing = collateralType.AllowSharing,
                    HairCut = collateralType.HairCut,
                    RequireInsurancePolicy = collateralType.RequireInsurancePolicy
                });
            }
            return collateralTypesViewModel;
        }

        #endregion


        #region COLLATERAL_CUSTOMER
        public IEnumerable<CollateralCustomerObj> GetCollateralSingleCustomer(int customerId)
        {
            try
            {
                var collateral = (from a in _dataContext.credit_collateralcustomer
                                  join b in _dataContext.credit_loanapplicationcollateraldocument on a.CollateralCustomerId equals b.CollateralCustomerId
                                  where a.Deleted == false && a.CustomerId == customerId && b.Deleted == false
                                  select
                                      new CollateralCustomerObj
                                      {
                                          CollateralCustomerId = a.CollateralCustomerId,
                                          CustomerId = a.CustomerId,
                                          CustomerName = _dataContext.credit_loancustomer.FirstOrDefault(x=>x.CustomerId == a.CustomerId).FirstName+ " " + _dataContext.credit_loancustomer.FirstOrDefault(x => x.CustomerId == a.CustomerId).LastName,
                                          CollateralTypeId =  a.CollateralTypeId,
                                          CollateralTypeName = _dataContext.credit_collateraltype.FirstOrDefault(x => x.CollateralTypeId == a.CollateralTypeId).Name,
                                          CurrencyId = a.CurrencyId,
                                          //Currency = a.cor_currency.CurrencyName,
                                          CollateralVerificationStatus = a.CollateralVerificationStatus,
                                          CollateralValue = a.CollateralValue,
                                          Location = a.Location,
                                          CollateralCode = a.CollateralCode,
                                          DocumentName = b.DocumentName,
                                      }).ToList();

                return collateral;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public CollateralCustomerObj GetCollateralCustomer(int collateralCustomerId)
        {
            try
            {
                var collateral = (from a in _dataContext.credit_collateralcustomer
                                  where a.Deleted == false && a.CollateralCustomerId == collateralCustomerId
                                  select

                                      new CollateralCustomerObj
                                      {
                                          CollateralCustomerId = a.CollateralCustomerId,
                                          CustomerId = a.CustomerId,
                                          CustomerName = _dataContext.credit_loancustomer.FirstOrDefault(x => x.CustomerId == a.CustomerId).FirstName + " " + _dataContext.credit_loancustomer.FirstOrDefault(x => x.CustomerId == a.CustomerId).LastName,
                                          CollateralTypeId = a.CollateralTypeId,
                                          CollateralTypeName = _dataContext.credit_collateraltype.FirstOrDefault(x => x.CollateralTypeId == a.CollateralTypeId).Name,
                                          CurrencyId = a.CurrencyId,
                                          //Currency = a.cor_currency.CurrencyName,
                                          CollateralVerificationStatus = a.CollateralVerificationStatus,
                                          CollateralValue = a.CollateralValue,
                                          Location = a.Location,
                                          CollateralCode = a.CollateralCode,
                                      }).FirstOrDefault();
                return collateral;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<int> AddUpdateCustomerCollateral(credit_collateralcustomer model)
        {
            try
            {
                var creditCollateralCustomerId = 0;
                var collateralCode = GenerateCollateralCode(model.CollateralTypeId);
                model.CollateralCode = collateralCode;
                if (model.CollateralCustomerId > 0)
                {
                    var itemToUpdate = await _dataContext.credit_collateralcustomer.FindAsync(model.CollateralCustomerId);
                    _dataContext.Entry(itemToUpdate).CurrentValues.SetValues(model);
                }
                else
                    _dataContext.credit_collateralcustomer.Add(model);
                await _dataContext.SaveChangesAsync();
                creditCollateralCustomerId = model.CollateralCustomerId;
                return creditCollateralCustomerId;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteCollateralCutomer(int collateralCustomerId)
        {
            var itemToDelete =  _dataContext.credit_collateralcustomer.Find(collateralCustomerId);
            itemToDelete.Deleted = true;
            _dataContext.Entry(itemToDelete).CurrentValues.SetValues(itemToDelete);
            return  _dataContext.SaveChanges() > 0;
        }
        public LoanChequeRespObj GenerateExportCustomerCollateral(int collateralCustomerId)
        {
            var collateralDocument = _dataContext.credit_loanapplicationcollateraldocument.FirstOrDefault(x=>x.CollateralCustomerId == collateralCustomerId);
            if (collateralDocument != null)
            {
                if (collateralDocument.DocumentName != null && collateralDocument.DocumentName != "")
                {
                    var val2 = collateralDocument.DocumentName.Split(".")[1];
                    return new LoanChequeRespObj
                    {
                        FileExtension = "." + val2,
                        FileName = collateralDocument.DocumentName,
                        Export = collateralDocument.Document,
                        Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Successful" } }
                    };
                }
            }

            return new LoanChequeRespObj
            {
                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Document was not uploaded" } }
            };
        }

        public List<CollateralCustomerObj> GetAllCollateralCustomer()
        {
            try
            {
                var collateral = (from a in _dataContext.credit_collateralcustomer
                                  where a.Deleted == false
                                  select

                                 new CollateralCustomerObj
                                 {
                                     CollateralCustomerId = a.CollateralCustomerId,
                                     CustomerId = a.CustomerId,
                                     CustomerName = a.credit_loancustomer.FirstName + " " + a.credit_loancustomer.LastName,
                                     CollateralTypeId = a.CollateralTypeId,
                                     CollateralTypeName = a.credit_collateraltype.Name,
                                     CurrencyId = a.CurrencyId,
                                     //Currency = a.cor_currency.CurrencyName,
                                     CollateralVerificationStatus = a.CollateralVerificationStatus,
                                     CollateralValue = a.CollateralValue,
                                     Location = a.Location,
                                     CollateralCode = a.CollateralCode,
                                 }).ToList();

                return collateral;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> GenerateExportCollateralCustomers()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CustomerEmail");
            dt.Columns.Add("LoanReferenceNumber");
            dt.Columns.Add("CollateralCode");
            dt.Columns.Add("CollateralTypeName");
            dt.Columns.Add("CollateralValue");
            dt.Columns.Add("ActualCollateralValue");
            dt.Columns.Add("Currency");
            dt.Columns.Add("Location");

            var getCurrencyList = await _serverRequest.GetCurrencyAsync();

            var collateralCustomer = (from a in _dataContext.credit_collateralcustomer 
                                      join b in _dataContext.credit_collateralcustomerconsumption on a.CollateralCustomerId equals b.CollateralCustomerId
                                  where a.Deleted == false
                                  select
                                  new CollateralCustomerObj
                                  {
                                      CustomerEmail = _dataContext.credit_loancustomer.FirstOrDefault(x=>x.CustomerId == a.CustomerId).Email,
                                      LoanReferenceNumber = _dataContext.credit_loan.FirstOrDefault(x=>x.LoanApplicationId == b.LoanApplicationId).LoanRefNumber,
                                      CollateralCode = a.CollateralCode,
                                      CollateralTypeName = _dataContext.credit_collateraltype.FirstOrDefault(x=>x.CollateralTypeId == a.CollateralTypeId).Name,
                                      CollateralValue = a.CollateralValue,
                                      ActualCollateralValue = b.ActualCollateralValue,
                                      CurrencyId = a.CurrencyId,
                                      Location = a.Location
                                  }).ToList();

            foreach (var kk in collateralCustomer)
            {           
                var currency = getCurrencyList.commonLookups.Where(x => x.LookupId == kk.CurrencyId).FirstOrDefault();
                if (currency != null)
                {
                    kk.Currency = currency.LookupName;
                }

                var row = dt.NewRow();
                row["CustomerEmail"] = kk.CustomerEmail;
                row["LoanReferenceNumber"] = kk.LoanReferenceNumber;
                row["CollateralCode"] = kk.CollateralCode;
                row["CollateralTypeName"] = kk.CollateralTypeName;
                row["CollateralValue"] = kk.CollateralValue;
                row["ActualCollateralValue"] = kk.ActualCollateralValue;
                row["Currency"] = kk.Currency;
                row["Location"] = kk.Location;

                dt.Rows.Add(row);
            }
            Byte[] fileBytes = null;

            if (collateralCustomer != null)
            {
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("collateralCustomer");
                    ws.DefaultColWidth = 20;
                    ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                    fileBytes = pck.GetAsByteArray();
                }
            }
            return fileBytes;
        }

        public async Task<CollateralCustomerRegRespObj> UploadCustomerCollateral(List<byte[]> record, string createdBy)
        {
            try
            {
                if (record == null) return new CollateralCustomerRegRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid Payload" } }
                };
                var response = new CollateralCustomerRegRespObj { Status = new APIResponseStatus { Message = new APIResponseMessage() } };
                List<CollateralCustomerObj> uploadedRecord = new List<CollateralCustomerObj>();
                credit_collateralcustomer collateralcustomer = new credit_collateralcustomer();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                foreach (var byteItem in record)
                {
                    using (MemoryStream stream = new MemoryStream(byteItem))
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        //Use first sheet by default
                        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                        int totalRows = workSheet.Dimension.Rows;
                        //First row is considered as the header
                        for (int i = 2; i <= totalRows; i++)
                        {
                            //bool.TryParse(workSheet.Cells[i, 5].Value.ToString(), out bool collateralVerificationStatus);

                            uploadedRecord.Add(new CollateralCustomerObj
                            {
                                Excel_line_number = i,
                                CustomerEmail = workSheet.Cells[i, 1].Value.ToString() != null ? workSheet.Cells[i, 1].Value.ToString() : "",
                                LoanReferenceNumber = workSheet.Cells[i, 2].Value.ToString() != null ? workSheet.Cells[i, 2].Value.ToString() : "",
                                CollateralCode = workSheet.Cells[i, 3].Value.ToString(),
                                CollateralTypeName = workSheet.Cells[i, 4].Value.ToString(),
                                CollateralValue = decimal.Parse(workSheet.Cells[i, 5].Value.ToString()),
                                ActualCollateralValue = decimal.Parse(workSheet.Cells[i, 6].Value.ToString()),
                                Currency = workSheet.Cells[i, 7].Value.ToString(),
                                Location = workSheet.Cells[i, 8].Value.ToString(),
                            });
                        }
                    }
                }
                var getCurrencyList = await _serverRequest.GetCurrencyAsync();      
                        if (uploadedRecord.Count > 0)
                        {
                            foreach (var item in uploadedRecord)
                            {
                                if (item.CustomerEmail == "" || item.CustomerEmail == null)
                                {
                                    response.Status.Message.FriendlyMessage = $"Please include a valid customer's email detected on line {item.Excel_line_number}";
                                    return response;
                                }
                                var accountTypeExist = _dataContext.credit_loancustomer.Where(x => x.Email == item.CustomerEmail).FirstOrDefault();
                                if (accountTypeExist == null)
                                {
                                    response.Status.Message.FriendlyMessage = $"Customer Email {item.CustomerEmail} doesn't match any user, detected on line {item.Excel_line_number}";
                                    return response;
                                }
                                var currency = getCurrencyList.commonLookups.Where(x => x.LookupName.ToLower().Trim() == item.Currency.ToLower().Trim()).FirstOrDefault();
                                if (currency == null)
                                {
                                    response.Status.Message.FriendlyMessage = $"Please include a valid currency name, detected on line {item.Excel_line_number}";
                                    return response;
                                }
                                var loan = _dataContext.credit_loan.FirstOrDefault(x => x.LoanRefNumber.ToLower().Trim() == item.LoanReferenceNumber.ToLower().Trim());
                                if (loan == null)
                                {
                                    response.Status.Message.FriendlyMessage = $"Please include a valid loan reference number, detected on line {item.Excel_line_number}";
                                    return response;
                                }
                                var collateralType = _dataContext.credit_collateraltype.Where(x => x.Name.ToLower().Trim() == item.CollateralTypeName.ToLower().Trim() && x.Deleted == false).FirstOrDefault();
                                if (collateralType == null)
                                {
                                    response.Status.Message.FriendlyMessage = $"Please include a valid collateralType name, detected on line {item.Excel_line_number}";
                                    return response;
                                }
                                item.CollateralTypeId = collateralType.CollateralTypeId;
                        if (item.CollateralCode.ToLower().Trim() == "nil")
                        {
                            var CollateralCode = GenerateCollateralCode(item.CollateralTypeId);
                            collateralcustomer = new credit_collateralcustomer
                            {
                                CustomerId = accountTypeExist.CustomerId,
                                CollateralTypeId = item.CollateralTypeId,
                                CurrencyId = item.CurrencyId,
                                LoanApplicationId = loan.LoanApplicationId,
                                CollateralValue = item.CollateralValue,
                                CollateralVerificationStatus = true,
                                Location = item.Location,
                                CollateralCode = CollateralCode,
                                Active = true,
                                Deleted = false,
                                CreatedBy = createdBy,
                                CreatedOn = DateTime.Now,
                            };
                            _dataContext.credit_collateralcustomer.Add(collateralcustomer);
                            _dataContext.SaveChanges();

                            var consumption = new credit_collateralcustomerconsumption
                            {
                                CollateralCustomerId = collateralcustomer.CollateralCustomerId,
                                CustomerId = accountTypeExist.CustomerId,
                                LoanApplicationId = loan.LoanApplicationId,
                                Amount = item.ActualCollateralValue,
                                Active = true,
                                Deleted = false,
                                CreatedBy = createdBy,
                                CreatedOn = DateTime.Now,
                            };
                            await _dataContext.credit_collateralcustomerconsumption.AddAsync(consumption);
                            _dataContext.SaveChanges();
                        }
                        else
                        {
                            collateralcustomer = _dataContext.credit_collateralcustomer.FirstOrDefault(x => x.CollateralCode.ToLower().Trim() == item.CollateralCode.ToLower().Trim());
                            if (collateralcustomer != null)
                            {
                                collateralcustomer.CustomerId = accountTypeExist.CustomerId;
                                collateralcustomer.CollateralTypeId = item.CollateralTypeId;
                                collateralcustomer.CurrencyId = item.CurrencyId;
                                collateralcustomer.LoanApplicationId = loan.LoanApplicationId;
                                collateralcustomer.CollateralValue = item.CollateralValue;
                                collateralcustomer.CollateralVerificationStatus = true;
                                collateralcustomer.Location = item.Location;
                                collateralcustomer.UpdatedBy = createdBy;
                                collateralcustomer.UpdatedOn = DateTime.Now;
                            }

                            var consumption = _dataContext.credit_collateralcustomerconsumption.FirstOrDefault(x => x.LoanApplicationId == loan.LoanApplicationId);
                            if (consumption != null)
                            {
                                consumption.CollateralCustomerId = collateralcustomer.CollateralCustomerId;
                                consumption.CustomerId = accountTypeExist.CustomerId;
                                consumption.LoanApplicationId = loan.LoanApplicationId;
                                consumption.Amount = item.ActualCollateralValue;
                            }
                            _dataContext.SaveChanges();
                        }


                            }
                        }
                response.Status.Message.FriendlyMessage = $"Records Uploaded Successfully";
                response.Status.IsSuccessful = true;
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<CollateralCustomerObj> GetAllCustomerCollateralByLoanApplication(int loanApplicationId, bool includeNotAllowSharing)
        {
            try
            {
                var loanCustomer = _dataContext.credit_loanapplication.Find(loanApplicationId);
                var collateral = (from a in _dataContext.credit_collateralcustomer
                                  where a.Deleted == false && a.CustomerId == loanCustomer.CustomerId
                                  select
                                  new CollateralCustomerObj
                                  {
                                      CollateralCustomerId = a.CollateralCustomerId,
                                      CustomerId = a.CustomerId,
                                      CustomerName = _dataContext.credit_loancustomer.FirstOrDefault(x => x.CustomerId == a.CustomerId).FirstName + " " + _dataContext.credit_loancustomer.FirstOrDefault(x => x.CustomerId == a.CustomerId).LastName,
                                      CollateralTypeId = a.CollateralTypeId,
                                      CollateralTypeName = _dataContext.credit_collateraltype.FirstOrDefault(x => x.CollateralTypeId == a.CollateralTypeId).Name,
                                      CurrencyId = a.CurrencyId,
                                      //Currency = a.cor_currency.CurrencyName,
                                      CollateralVerificationStatus = a.CollateralVerificationStatus,
                                      CollateralValue = a.CollateralValue,
                                      LoanApplicationId = loanApplicationId,
                                      Location = a.Location,
                                      CollateralCode = a.CollateralCode,
                                  }).ToList();

                var returnModelCollateral = new List<CollateralCustomerObj>();

                foreach (var col in collateral)
                {
                    var collateralType = _dataContext.credit_collateraltype.FirstOrDefault(x => x.CollateralTypeId == col.CollateralTypeId);

                    if (collateralType.AllowSharing == true)
                    {
                        returnModelCollateral.Add(new CollateralCustomerObj
                        {
                            CollateralCustomerId = col.CollateralCustomerId,
                            CustomerId = col.CustomerId,
                            CustomerName = col.CustomerName,
                            CollateralTypeId = col.CollateralTypeId,
                            CollateralTypeName = col.CollateralTypeName,
                            CurrencyId = col.CurrencyId,
                            Currency = col.Currency,
                            CollateralVerificationStatus = col.CollateralVerificationStatus,
                            CollateralValue = col.CollateralValue,
                            LoanApplicationId = col.LoanApplicationId,
                            Location = col.Location,
                            CollateralCode = col.CollateralCode,
                        });
                    }
                    else
                    {
                        var exist = _dataContext.credit_collateralcustomerconsumption
                            .FirstOrDefault(x => x.CollateralCustomerId == col.CollateralCustomerId && x.Deleted == false);

                        if (exist == null || (exist != null && includeNotAllowSharing))
                        {
                            returnModelCollateral.Add(new CollateralCustomerObj
                            {
                                CollateralCustomerId = col.CollateralCustomerId,
                                CustomerId = col.CustomerId,
                                CustomerName = col.CustomerName,
                                CollateralTypeId = col.CollateralTypeId,
                                CollateralTypeName = col.CollateralTypeName,
                                CurrencyId = col.CurrencyId,
                                Currency = col.Currency,
                                CollateralVerificationStatus = col.CollateralVerificationStatus,
                                CollateralValue = col.CollateralValue,
                                LoanApplicationId = col.LoanApplicationId,
                                Location = col.Location,
                                CollateralCode = col.CollateralCode,
                            });
                        }
                    }
                }

                return returnModelCollateral;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GenerateCollateralCode(int collateralTypeId)
        {
            var Code = _dataContext.credit_collateraltype.FirstOrDefault(x => x.CollateralTypeId == collateralTypeId).Name;
            var count = ((_dataContext.credit_collateralcustomer.Count(x => x.CollateralTypeId == collateralTypeId)) + 1);
            var reference = $"{Code.Substring(0, 3).ToUpper()}-{GeneralHelpers.GenerateZeroString(5) + count.ToString().PadRight(5)}";

            return reference;
        }
     
        #endregion


        #region LOAN_APPLICATION_COLLATERAL
        public bool AddUpdateLoanApplicationCollateral(LoanApplicationCollateralObj entity)
        {
            try
            {
                if (entity == null) return false;
                credit_loanapplicationcollateral loanCollateral = null;
                if (entity.LoanApplicationCollateralId > 0)
                {
                    loanCollateral = _dataContext.credit_loanapplicationcollateral.Find(entity.LoanApplicationCollateralId);
                    if (loanCollateral != null)
                    {
                        loanCollateral.CollateralCustomerId = entity.CollateralCustomerId;
                        loanCollateral.LoanApplicationId = entity.LoanApplicationId;
                        loanCollateral.ActualCollateralValue = entity.ActualCollateralValue;
                        loanCollateral.Active = true;
                        loanCollateral.Deleted = false;
                        loanCollateral.UpdatedBy = entity.CreatedBy;
                        loanCollateral.UpdatedOn = DateTime.Now;
                    }
                }
                else
                {
                    loanCollateral = new credit_loanapplicationcollateral
                    {
                        CollateralCustomerId = entity.CollateralCustomerId,
                        LoanApplicationId = entity.LoanApplicationId,
                        ActualCollateralValue = entity.ActualCollateralValue,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    _dataContext.credit_loanapplicationcollateral.Add(loanCollateral);
                }

                var response = _dataContext.SaveChanges() > 0;

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteLoanApplicationCollateral(int loanApplicationCollateralId)
        {
            var itemToDelete = _dataContext.credit_loanapplicationcollateral.Find(loanApplicationCollateralId);
            itemToDelete.Deleted = true;
            _dataContext.Entry(itemToDelete).CurrentValues.SetValues(itemToDelete);
            return _dataContext.SaveChanges() > 0;
        }

        public async Task DeleteLoanApplicationCollateralDocumentAsync(int collateralCustomerId)
        {
            var loanApplicationCollateralDocumentsDb = await _dataContext.credit_loanapplicationcollateraldocument.Where(x => x.CollateralCustomerId == collateralCustomerId).ToListAsync();

            if (loanApplicationCollateralDocumentsDb != null )
            {
                foreach (var document in loanApplicationCollateralDocumentsDb)
                {
                    document.Deleted = true;
                }

                await _dataContext.SaveChangesAsync();
            }
        }

        public IEnumerable<LoanApplicationCollateralObj> GetAllLoanApplicationCollateral()
        {
            try
            {
                var loancollateral = (from a in _dataContext.credit_loanapplicationcollateral
                                      where a.Deleted == false
                                      select

                                      new LoanApplicationCollateralObj
                                      {
                                          LoanApplicationCollateralId = a.LoanApplicationCollateralId,
                                          CollateralCustomerId = a.CollateralCustomerId,
                                          LoanApplicationId = a.LoanApplicationId,
                                          LoanApplicationRefNo = a.credit_loanapplication.ApplicationRefNumber,
                                          ActualCollateralValue = a.ActualCollateralValue,
                                      }).ToList();

                return loancollateral;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<LoanApplicationCollateralObj> GetLoanApplicationCollateralForLoanApplicationId(int loanApplicationId)
        {
            try
            {
                var loanCollateral = (from a in _dataContext.credit_collateralcustomerconsumption
                                      join b in _dataContext.credit_collateralcustomer on a.CollateralCustomerId equals b.CollateralCustomerId
                                      join c in _dataContext.credit_loanapplication on a.LoanApplicationId equals c.LoanApplicationId
                                      join d in _dataContext.credit_product on c.ApprovedProductId equals d.ProductId
                                      where a.Deleted == false && a.LoanApplicationId == loanApplicationId
                                      select

                                      new LoanApplicationCollateralObj
                                      {
                                          CollateralCustomerConsumptionId = a.CollateralCustomerConsumptionId,
                                          CollateralCustomerId = a.CollateralCustomerId,
                                          LoanApplicationId = a.LoanApplicationId,
                                          ActualCollateralValue = a.Amount,
                                          CollateralValue = (decimal)d.CollateralPercentage / 100 * c.ApprovedAmount,
                                          CollateralTypeName = _dataContext.credit_collateraltype.FirstOrDefault(x=>x.CollateralTypeId == b.CollateralTypeId).Name, //b.credit_collateraltype.Name,
                                          CollateralCode = b.CollateralCode
                                      }).ToList();

                return loanCollateral;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<LoanApplicationCollateralObj> GetLoanApplicationCollateral(int loanApplicationId)
        {
            try
            {
                var loancollateral = (from a in _dataContext.credit_loanapplicationcollateral
                                      where a.Deleted == false
                                      select

                                      new LoanApplicationCollateralObj
                                      {
                                          LoanApplicationCollateralId = a.LoanApplicationCollateralId,
                                          CollateralCustomerId = a.CollateralCustomerId,
                                          LoanApplicationId = a.LoanApplicationId,
                                          LoanApplicationRefNo = a.credit_loanapplication.ApplicationRefNumber,
                                          ActualCollateralValue = a.ActualCollateralValue,
                                      }).ToList();

                return loancollateral;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public decimal GetLoanApplicationRequireAmount(int loanApplicationId)
        {
            try
            {
                var loanCollateral = (from c in _dataContext.credit_loanapplication
                                      join d in _dataContext.credit_product on c.ApprovedProductId equals d.ProductId
                                      where c.Deleted == false && c.LoanApplicationId == loanApplicationId
                                      select

                                      new LoanApplicationCollateralObj
                                      {
                                          CollateralValue = (decimal)d.CollateralPercentage / 100 * c.ApprovedAmount,
                                      }).FirstOrDefault();

                return loanCollateral.CollateralValue;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

    }
}
