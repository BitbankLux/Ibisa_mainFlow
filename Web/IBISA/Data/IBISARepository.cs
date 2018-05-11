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
    }
}