using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoATM
{
    public class ATMController
    {
        public ATMController(ILogFile logFile,IAccountDAC accountDAC)
        {
            this.log = logFile;
            this.dac = accountDAC;
        }
        private readonly ILogFile log;
        private readonly IAccountDAC dac;
        
        public bool Withdraw(string username,double amount)
        {
         var selectedAccount=   GetAccountByUsername(username);
            const int MinWithdrawAmount = 1;
            var isWithdrawRequestValid =selectedAccount!= null
                && selectedAccount.Balance >=amount
                && amount >=MinWithdrawAmount;
            if (!isWithdrawRequestValid) return false;
            selectedAccount.Balance -= amount;
            log.WriteWithdraw(username,amount);
            return isWithdrawRequestValid;
        }
        public Account GetAccountByUsername(string Username)
        => dac.GetAllAccounts().FirstOrDefault(it => it.Username == Username);
        
    }
}
