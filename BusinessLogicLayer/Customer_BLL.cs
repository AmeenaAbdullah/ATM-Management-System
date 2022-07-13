using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO_Customer;
using DataAccessLayer;
namespace BusinessLogicLayer
{
  
    public class Customer_BLL
    {
        AllData data = new AllData();            //FOR ACCESS DATA LINK LAYER

        /***************************DISABLE LOGIN FUNCTION*****************************/
        public void disableLogin(Customer c)
        {
            c.status = "disable";
            c.Login = Encrypt(c.Login);
          //  UpdateAccount(data.GetID(c,0), c,1);
            data.AccountDisable(data.GetID(c, 0));
        }

        /// <summary>
        /// This function is used to create new customer 
        /// </summary>
        /// <param name="cus">customer object</param>
        public bool CreateNewAcount(Customer cus)
        {
            //check user already exist or not 
           if(CusExist(cus,out Customer c))
            {
                return false;
            }
            else
            {
                //encrypt login information 
                cus.Login=Encrypt(cus.Login);
                cus.PinCode = Encrypt(cus.PinCode);
                data.SaveAccount(cus);        //save account info
                User u = new User();
                u.Login = cus.Login;
                u.PinCode = cus.PinCode;
                u.Type = "customer";
                data.SaveUser(u);
                return true;
            }
            
        }
        /*********************************** DELETE THE EXISTING ACCOUNT BY TAKE CUSTOMER AS A PARAMETER ************************************/
        public bool DeleteExistingAcount(int id,out Customer name)
        {
            //check user exist or not 
            if (data.CustomerExistById(id,out Customer n))
            {
                //delete the user
                
                name = n;
                return true;
            }
            else
            {
                name = n;
                return false;
            }

        }
       
        /*****************DELTE ACCOUNT BY ID FROM THE USER****************************/
        public void delete(int id)
        {
            User u = new User();
            Customer c = new Customer();
            c = GetCustomerInfo(id);
            u.Login = c.Login;
            u.PinCode = c.PinCode;
            data.DeleteTransactionRecord(id);
            data.deleteUser(u);
            data.DeleteAccount(id);
           
        }

        /****************************GET CUSTOMER INFO FROM THE DATA BASE AND PASS INTO THE BUSINESS LOGIC LAYER********************************/
        public Customer GetCustomerInfo(int accountno)
        {
            Customer customer = new Customer();
            if(data.CustomerExistById(accountno,out customer))
            {
                return customer;
            }
            else
            {
                return null;
            }

        }
        /*********************************** GET CUSTOMER BY TAKING PARAMETER AS A CUTOMER(INFO) ************************************/

        public Customer GetCustomer(Customer c)
        {
            c.Login = Encrypt(c.Login);
            c.PinCode = Encrypt(c.PinCode);
            int id = data.GetID(c,0);
            return GetCustomerInfo(id);  //get customer
        }
        /*********************************** UPDATE CUSTOMER BY TAKING PARAMETER AS A CUTOMER(INFO) ************************************/

        public void UpdateAccount(int Accountid,Customer cus,int flag)
        {
            if (flag == 0)
            {
                cus.Login = Encrypt(cus.Login);
                cus.PinCode = Encrypt(cus.PinCode);
            }
            
            data.UpdateAccount(Accountid, cus);
           
        }

        /*****************************SEARCH DATA********************************/
        public List<Customer> SearchData(Customer c)
        {
            if(c.Login!="" ) c.Login=Encrypt(c.Login);
            if (c.PinCode != "") c.PinCode = Encrypt(c.PinCode);
            List<Customer> customers = new List<Customer>();
            customers=data.SearchAccount(c);
            return customers;
        }
        /*****************************DEPOSITE CASH********************************/
        public void DepositeCash(decimal blnc,Customer cus)
        {
            decimal balance = blnc + cus.StartingBalnce;
            cus.StartingBalnce = balance;
            int id=data.GetID(cus,1);

            UpdateAccount(id,cus,1);

        }
        public void AddTransaction(string type,Customer c,decimal amount)
        {
            data.AddTranactionRecord(type,c,amount);
        }
        /**************************** CHECK ACCOUNT ********************************/
        public bool CheckwithdrawalLimit(Customer c,decimal w)
        {
            
            if (data.CheckwithdrawalLimit(c, w,out decimal total))
            {
             if(total+w > 20000)
                    return false;
            
            }
           
            return true;

        }
        /****************************GET RECORD BETWEEN SPECIFIC AMOUNT********************************/
        public List<Customer> GetRecordsByAmountRange(decimal minAmount,decimal maxAmount)
        {
            List<Customer> c = new List<Customer>();
            c=data.GetRecordsByAmountRange(minAmount, maxAmount);
            foreach(Customer cus in c)
            {
               // cus.Login = Encrypt(cus.Login);
               //cus.PinCode = Encrypt(cus.PinCode);
            }
            return c;
        }
        /****************************GET RECORD BETWEEN TWO DATES********************************/
        public (List<Customer>, List<TransactionRecord>) GetRecordsByDateRange(DateTime startDate, DateTime EndDate)
        {
            List<Customer> c = new List<Customer>();
            List<TransactionRecord> TR = new List<TransactionRecord>();
           (c,TR) = data.GetRecordsByDate(startDate, EndDate);
            return (c,TR);
        }

