using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IBISA.Controllers;
using IBISA.Models;

namespace IBISA.Data
{
    public class IBISARepository : IDisposable
    {
        private bool _disposed;
        private readonly IBISAEntities _entities;

        public IBISARepository()
        {
            _entities = new IBISAEntities();
        }

        public List<AgreementDetail> GetAgreements()
        {
            return (from agreement in this._entities.Agreements
                    let contibutionWallet = (from txs in agreement.Transactions
                                             where txs.TransactionTypeId == 1 && txs.Amount.HasValue
                                             select txs.Amount.Value)
                    let indemnityWallet = (from txs in agreement.Transactions
                                           where txs.TransactionTypeId == 2 && txs.Amount.HasValue
                                           select txs.Amount.Value)
                    select new AgreementDetail()
                    {
                        agreementPrimaryId = agreement.AgreementPrimaryId,
                        agreementNumber = agreement.AgreementNumber,
                        agreementId = agreement.AgreementId ?? 1,
                        wallet_address = agreement.WalletAddress,
                        deposit_wallet = contibutionWallet.Any() ? contibutionWallet.Sum(x => x) : 0,
                        withdrawal_wallet = indemnityWallet.Any() ? indemnityWallet.Sum(x => x) : 0,
                        zone = agreement.Zone
                    }).ToList();
        }

        public bool SaveTransaction(Transaction transaction)
        {
            this._entities.Transactions.Add(transaction);
            return this._entities.SaveChanges() == 1;
        }

        public List<TransactionInfo> GetTransactionDetails(int id)
        {
            return (from txs in this._entities.Transactions
                    where txs.AgreementPrimaryId == id
                    select new TransactionInfo
                    {
                        TransactionId = txs.TransactionId,
                        AgreementPrimaryId = txs.AgreementPrimaryId,
                        AgreementNumber = txs.Agreement.AgreementNumber,
                        TransactionHash = txs.TransactionHash,
                        DateOfAgreement = txs.DateOfAgreement ?? txs.TransactionDate.Value,
                        Amount = txs.Amount ?? 0,
                        TransactionTypeId = txs.TransactionTypeId.Value
                    }).ToList();
        }

