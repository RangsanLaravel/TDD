using System;
using System.Collections.Generic;

namespace DemoATM
{
    public interface IAccountDAC
    {
        IEnumerable<Account> GetAllAccounts();
    }
}
