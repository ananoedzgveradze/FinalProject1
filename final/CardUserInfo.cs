using final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final
{
    internal class CardUserInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public CardDetails Details { get; set; }
        public string PinCode { get; set; }
        public decimal Balance { get; set; }
        public List<Transaction> TransactionHistory { get; set; }
    }
}
