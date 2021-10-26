using Banking.Contracts.Response.Deposit;
using Banking.Data;
using Banking.DomainObjects.Credit;
using Banking.Repository.Interface.Credit;
using GODP.Entities.Models;
using GOSLibraries.Enums;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Repository.Implement.Credit
{
    public class FeeRepository : IFeeRepository
    {
        private readonly DataContext _dataContext;
        public FeeRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<bool> AddUpdateFeeAsync(credit_fee model)
        {
            try
            {
                if (model.FeeId > 0)
                {
                    var itemToUpdate = await _dataContext.credit_fee.FindAsync(model.FeeId);
                    _dataContext.Entry(itemToUpdate).CurrentValues.SetValues(model);
                }
                else
                    await _dataContext.credit_fee.AddAsync(model);
                return await _dataContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteFeeAsync(int id)
        {
            var itemToDelete = await _dataContext.credit_fee.FindAsync(id);
            itemToDelete.Deleted = true;
            _dataContext.Entry(itemToDelete).CurrentValues.SetValues(itemToDelete);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<credit_fee>> GetAllFeeAsync()
        {
            return await _dataContext.credit_fee.Where(x => x.Deleted == false).ToListAsync();
        }

        public async Task<credit_fee> GetFeeByIdAsync(int id)
        {
            return await _dataContext.credit_fee.FindAsync(id);
        }

        public async Task<bool> UploadFeeAsync(List<byte[]> record, string createdBy)
        {
            try
            {
                if (record == null) return false;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                List<credit_fee> uploadedRecord = new List<credit_fee>();
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
                                var item = new credit_fee
                                {
                                    FeeName = workSheet.Cells[i, 1].Value.ToString(),
                                    IsIntegral = bool.Parse(workSheet.Cells[i, 2].Value.ToString()),
                                    PassEntryAtDisbursment = bool.Parse(workSheet.Cells[i, 3].Value.ToString()),
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
                        var category = _dataContext.credit_fee.Where(x => x.FeeName == item.FeeName && x.Deleted == false).FirstOrDefault();
                        if (category != null)
                        {
                            category.FeeName = item.FeeName;
                            category.IsIntegral = (bool)item.IsIntegral;
                            category.PassEntryAtDisbursment = (bool)item.PassEntryAtDisbursment;
                            category.Active = true;
                            category.Deleted = false;
                            category.UpdatedBy = createdBy;
                            category.UpdatedOn = DateTime.Now;
                        }
                        else
                        {
                            var structure = new credit_fee
                            {
                                FeeName = item.FeeName,
                                IsIntegral = (bool)item.IsIntegral,
                                PassEntryAtDisbursment = (bool)item.PassEntryAtDisbursment,
                                Active = true,
                                Deleted = false,
                                CreatedBy = createdBy,
                                CreatedOn = DateTime.Now,
                            };
                            //structures.Add(structure);
                           await _dataContext.credit_fee.AddAsync(structure);
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

        public byte[] GenerateExportFees()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Fee Name");
            dt.Columns.Add("Is Integral");
            dt.Columns.Add("PassEntryAtDisbursment");
            var structures = (from a in _dataContext.credit_fee
                              where a.Deleted == false
                              select new credit_fee
                              {
                                  FeeName = a.FeeName,
                                  FeeId = a.FeeId,
                                  IsIntegral = a.IsIntegral,
                                  PassEntryAtDisbursment = a.PassEntryAtDisbursment
                              }).ToList();
            foreach (var kk in structures)
            {
                var row = dt.NewRow();
                row["Fee Name"] = kk.FeeName;
                row["Is Integral"] = kk.IsIntegral;
                row["PassEntryAtDisbursment"] = kk.PassEntryAtDisbursment;
                dt.Rows.Add(row);
            }
            Byte[] fileBytes = null;

            if (structures != null)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Fees");
                    ws.DefaultColWidth = 20;
                    ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                    fileBytes = pck.GetAsByteArray();
                }
            }
            return fileBytes;
        }

        public async Task<IEnumerable<credit_fee>> GetAllIntegralFeeAsync()
        {
            return await _dataContext.credit_fee.Where(x => x.Deleted == false && x.IsIntegral == false).ToListAsync();
        }


        //RepaymentType
        public async Task<IEnumerable<credit_repaymenttype>> GetAllRepaymentTypeAsync()
        {
            return await _dataContext.credit_repaymenttype.Where(x => x.Deleted == false).ToListAsync();
        }


        public string AddUpdateDepositForm(DepositformObj entity)
        {
            try
            {
                if (entity == null) return null;
                deposit_depositform deposit = new deposit_depositform();

                var refNo = GenerateLoanReferenceNumber();
                deposit = new deposit_depositform
                {
                    DepositFormId = entity.DepositFormId,
                    Structure = entity.Structure,
                    CurrencyId = entity.Currency,
                    Operation = (int)OperationsEnum.DepositFormSubmit,
                    TransactionId = refNo,
                    AccountNumber = entity.AccountNumber,
                    Amount = entity.Amount,
                    ValueDate = entity.ValueDate,
                    TransactionDate = DateTime.Now,
                    TransactionDescription = entity.TransactionDescription,
                    TransactionParticulars = entity.TransactionParticulars,
                    Remark = entity.Remark,
                    ModeOfTransaction = entity.ModeOfTransaction,
                    InstrumentNumber = entity.InstrumentNumber,
                    InstrumentDate = entity.InstrumentDate,
                };
                _dataContext.deposit_depositform.Add(deposit);
                _dataContext.SaveChanges();

                return refNo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<DepositformObj> GetAllDepositForm()
        {
            try
            {
                var category = (from a in _dataContext.deposit_depositform
                                join b in _dataContext.credit_loancustomer on a.AccountNumber equals b.CASAAccountNumber
                                where a.Deleted == false
                                select

                                       new DepositformObj
                                       {
                                           DepositFormId = a.DepositFormId,
                                           Structure = a.Structure,
                                           //CompanyName = _dataContext.cor_companystructure.Where(x => x.CompanyStructureId == a.Structure).FirstOrDefault().Name,
                                           Operation = a.Operation,
                                           TransactionId = a.TransactionId,
                                           AccountNumber = a.AccountNumber,
                                           AcountName = $"{ b.FirstName}  { b.LastName}",
                                           AvailableBalance = _dataContext.credit_casa.Where(a => a.AccountNumber == b.CASAAccountNumber).FirstOrDefault().AvailableBalance,
                                           Amount = a.Amount,
                                           ValueDate = a.ValueDate,
                                           TransactionDate = a.TransactionDate,
                                           TransactionDescription = a.TransactionDescription,
                                           TransactionParticulars = a.TransactionParticulars,
                                           Remark = a.Remark,
                                           ModeOfTransaction = a.ModeOfTransaction,
                                           InstrumentDate = a.InstrumentDate,
                                           InstrumentNumber = a.InstrumentNumber,
                                       }).ToList().OrderByDescending(x => x.TransactionDate);
                return category;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GenerateLoanReferenceNumber()
        {
            TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);
            TimeSpan unixTicks = new TimeSpan(DateTime.UtcNow.Ticks) - epochTicks;
            double unixTime = (int)unixTicks.TotalSeconds;
            return unixTime.ToString();
        }
    }
}
