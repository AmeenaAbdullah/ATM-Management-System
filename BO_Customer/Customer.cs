using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO_Customer
{
   public class Customer
    {
        public string Login { get; set; }
        public string PinCode { get; set; }
        public string HolderName { get; set; }
        public string Type{ get; set; } 
        public decimal StartingBalnce { get; set; }
        public DateTime date { set; get; }
        public string status { set; get; }
        public int id { set; get; }
    }
    public class User
    {
        public string Login { get; set; }
        public string PinCode { get; set; }
        public string Type{ get; set; }

    }
    public class TransactionRecord
    {
        public int Id { get; set; }
        public string Transactiontype { get; set; }
        public string date { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }

    }
}
