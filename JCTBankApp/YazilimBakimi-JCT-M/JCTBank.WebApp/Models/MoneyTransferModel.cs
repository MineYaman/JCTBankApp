using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JCTBank.WebApp.Models
{
    public class MoneyTransferModel
    {
        public int Id { get; set; }

  
        public int SendingCustomerNo { get; set; }

        public int AdditionalSendingCustomerNo { get; set; }

       
        public int ReceiverNo { get; set; }

        public int AdditionalReceiverNo { get; set; }

        public decimal Balance { get; set; }

        public string TransferType { get; set; }
        public string WebOrMobil { get; set; }
    }
}