        public bool Checkwithdrawal(decimal widthdrawAmount,decimal blnc,out decimal remainingBlnc)
        {
            //if withdraw amount is available in the current account
            remainingBlnc = blnc - widthdrawAmount;
            if (blnc - widthdrawAmount >= 0)
            {
                
                return true;
            }
         
            return false;
        }
        /****************************WITHDRAW CASH********************************/
        public void withdrawalcash(Customer c)
        {

            //c.Login = Encrypt(c.Login);
            //c.PinCode = Encrypt(c.PinCode);
            int id = data.GetID(c,1);
            UpdateAccount(id,c,1);
        }


        public TransactionRecord GetTransactionRecord(string t,Customer c)
        {
            TransactionRecord record = new TransactionRecord();
            record = data.TransactionRecord(c);
            return record;
        }
        /// <summary>
        /*This function is used to encrypt pincode.
       For alphabets we swap A with Z, B with Y and so on.
       A B C D E F G H I J K L M N O P Q R S T U V W X Y Z
       Z Y X W V U T S R Q P O N M L K J I H G F E D C B A
       For Number we have
       0123456789
       9876543210
        */
        /// </summary>
        /// <param name="pin">pincode</param>
        /// <returns>its return encrypted pincode</returns>

        public string Encrypt(string pin)
        {
            string pincode = "";
            foreach (char c in pin)
            {
                if(c >=65 && c<=90)
                {
                    //Capital letters
                   char code = Convert.ToChar(90-(Convert.ToInt32(c)-65));
                   pincode += code;
                }
                else if(c >=48 && c <= 57)
                {
                    //numbers
                    char code = Convert.ToChar(57 - (Convert.ToInt32(c) - 48));
                    pincode += code;
                }
                else if(c >= 97 && c <= 122)
                {
                    //small letters
                    char code = Convert.ToChar(122 - (Convert.ToInt32(c) - 97)); 
                    pincode += code;
                }
               
            }

            return pincode;
        }
        
        /*************************Check Either admin is exist or not*******************************/
        public bool CusExist(Customer cus,out Customer c)
        {
            c = null;
            if (data.CustomerExist(cus,out cus))
            {
                c = cus;
                return true;
            }
            else
            {
                return false;
            }
        }
        /*************************Check Either USER is exist or not*******************************/
        public bool UserExist(User u,int flag,out string type)
        {
            u.Login = Encrypt(u.Login);
            u.PinCode = Encrypt(u.PinCode);
            if (data.UserExist(u,flag,out type))
            {
                return true;
            }
            return false;
        }
        public bool checkMultiple(decimal amount)
        {
            if (amount % 500 == 0)
                return true;
             return false;
        }
        public bool ChecktransferAccount(int AccountId,decimal AddAmount,out string name)
        {
            name = "";
            Customer c=GetCustomerInfo(AccountId);    //GET CUSTOMER TO TRANSFER AMOUNT
            if (c == null) 
            return false;
            name=c.Login;
            name = Encrypt(name);
            return true;
        }
        
        /***************************TRANFER AMOUNT ***************************/
        public void TransferAmount(int AccountId,decimal AddAmount,Customer cus,out string AmountAval)
        {
            AmountAval = "TRUE";
            cus.StartingBalnce -= AddAmount;           //DEDUCT AMOUNT FROM THE CURRENT BALANCE
            if (cus.StartingBalnce < 0)
            {
                AmountAval = "False";
                return;
            }
            
            UpdateAccount(data.GetID(cus,1), cus, 1);
            Customer c = GetCustomerInfo(AccountId);    //GET CUSTOMER TO TRANSFER AMOUNT
            c.StartingBalnce += AddAmount;              //ADD AMOUNT IN BALANCE
            UpdateAccount(AccountId, c,1);              //UPDATE ACCOUNT
        }


    }
    
}