        public TransactionInfo GetLatestTransactionDetailsByTrsanactionType(int id, int transactionTypeId)
        {
            return (from txs in this._entities.Transactions
                    where txs.AgreementPrimaryId == id && txs.TransactionTypeId == transactionTypeId
                    orderby txs.DateOfAgreement ?? txs.TransactionDate descending
                    select new TransactionInfo
                    {
                        TransactionId = txs.TransactionId,
                        AgreementPrimaryId = txs.AgreementPrimaryId,
                        AgreementNumber = txs.Agreement.AgreementNumber,
                        TransactionHash = txs.TransactionHash,
                        DateOfAgreement = txs.DateOfAgreement ?? txs.TransactionDate.Value,
                        Amount = txs.Amount ?? 0,
                        TransactionTypeId = txs.TransactionTypeId.Value
                    }).FirstOrDefault();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._entities.Dispose();
                    //this._UTMC.Dispose();
                }
            }
            this._disposed = true;
        }

        #endregion

        #region Watchers
        public List<WatcherWorkplaceDetails> GetWorkplaceDetail()
        {
            return (from I in _entities.Iddirs
                    join Q in _entities.Questions on I.IddirId equals Q.IddirId

                    select new WatcherWorkplaceDetails()
                    {
                        iddirId = I.IddirId,
                        quationsId = Q.QuestionId,
                        quations = Q.Question1,
                        iddirName = I.Name,
                        dueDate = Q.DueDate,
                        isAnswerd = false
                    }).ToList();
        }

        public List<WatherResponse> getTaskCompletDetail(int userId)
        {

            return (from WR in _entities.WatcherResponses
                    where WR.WatcherId == userId
                    select new WatherResponse()
                    {
                        WatcherResponseId = WR.WatcherResponseId,
                        QuestionId = WR.QuestionId,
                        OptionId = WR.OptionId,
                        WatcherId = WR.WatcherId
                    }).ToList();

        }

        public List<options> getOptions(int qId)
        {
            return (from Q in _entities.Questions
                    join O in _entities.Options on Q.QuestionId equals O.QuestionId
                    select new options()
                    {
                        optionDesc = O.OptionName,
                        optionValue = O.OptionValue,
                        optionId = O.OptionId,
                        quation = Q.Question1,
                        displayorder = O.DisplayOrder
                    }).OrderBy(a=>a.displayorder).ToList();
        }

        public bool saveWatcherresponse(WatcherRespons watherQA)
        {
            this._entities.WatcherResponses.Add(watherQA);
            return this._entities.SaveChanges() == 1;
        }
        public LoginModel CheckLogin(LoginModel lm)
        {
            var userDetail = _entities.Users.Where(a => a.Username == lm.userName && a.Password == lm.password).FirstOrDefault();
            LoginModel logindetails = new LoginModel();

            if (userDetail != null)
            {
                logindetails.userId = userDetail.UserId;
                logindetails.userName = userDetail.Username;
            }
            else
            {
                logindetails = null;
            }
            return logindetails;

        }
        public int GetSelectedOption(int watcherId, int quationId)
        {
            int sOption = 0;
            var wrDetail = _entities.WatcherResponses.Where(a => a.WatcherId == watcherId && a.QuestionId == quationId).FirstOrDefault();
            if (wrDetail != null)
            {
                sOption = wrDetail.OptionId;
            }
            return sOption;
        }
        public string getquation(int iddirid)
        {
            string quation = "";
            var qDetail = _entities.Questions.Where(a => a.IddirId == iddirid).FirstOrDefault();
            if (qDetail != null)
            {
                quation = qDetail.Question1;
            }
            return quation;
        }
        public List<BubbleChartData> GetBubblechartData(int RadiusMultiplicationFactor)
        {
            var a = _entities.Database.ExecuteSqlCommand("exec SP_Bubblechart");
            var b = (from p in _entities.WatcherResponses
                     join c in _entities.Options on p.OptionId equals c.OptionId into j1
                     from j2 in j1.DefaultIfEmpty()
                     group j2 by new { j2.OptionId ,j2.OptionName, j2.OptionValue }  into grouped
                     select new { ParentId = grouped.Key, Count = grouped.Count(t => t.OptionId != 0) });

            
            List<BubbleChartData> listbubble = new List<BubbleChartData>();
            foreach (var item in b)
            {
                BubbleChartData bbl = new BubbleChartData();
                bbl.x = item.ParentId.OptionValue;
                bbl.y = item.Count;
                bbl.r = item.Count * RadiusMultiplicationFactor;
                bbl.lableOption = item.ParentId.OptionName;
                listbubble.Add(bbl);
            }
            return listbubble;
        }

        public void MachineAssestmentData()
        {
            var a = _entities.MachineAssessments.GroupBy(i => i.AssessmentGroup).
                Select(g => new { Anomakly = g.Average(i => i.Anomaly), cc = g.Key});


            try
            {
                var test = _entities.Database.SqlQuery(typeof(MachineAssessment), "select * from MachineAssessments", null);
            }
            catch (Exception ex)
            {
                throw;
            }

            try
            {
                var t = _entities.Database.SqlQuery(typeof(List<MachineAssessment>), "select * from MachineAssessments", null).AsQueryable();
                foreach (var item in t)
                {

                }
            }
            catch (Exception ex)
            {
                throw;
            }

            //var avgRes = _entities.MachineAssessments.SqlQuery("select AVG(Anomaly) as 'Anomaly', AssessmentGroup from [IBISA].[dbo].[MachineAssessment] group by AssessmentGroup");
        }
        #endregion
    }
}