using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final
{
    internal class Transaction
    {
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public decimal AmountGel { get; set; }
        public decimal AmountUsd { get; set; }
        public decimal AmountEur { get; set; }

    }
}
