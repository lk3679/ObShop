using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBShopWeb.Poslib
{
    public class CreditCard
    {
    }

    public class CreditCardData
    {
        public virtual string TransactionType { get; set; }

        public virtual string HostId { get; set; }

        public virtual string ReceiptNo { get; set; }

        public virtual string CardNo { get; set; }

        public virtual string InstallmentPeriod { get; set; }

        public virtual string CupFlag { get; set; }

        public virtual int TransactionAmount { get; set; }

        public virtual DateTime TransactionTime { get; set; }

        public virtual string ApprovalNo { get; set; }

        public virtual int AuthAmount { get; set; }

        public virtual string ResponseCode { get; set; }

        public virtual string TerminalId { get; set; }

        public virtual string RefNo { get; set; }

        public virtual string Reserve { get; set; }

        public virtual string StoreId { get; set; }

        public virtual string Reserve2 { get; set; }
    }
}