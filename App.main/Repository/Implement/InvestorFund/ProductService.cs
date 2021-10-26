using Banking.Contracts.Response.InvestorFund;
using Banking.Data;
using Banking.DomainObjects.InvestorFund;
using Banking.Repository.Interface.InvestorFund;
using Banking.Requests;
using GOSLibraries.GOS_API_Response;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Repository.Implement.InvestorFund
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dataContext;
        private readonly IIdentityServerRequest _serverRequest;
        public ProductService(DataContext dataContext, IIdentityServerRequest serverRequest)
        {
            _dataContext = dataContext;
            _serverRequest = serverRequest;
        }

        #region Product

        public InfProductRespObj AddUpdateProduct(InfProductObj entity)
        {
            try
            {
                if (entity == null) return new InfProductRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid request payload" } }
                };

                    var accountTypeExist = _dataContext.inf_product.FirstOrDefault(x=>x.ProductName.ToLower().Trim() == entity.ProductName.ToLower().Trim());
                    if (accountTypeExist != null)
                    {
                        accountTypeExist.ProductCode = entity.ProductCode;
                        accountTypeExist.ProductName = entity.ProductName;
                        accountTypeExist.Rate = entity.Rate;
                        accountTypeExist.ProductTypeId = entity.ProductTypeId;
                        accountTypeExist.ProductLimit = entity.ProductLimit;
                        accountTypeExist.InterestRateMax = entity.InterestRateMax;
                        accountTypeExist.InterestRepaymentTypeId = entity.InterestRepaymentTypeId;
                        accountTypeExist.ScheduleMethodId = entity.ScheduleMethodId;
                        accountTypeExist.FrequencyId = entity.FrequencyId;
                        accountTypeExist.MaximumPeriod = entity.MaximumPeriod;
                        accountTypeExist.InterestRateAnnual = entity.InterestRateAnnual;
                        accountTypeExist.InterestRateFrequency = entity.InterestRateFrequency;
                        accountTypeExist.ProductPrincipalGl = entity.ProductPrincipalGl;
                        accountTypeExist.ReceiverPrincipalGl = entity.ReceiverPrincipalGl;
                        accountTypeExist.InterstExpenseGl = entity.InterstExpenseGl;
                        accountTypeExist.InterestPayableGl = entity.InterestPayableGl;
                        accountTypeExist.TaxGl = entity.TaxGl;
                        accountTypeExist.ProductLimitId = entity.ProductLimitId;
                        accountTypeExist.EarlyTerminationCharge = entity.EarlyTerminationCharge;
                        accountTypeExist.TaxRate = entity.TaxRate;
                        accountTypeExist.Active = true;
                        accountTypeExist.Deleted = false;
                        accountTypeExist.UpdatedBy = entity.CreatedBy;
                        accountTypeExist.UpdatedOn = DateTime.Now;
                    }
                else
                {
                    var accountType = new inf_product
                    {
                        ProductCode = entity.ProductCode,
                        ProductName = entity.ProductName,
                        Rate = entity.Rate,
                        ProductTypeId = entity.ProductTypeId,
                        ProductLimit = entity.ProductLimit,
                        InterestRateMax = entity.InterestRateMax,
                        InterestRepaymentTypeId = entity.InterestRepaymentTypeId,
                        ScheduleMethodId = entity.ScheduleMethodId,
                        FrequencyId = entity.FrequencyId,
                        MaximumPeriod = entity.MaximumPeriod,
                        InterestRateAnnual = entity.InterestRateAnnual,
                        InterestRateFrequency = entity.InterestRateFrequency,
                        ProductPrincipalGl = entity.ProductPrincipalGl,
                        ReceiverPrincipalGl = entity.ReceiverPrincipalGl,
                        InterstExpenseGl = entity.InterstExpenseGl,
                        InterestPayableGl = entity.InterestPayableGl,
                        EarlyTerminationCharge = entity.EarlyTerminationCharge,
                        TaxRate = entity.TaxRate,
                        TaxGl = entity.TaxGl,
                        Active = true,
                        Deleted = false,
                        UpdatedBy = entity.CreatedBy,
                        UpdatedOn = DateTime.Now,
                    };
                    _dataContext.inf_product.Add(accountType);
                }

                var isDone = _dataContext.SaveChanges() > 0;

                return new InfProductRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = isDone ? true : false, Message = new APIResponseMessage { FriendlyMessage = isDone ? "Successful" : "Unsuccessful" } }
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteProduct(int id)
        {
            var itemToDelete = _dataContext.inf_product.Find(id);
            itemToDelete.Deleted = true;
            _dataContext.Entry(itemToDelete).CurrentValues.SetValues(itemToDelete);
            return _dataContext.SaveChanges() > 0;
        }

        public byte[] GenerateExportProduct()
        {
            var subglList = _serverRequest.GetAllSubGlAsync().Result;
            DataTable dt = new DataTable();
            dt.Columns.Add("Product Code");
            dt.Columns.Add("Product Name");
            dt.Columns.Add("Product Type");
            dt.Columns.Add("Product Limit");
            dt.Columns.Add("Interest Repayment Type");
            dt.Columns.Add("Schedule Method");
            dt.Columns.Add("Frequency");
            dt.Columns.Add("Maximum Period");
            dt.Columns.Add("Interest Rate Annual");
            dt.Columns.Add("Liquidation Charge");
            dt.Columns.Add("Tax Rate");
            dt.Columns.Add("Product Principal GL");
            dt.Columns.Add("Receiver Principal GL");
            dt.Columns.Add("Interest Payable GL");
            dt.Columns.Add("Interest Expense GL");
            dt.Columns.Add("Tax GL");
            
            var statementType = (from a in _dataContext.inf_product
                                 where a.Deleted == false
                                 select new InfProductObj
                                 {
                                     ProductCode = a.ProductCode,
                                     ProductName = a.ProductName,
                                     ProductTypeId = a.ProductTypeId,
                                     ProductLimit = a.ProductLimit,
                                     InterestRepaymentTypeId = a.InterestRepaymentTypeId,
                                     ScheduleMethodId = a.ScheduleMethodId,
                                     FrequencyId = a.FrequencyId,
                                     MaximumPeriod = a.MaximumPeriod,
                                     InterestRateAnnual = a.InterestRateAnnual,
                                     EarlyTerminationCharge = a.EarlyTerminationCharge,
                                     ProductPrincipalGl = a.ProductPrincipalGl,
                                     ReceiverPrincipalGl = a.ReceiverPrincipalGl,
                                     InterestPayableGl = a.InterestPayableGl,
                                     InterstExpenseGl = a.InterstExpenseGl,
                                     TaxGl = a.TaxGl,
                                     TaxRate = a.TaxRate
                                 }).ToList();
            foreach (var kk in statementType)
            {
                var frequency = _dataContext.credit_frequencytype.FirstOrDefault(x => x.FrequencyTypeId == kk.FrequencyId);
                var producttype = _dataContext.inf_producttype.FirstOrDefault(x => x.ProductTypeId == kk.ProductTypeId);
                var interestRepaymentType = _dataContext.credit_repaymenttype.FirstOrDefault(x => x.RepaymentTypeId == kk.InterestRepaymentTypeId);
                var scheduleMethod = _dataContext.credit_loanscheduletype.FirstOrDefault(x => x.LoanScheduleTypeId == kk.ScheduleMethodId);

                kk.ProductPrincipalGLCode = subglList.subGls.FirstOrDefault(x => x.SubGLId == kk.ProductPrincipalGl)?.SubGLCode;
                kk.ReceiverPrincipalGlCode = subglList.subGls.FirstOrDefault(x => x.SubGLId == kk.ReceiverPrincipalGl)?.SubGLCode;
                kk.InterestPayableGlCode = subglList.subGls.FirstOrDefault(x => x.SubGLId == kk.InterestPayableGl)?.SubGLCode;
                kk.InterstExpenseGlCode = subglList.subGls.FirstOrDefault(x => x.SubGLId == kk.InterstExpenseGl)?.SubGLCode;

                var row = dt.NewRow();
                row["Product Code"] = kk.ProductCode;
                row["Product Name"] = kk.ProductName;
                row["Product Type"] = producttype.Name;
                row["Product Limit"] = kk.ProductLimit;
                row["Interest Repayment Type"] = interestRepaymentType.RepaymentTypeName;
                row["Schedule Method"] = scheduleMethod.LoanScheduleTypeName;
                row["Frequency"] = frequency.Mode;
                row["Maximum Period"] = kk.MaximumPeriod;
                row["Interest Rate Annual"] = kk.InterestRateAnnual;
                row["Liquidation Charge"] = kk.EarlyTerminationCharge;
                row["Tax Rate"] = kk.TaxRate;
                row["Product Principal GL"] = kk.ProductPrincipalGLCode;
                row["Receiver Principal GL"] = kk.ReceiverPrincipalGlCode;
                row["Interest Payable GL"] = kk.InterestPayableGlCode;
                row["Interest Expense GL"] = kk.InterstExpenseGlCode;
                row["Tax GL"] = kk.TaxGl;
                
                dt.Rows.Add(row);
            }
            Byte[] fileBytes = null;

            if (statementType != null)
            {
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Product");
                    ws.DefaultColWidth = 20;
                    ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                    fileBytes = pck.GetAsByteArray();
                }
            }
            return fileBytes;
        }

        public List<InfProductObj> GetAllProduct()
        {
            try
            {
                var product = (from a in _dataContext.inf_product
                               where a.Deleted == false
                               select
                               new InfProductObj
                               {
                                   ProductId = a.ProductId,
                                   ProductCode = a.ProductCode,
                                   ProductName = a.ProductName,
                                   Rate = a.Rate,
                                   ProductTypeId = a.ProductTypeId,
                                   ProductLimit = a.ProductLimit,
                                   ProductLimitId = a.ProductLimitId,
                                   InterestRateMax = a.InterestRateMax,
                                   InterestRepaymentTypeId = a.InterestRepaymentTypeId,
                                   ScheduleMethodId = a.ScheduleMethodId,
                                   FrequencyId = a.FrequencyId,
                                   FrequencyName = _dataContext.credit_frequencytype.FirstOrDefault(x=>x.FrequencyTypeId == a.FrequencyId).Mode,
                                   MaximumPeriod = a.MaximumPeriod,
                                   InterestRateAnnual = a.InterestRateAnnual,
                                   InterestRateFrequency = a.InterestRateFrequency,
                                   ProductPrincipalGl = a.ProductPrincipalGl,
                                   ReceiverPrincipalGl = a.ReceiverPrincipalGl,
                                   InterstExpenseGl = a.InterstExpenseGl,
                                   InterestPayableGl = a.InterestPayableGl,
                                   EarlyTerminationCharge = a.EarlyTerminationCharge,
                                   TaxRate = a.TaxRate,
                                   TaxGl = a.TaxGl,
                               }).ToList();
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public InfProductObj GetProduct(int Id)
        {
            try
            {
                var product = (from a in _dataContext.inf_product
                               where a.Deleted == false && a.ProductId == Id
                               select
                              new InfProductObj
                              {
                                  ProductId = a.ProductId,
                                  ProductCode = a.ProductCode,
                                  ProductName = a.ProductName,
                                  Rate = a.Rate,
                                  ProductTypeId = a.ProductTypeId,
                                  ProductLimit = a.ProductLimit,
                                  ProductLimitId = a.ProductLimitId,
                                  InterestRateMax = a.InterestRateMax,
                                  InterestRepaymentTypeId = a.InterestRepaymentTypeId,
                                  ScheduleMethodId = a.ScheduleMethodId,
                                  FrequencyId = a.FrequencyId,
                                  MaximumPeriod = a.MaximumPeriod,
                                  InterestRateAnnual = a.InterestRateAnnual,
                                  InterestRateFrequency = a.InterestRateFrequency,
                                  ProductPrincipalGl = a.ProductPrincipalGl,
                                  ReceiverPrincipalGl = a.ReceiverPrincipalGl,
                                  InterstExpenseGl = a.InterstExpenseGl,
                                  InterestPayableGl = a.InterestPayableGl,
                                  EarlyTerminationCharge = a.EarlyTerminationCharge,
                                  TaxRate = a.TaxRate,
                                  TaxGl = a.TaxGl,
                              }).FirstOrDefault();
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public InfProductRespObj UploadProduct(List<byte[]> record, string createdBy)
        {
            try
            {
                List<InfProductObj> uploadedRecord = new List<InfProductObj>();
                if (record == null) return new InfProductRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Unsuccessful" } }
                };
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var subglList = _serverRequest.GetAllSubGlAsync().Result;
                if (record.Count() > 0)
                {
                    foreach (var byteItem in record)
                    {
                        using (MemoryStream stream = new MemoryStream(byteItem))
                        using (ExcelPackage excelPackage = new ExcelPackage(stream))
                        {
                            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                            int totalRows = workSheet.Dimension.Rows;

                            for (int i = 2; i <= totalRows; i++)
                            {
                                var item = new InfProductObj
                                {
                                    ProductCode = workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : null,
                                    ProductName = workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : null,
                                    ProductTypeName = workSheet.Cells[i, 3].Value != null ? workSheet.Cells[i, 3].Value.ToString() : null,
                                    ProductLimit = workSheet.Cells[i, 4].Value != null ? int.Parse(workSheet.Cells[i, 4].Value.ToString()) : 0,
                                    InterestRepaymentTypeName = workSheet.Cells[i, 5].Value != null ? workSheet.Cells[i, 5].Value.ToString() : null,
                                    ScheduleMethodName = workSheet.Cells[i, 6].Value != null ? workSheet.Cells[i, 6].Value.ToString() : null,
                                    FrequencyName = workSheet.Cells[i, 7].Value != null ? workSheet.Cells[i, 7].Value.ToString() : null,
                                    MaximumPeriod = workSheet.Cells[i, 8].Value != null ? decimal.Parse(workSheet.Cells[i, 8].Value.ToString()) : 0,
                                    InterestRateAnnual = workSheet.Cells[i, 9].Value != null ? decimal.Parse(workSheet.Cells[i, 9].Value.ToString()) : 0,
                                    EarlyTerminationCharge = workSheet.Cells[i, 10].Value != null ? decimal.Parse(workSheet.Cells[i, 10].Value.ToString()) : 0,
                                    TaxRate = workSheet.Cells[i, 11].Value != null ? decimal.Parse(workSheet.Cells[i, 11].Value.ToString()) : 0,
                                    ProductPrincipalGLCode = workSheet.Cells[i, 12].Value != null ? workSheet.Cells[i, 12].Value.ToString() : "",
                                    ReceiverPrincipalGlCode = workSheet.Cells[i, 13].Value != null ? workSheet.Cells[i, 13].Value.ToString() : "",
                                    InterestPayableGlCode = workSheet.Cells[i, 14].Value != null ? workSheet.Cells[i, 14].Value.ToString() : "",
                                    InterstExpenseGlCode = workSheet.Cells[i, 15].Value != null ? workSheet.Cells[i, 15].Value.ToString() : "",
                                };
                                uploadedRecord.Add(item);
                            }
                        }
                    }
                }

                if (uploadedRecord.Count > 0)
                {
                    foreach (var item in uploadedRecord)
                    {
                        if (item.ProductName == "" || item.InterestRepaymentTypeName == "" || item.ProductTypeName == "" || item.FrequencyName == "" || item.ScheduleMethodName == "")
                        {
                            return new InfProductRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include all fields" } }
                            };
                        }
                        var productId = 0;
                        var productobj = _dataContext.inf_product.Where(x => x.ProductName.ToLower().Trim() == item.ProductName.ToLower().Trim()).FirstOrDefault();
                        if (productobj != null)
                        {
                            productId = productobj.ProductId;
                        }
                        var productType = _dataContext.inf_producttype.Where(x => x.Name.ToLower().Trim() == item.ProductTypeName.ToLower().Trim()).FirstOrDefault();
                        if (productType == null)
                        {
                            return new InfProductRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid product type name" } }
                            };
                        }
                        var frequencyType = _dataContext.credit_frequencytype.Where(x => x.Mode.ToLower().Trim() == item.FrequencyName.ToLower().Trim()).FirstOrDefault();
                        if (frequencyType == null)
                        {
                            return new InfProductRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid frequency type name" } }
                            };
                        }
                        var interestRepayment = _dataContext.credit_repaymenttype.Where(x => x.RepaymentTypeName.ToLower().Trim() == item.InterestRepaymentTypeName.ToLower().Trim()).FirstOrDefault();
                        if (interestRepayment == null)
                        {
                            return new InfProductRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid interestRepayment type name" } }
                            };
                        }
                        var ScheduleType = _dataContext.credit_loanscheduletype.Where(x => x.LoanScheduleTypeName.ToLower().Trim() == item.ScheduleMethodName.ToLower().Trim()).FirstOrDefault();
                        if (ScheduleType == null)
                        {
                            return new InfProductRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid ScheduleType name" } }
                            };
                        }

                        var productPrincipalGL = subglList.subGls.Where(x => x.SubGLCode.ToLower().Trim() == item.ProductPrincipalGLCode.ToLower().Trim()).FirstOrDefault();
                        if (productPrincipalGL == null)
                        {
                            return new InfProductRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid Product Principal GL code" } }
                            };
                        }
                        var receiverPrincipalGL = subglList.subGls.Where(x => x.SubGLCode.ToLower().Trim() == item.ReceiverPrincipalGlCode.ToLower().Trim()).FirstOrDefault();
                        if (receiverPrincipalGL == null)
                        {
                            return new InfProductRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid Reciever Principal GL code" } }
                            };
                        }
                        var interestPayableGL = subglList.subGls.Where(x => x.SubGLCode.ToLower().Trim() == item.InterestPayableGlCode.ToLower().Trim()).FirstOrDefault();
                        if (interestPayableGL == null)
                        {
                            return new InfProductRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid Interest Payable GL code" } }
                            };
                        }
                        var interestExpenseGL = subglList.subGls.Where(x => x.SubGLCode.ToLower().Trim() == item.InterstExpenseGlCode.ToLower().Trim()).FirstOrDefault();
                        if (interestExpenseGL == null)
                        {
                            return new InfProductRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid Interest Expense GL code" } }
                            };
                        }
                        var taxGL = subglList.subGls.Where(x => x.SubGLCode.ToLower().Trim() == item.TaxGlCode.ToLower().Trim()).FirstOrDefault();
                        if (taxGL == null)
                        {
                            return new InfProductRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid Interest Tax GL code" } }
                            };
                        }
                        var productexist = _dataContext.inf_product.Find(productId);
                        if (productexist != null)
                        {
                            productexist.ProductCode = item.ProductCode;
                            productexist.ProductName = item.ProductName;
                            productexist.ProductTypeId = productType.ProductTypeId;
                            productexist.ProductLimit = item.ProductLimit;
                            productexist.InterestRepaymentTypeId = interestRepayment.RepaymentTypeId;
                            productexist.ScheduleMethodId = ScheduleType.LoanScheduleTypeId;
                            productexist.FrequencyId = frequencyType.FrequencyTypeId;
                            productexist.MaximumPeriod = item.MaximumPeriod;
                            productexist.InterestRateAnnual = item.InterestRateAnnual;
                            productexist.ProductPrincipalGl = productPrincipalGL.SubGLId;
                            productexist.ReceiverPrincipalGl = receiverPrincipalGL.SubGLId;
                            productexist.InterstExpenseGl = interestPayableGL.SubGLId;
                            productexist.InterestPayableGl = interestExpenseGL.SubGLId;
                            productexist.TaxGl = taxGL.SubGLId;
                            productexist.EarlyTerminationCharge = item.EarlyTerminationCharge;
                            productexist.TaxRate = item.EarlyTerminationCharge;
                            productexist.Active = true;
                            productexist.Deleted = false;
                            productexist.UpdatedBy = createdBy;
                            productexist.UpdatedOn = DateTime.Now;
                        }
                        else
                        {
                            var product = new inf_product
                            {
                                ProductCode = item.ProductCode,
                                ProductName = item.ProductName,
                                ProductTypeId = productType.ProductTypeId,
                                ProductLimit = item.ProductLimit,
                                InterestRepaymentTypeId = interestRepayment.RepaymentTypeId,
                                ScheduleMethodId = ScheduleType.LoanScheduleTypeId,
                                FrequencyId = frequencyType.FrequencyTypeId,
                                MaximumPeriod = item.MaximumPeriod,
                                InterestRateAnnual = item.InterestRateAnnual,
                                ProductPrincipalGl = productPrincipalGL.SubGLId,
                                ReceiverPrincipalGl = receiverPrincipalGL.SubGLId,
                                InterstExpenseGl = interestPayableGL.SubGLId,
                                InterestPayableGl = interestExpenseGL.SubGLId,
                                EarlyTerminationCharge = item.EarlyTerminationCharge,
                                Active = true,
                                Deleted = false,
                                CreatedBy = createdBy,
                                CreatedOn = DateTime.Now,
                                TaxRate = item.TaxRate,
                                TaxGl = taxGL.SubGLId,
                            };
                            _dataContext.inf_product.Add(product);
                        }
                    }
                }

                var response = _dataContext.SaveChanges() > 0;
                return new InfProductRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = response ? true : false, Message = new APIResponseMessage { FriendlyMessage = response ? "Successful" : "Unsuccessful" } }
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region ProductType
        public bool AddUpdateProductType(inf_producttype model)
        {
            try
            {
                if (model.ProductTypeId > 0)
                {
                    var itemToUpdate = _dataContext.inf_producttype.Find(model.ProductTypeId);
                     //_dataContext.inf_producttype.Add(itemToUpdate);
                    _dataContext.Entry(itemToUpdate).CurrentValues.SetValues(model);
                }
                else
                    _dataContext.inf_producttype.Add(model);
                return _dataContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteProductType(int id)
        {
            var itemToDelete = _dataContext.inf_producttype.Find(id);
            itemToDelete.Deleted = true;
            _dataContext.Entry(itemToDelete).CurrentValues.SetValues(itemToDelete);
            return _dataContext.SaveChanges() > 0;
        }

        public byte[] GenerateExportProductType()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Description");
            var statementType = (from a in _dataContext.inf_producttype
                                 where a.Deleted == false
                                 select new InfProductTypeObj
                                 {
                                     Name = a.Name,
                                     Description = a.Description
                                 }).ToList();

            foreach (var kk in statementType)
            {
                var row = dt.NewRow();
                row["Name"] = kk.Name;
                row["Description"] = kk.Description;
                dt.Rows.Add(row);
            }
            Byte[] fileBytes = null;

            if (statementType != null)
            {
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ProductType");
                    ws.DefaultColWidth = 20;
                    ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                    fileBytes = pck.GetAsByteArray();
                }
            }
            return fileBytes;
        }

        public List<InfProductTypeObj> GetAllProductType()
        {
            try
            {
                var producttype = (from a in _dataContext.inf_producttype
                               where a.Deleted == false
                               select

                               new InfProductTypeObj
                               {
                                   ProductTypeId = a.ProductTypeId,
                                   Name = a.Name,
                                   Description = a.Description,
                               }).ToList();

                return producttype;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public InfProductTypeObj GetProductType(int Id)
        {
            try
            {
                var product = (from a in _dataContext.inf_producttype
                               where a.Deleted == false && a.ProductTypeId == Id
                               select

                              new InfProductTypeObj
                              {
                                  ProductTypeId = a.ProductTypeId,
                                  Name = a.Name,
                                  Description = a.Description,
                              }).FirstOrDefault();

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public InfProductTypeRegRespObj UploadProductType(List<byte[]> record, string createdBy)
        {
            try
            {
                List<inf_producttype> uploadedRecord = new List<inf_producttype>();
                if (record == null) return new InfProductTypeRegRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Unsuccessful" } }
                };
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                if (record.Count() > 0)
                {
                    foreach (var byteItem in record)
                    {
                        using (MemoryStream stream = new MemoryStream(byteItem))
                        using (ExcelPackage excelPackage = new ExcelPackage(stream))
                        {
                            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                            int totalRows = workSheet.Dimension.Rows;

                            for (int i = 2; i <= totalRows; i++)
                            {
                                var item = new inf_producttype
                                {
                                    Name = workSheet.Cells[i, 1].Value.ToString(),
                                    Description = workSheet.Cells[i, 2].Value.ToString()
                                };
                                uploadedRecord.Add(item);
                            }
                        }
                    }
                }
                if (uploadedRecord.Count > 0)
                {
                    foreach (var item in uploadedRecord)
                    {
                        var producttypeexist = _dataContext.inf_producttype.FirstOrDefault(x=>x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                        if (producttypeexist != null)
                        {
                            producttypeexist.Name = item.Name;
                            producttypeexist.Description = item.Description;
                            producttypeexist.Active = true;
                            producttypeexist.Deleted = false;
                            producttypeexist.UpdatedBy = item.UpdatedBy;
                            producttypeexist.UpdatedOn = DateTime.Now;
                        }
                        else
                        {
                            var accountType = new inf_producttype
                            {
                                Name = item.Name,
                                Description = item.Description,
                                Active = true,
                                Deleted = false,
                                CreatedBy = createdBy,
                                CreatedOn = DateTime.Now,
                            };
                            _dataContext.inf_producttype.Add(accountType);
                        }
                    }
                }

                var response = _dataContext.SaveChanges() > 0;
                return new InfProductTypeRegRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = response ? true : false, Message = new APIResponseMessage { FriendlyMessage = response ? "Successful" : "Unsuccessful" } }
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

    }
}




