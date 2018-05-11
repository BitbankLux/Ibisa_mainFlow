using System;
using System.Data;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.Web.Mvc;
using IBISA.Models;
using LinqToExcel;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using IBISA.Data;
using IBISA.Helper;

namespace IBISA.Controllers
{
    public class IBISAAdminController : Controller
    {
        #region Action Methods

        public ActionResult ImportExcel()
        {
            return View();
        }

        public JsonResult UploadFile()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    var path = Server.MapPath("~/Uploads");
                    path = System.IO.Path.Combine(path,
                        Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Request.Files[0].FileName));
                    Request.Files[0].SaveAs(path);

                    var excel = new ExcelQueryFactory(path);
                    //var Viewmodal = from c in excel.Worksheet<IBISAViewmodal>()
                    //                select c;


                    object[,] obj = null;
                    int noOfCol = 0;
                    int noOfRow = 0;
                    HttpFileCollectionBase file = Request.Files;
                    if ((file != null) && (file.Count > 0))
                    {
                        //string fileName = file.FileName;
                        //string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[Request.ContentLength];
                        var data = Request.InputStream.Read(fileBytes, 0, Convert.ToInt32(Request.ContentLength));
                        // var usersList = new List<Users>();
                        //using (var package = new ExcelPackage())
                        using (var package = new ExcelPackage(Request.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            noOfCol = workSheet.Dimension.End.Column;
                            noOfRow = workSheet.Dimension.End.Row;
                            obj = new object[noOfRow, noOfCol];
                            obj = (object[,])workSheet.Cells.Value;

                        }
                    }
                    return Json(new { data = obj, row = noOfRow, col = noOfCol }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportFile()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ProcessFileUpload()
        {
            if (Request != null && Request.ContentLength > 0 && Request.Files.Count > 0)
            {
                try
                {
                    var httpPostedFileBase = Request.Files[0];
                    var path = Path.Combine(Server.MapPath("~/Uploads"), Guid.NewGuid() + Path.GetExtension(httpPostedFileBase.FileName));
                    httpPostedFileBase.SaveAs(path);

                    DataTable dataTable = new DataTable();
                    object[,] obj = null;
                    int noOfCol = 0;
                    int noOfRow = 0;

                    using (var package = new ExcelPackage(Request.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        noOfCol = workSheet.Dimension.End.Column;
                        noOfRow = workSheet.Dimension.End.Row;
                        obj = (object[,])workSheet.Cells.Value;

                        #region Convert Worksheet to DataTable

                        foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                        {
                            dataTable.Columns.Add(firstRowCell.Text);
                        }
                        const int startRow = 2;
                        for (int rowNum = startRow; rowNum <= workSheet.Dimension.End.Row; rowNum++)
                        {
                            var workSheetRow = workSheet.Cells[rowNum, 1, rowNum, workSheet.Dimension.End.Column];
                            DataRow row = dataTable.Rows.Add();
                            foreach (var cell in workSheetRow)
                            {
                                row[cell.Start.Column - 1] = cell.Text;
                            }
                        }

                        #endregion
                    }

                    if (dataTable.Rows.Count > 0)
                    {
                        var validateHeaders = ValidateHeaders(dataTable);
                        if (validateHeaders)
                        {
                            #region Transfer Ether To Smart Contract
                            var initialResponse = PostRequest("/TransferEtherToSmartContract");
                            #endregion

                            #region Create Users
                            var userResponse = PostRequest("/createUsers");
                            #endregion

                            #region Get Wallet Addresses
                            dynamic walletResponse = JObject.Parse(GetResponse("/GetAllWallets"));
                            var walletAddresses = JsonConvert.DeserializeObject<List<string>>(walletResponse.data.ToString());
                            //var walletAddresses = new List<string> { "", "", "", "", "", "", "", "", "" } ;
                            #endregion

                            var counter = 0;
                            foreach (DataRow dr in dataTable.Rows)
                            {
                                #region Process Date To Produce UNIX TimeStamp
                                var dateOfAgreement = Convert.ToDateTime(dr[EnumHelper.ImportParameters.DateOfAgreement.ToString()]);
                                #endregion

                                var transactionDetail = new Transaction
                                {
                                    Agreement = new Agreement
                                    {
                                        AgreementId = 1,
                                        AgreementNumber = Convert.ToString(dr[EnumHelper.ImportParameters.AgreementNumber.ToString()]),
                                        WalletAddress = walletAddresses[counter],
                                        Zone = Convert.ToString(dr[EnumHelper.ImportParameters.Zone.ToString()]),
                                        Location = Convert.ToString(dr[EnumHelper.ImportParameters.Location.ToString()]),
                                        PlotId = Convert.ToString(dr[EnumHelper.ImportParameters.PlotID.ToString()]),
                                        Crop = Convert.ToString(dr[EnumHelper.ImportParameters.Crop.ToString()]),
                                        MaxPayout = Convert.ToInt32(dr[EnumHelper.ImportParameters.MaxPayout.ToString()]),
                                        TargetContribution = Convert.ToInt32(dr[EnumHelper.ImportParameters.TargetContribution.ToString()]),
                                        UserDisplayName = Convert.ToString(dr[EnumHelper.ImportParameters.UserDisplayName.ToString()])
                                    },

                                    DateOfAgreement = dateOfAgreement,
                                    Amount = Convert.ToInt32(dr[EnumHelper.ImportParameters.CurrentContribution.ToString()]),
                                    TransactionDate = DateTime.Now,
                                    TransactionTypeId = (int)EnumHelper.TransactionType.Contribution
                                };

                                var createAgreementParams = new { wallet = transactionDetail.Agreement.WalletAddress, zone = transactionDetail.Agreement.Zone, crop = transactionDetail.Agreement.Crop, maxPayout = transactionDetail.Agreement.MaxPayout, targetContrib = transactionDetail.Agreement.TargetContribution, date = ((DateTimeOffset)dateOfAgreement).ToUnixTimeSeconds(), amt = transactionDetail.Amount };
                                var jsonParam = JsonConvert.SerializeObject(createAgreementParams);
                                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonParam);
                                var byteContent = new ByteArrayContent(buffer);
                                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                dynamic agreementResponse = JObject.Parse(PostRequest("/CreateAgreement", byteContent));
                                transactionDetail.TransactionHash = agreementResponse.data.tx_hash;

                                #region Save Agreements To DB

                                using (var ibisaRepository = new IBISARepository())
                                {
                                    ibisaRepository.SaveTransaction(transactionDetail);
                                }

                                #endregion

                                counter++;
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                IsValid = false,
                                Message = "File headers do not match the format required for Agreement.",
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            IsValid = false,
                            Message = "Data is not available in the uploaded file. Please verify it and upload once again.",
                        }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new
                    {
                        IsValid = true,
                        Message = "File uploaded and processed successfully.",
                        data = obj,
                        row = noOfRow,
                        col = noOfCol
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ExceptionHelper.Log("Method: ProcessFileUpload()", ex);
                    return Json(new
                    {
                        IsValid = false,
                        Message = "There is an error occurred while processing your file. Please verify it and upload once again.",
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                IsValid = false,
                Message = "There is an error occurred while processing your file. Please verify it and upload once again.",
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SystemDashboard()
        {
            try
            {
                dynamic response = JObject.Parse(GetResponse(string.Format("/GetSystemDetails/{0}", "zoneA")));
                if (response.status > 0)
                {
                    DashboardInfo dashboardInfo = JsonConvert.DeserializeObject<DashboardInfo>(response.data.ToString());

                    var dashboardInfoFromDB = new DashboardInfo();
                    using (var ibisaRepository = new IBISARepository())
                    {
                        dashboardInfoFromDB.AgreementDetails = ibisaRepository.GetAgreements();
                    }

                    //if (dashboardInfo.AgreementDetails != null && dashboardInfo.AgreementDetails.Any())
                    //    dashboardInfo.totalUsers = dashboardInfo.AgreementDetails.Count;

                    if (dashboardInfo.AgreementDetails != null && dashboardInfoFromDB.AgreementDetails != null && dashboardInfo.AgreementDetails.Any() && dashboardInfoFromDB.AgreementDetails.Any())
                    {
                        dashboardInfoFromDB.AgreementDetails.ForEach(x =>
                        {
                            var agreement = dashboardInfo.AgreementDetails.FirstOrDefault(i => i.wallet_address == x.wallet_address);
                            if (agreement != null)
                            {
                                agreement.agreementPrimaryId = x.agreementPrimaryId;
                                agreement.agreementNumber = x.agreementNumber;
                                agreement.zone = x.zone;
                            }
                        });
                    }

                    return View(dashboardInfo);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log("Method: SystemDashboard()", ex);
            }
            
            return View(new DashboardInfo());
        }

        [HttpPost]
        public JsonResult Contribute(int agreementPrimaryId, int id, string address)
        {
            try
            {
                TransactionInfo transactionDetail;
                using (var ibisaRepository = new IBISARepository())
                {
                    transactionDetail = ibisaRepository.GetLatestTransactionDetailsByTrsanactionType(agreementPrimaryId, (int)EnumHelper.TransactionType.Contribution);
                }

                if (transactionDetail == null || transactionDetail.AgreementPrimaryId <= 0) return Json(new { status = 0 });

                var date = transactionDetail.DateOfAgreement.AddMonths(1);
                var amount = transactionDetail.Amount;
                dynamic response = PostRequest(string.Format("/contribute/{0}/{1}/{2}/{3}", id, address, ((DateTimeOffset)date).ToUnixTimeSeconds(), amount));
                var jsonResponse = (JObject.Parse(response)).data;
                var transactionHash = jsonResponse.tx_hash;

                var transaction = new Transaction
                {
                    AgreementPrimaryId = agreementPrimaryId,
                    TransactionHash = transactionHash,
                    DateOfAgreement = date,
                    Amount = amount,
                    TransactionDate = DateTime.Now,
                    TransactionTypeId = (int)EnumHelper.TransactionType.Contribution
                };

                var result = false;
                using (var ibisaRepository = new IBISARepository())
                {
                    result = ibisaRepository.SaveTransaction(transaction);
                }

                if (result)
                    return Json(response);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log("Method: Contribute()", ex);
            }
            return Json(new { status = 0 });
        }

        [HttpPost]
        public JsonResult Indemnify(int agreementPrimaryId, int id, string address, string zone)
        {
            try
            {
                dynamic response = PostRequest(string.Format("/Indemnify/{0}/{1}/{2}", id, address, zone));
                var jsonResponse = (JObject.Parse(response)).data;
                var transactionHash = jsonResponse.tx_hash;
                var amount = Convert.ToDecimal(jsonResponse.indemnity_amount);

                var transaction = new Transaction
                {
                    AgreementPrimaryId = agreementPrimaryId,
                    TransactionHash = transactionHash,
                    DateOfAgreement = null,
                    Amount = amount,
                    TransactionDate = DateTime.Now,
                    TransactionTypeId = (int)EnumHelper.TransactionType.Indemnification
                };

                var result = false;
                using (var ibisaRepository = new IBISARepository())
                {
                    result = ibisaRepository.SaveTransaction(transaction);
                }

                if (result)
                    return Json(response);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log("Method: Indemnify()", ex);
            }
            
            return Json(new { status = 0 });
        }

        public ActionResult TransactionSummary(int id)
        {
            List<TransactionInfo> transactionDetails;
            using (var ibisaRepository = new IBISARepository())
            {
                transactionDetails = ibisaRepository.GetTransactionDetails(id);
            }

            return View(transactionDetails);
        }

        public ActionResult TransactionDetails(int id, string transactionHash)
        {
            try
            {
                dynamic response = GetResponse(string.Format("/GetTransactionReceipt/{0}", transactionHash));
                var jsonResponse = (JObject.Parse(response)).data;

                ViewBag.AgreementPrimaryId = id;
                ViewBag.hash = jsonResponse.hash;
                ViewBag.nonce = jsonResponse.nonce;
                ViewBag.blockHash = jsonResponse.blockHash;
                ViewBag.blockNumber = jsonResponse.blockNumber;
                ViewBag.transactionIndex = jsonResponse.transactionIndex;
                ViewBag.from = jsonResponse.from;
                ViewBag.to = jsonResponse.to;
                ViewBag.value = jsonResponse.value;
                ViewBag.gas = jsonResponse.gas;
                ViewBag.gasPrice = jsonResponse.gasPrice;
                ViewBag.input = jsonResponse.input;
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log("Method: SystemDashboard()", ex);
            }

            return View();
        }

        #endregion

        #region Private Methods

        private string GetResponse(string text)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.BlockchainUriTimeoutInSeconds);
            httpClient.BaseAddress = new Uri(AppSettingsHelper.BlockchainUri);
            var response = httpClient.GetAsync(text).Result;
            var contents = response.Content.ReadAsStringAsync().Result;
            httpClient.Dispose();
            return contents;
        }

        private string PostRequest(string text, HttpContent httpContent = null)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.BlockchainUriTimeoutInSeconds);
            httpClient.BaseAddress = new Uri(AppSettingsHelper.BlockchainUri);
            var response = httpClient.PostAsync(text, httpContent).Result;
            var contents = response.Content.ReadAsStringAsync().Result;
            httpClient.Dispose();
            return contents;
        }

        private static bool ValidateHeaders(DataTable dataTable)
        {
            var enumParams = Enum.GetNames(typeof(EnumHelper.ImportParameters));
            var columnNames = (from dc in dataTable.Columns.Cast<DataColumn>()
                               select dc.ColumnName).ToArray();
            return enumParams.SequenceEqual(columnNames);
        }

        #endregion
    }

    #region Extension Methods
    public static class Extensions
    {
        public static string KiloFormat(this int num)
        {
            if (num >= 100000000)
                return (num / 1000000).ToString("#,0M");

            if (num >= 10000000)
                return (num / 1000000).ToString("0.#") + "M";

            if (num >= 100000)
                return (num / 1000).ToString("#,0K");

            if (num >= 10000)
                return (num / 1000).ToString("0.#") + "K";

            if (num >= 1000)
                return (num / 1000).ToString("0.#") + "K";

            return num.ToString("#,0");
        }

        public static string KiloFormat(this decimal num)
        {
            if (num >= 100000000)
                return (num / 1000000).ToString("#,0M");

            if (num >= 10000000)
                return (num / 1000000).ToString("0.#") + "M";

            if (num >= 100000)
                return (num / 1000).ToString("#,0K");

            if (num >= 10000)
                return (num / 1000).ToString("0.#") + "K";

            if (num >= 1000)
                return (num / 1000).ToString("0.#") + "K";

            return num.ToString("#,0");
        }
    }
    #endregion
}