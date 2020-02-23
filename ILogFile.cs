using System;
namespace DemoATM
{
    public interface ILogFile
    {
        void WriteWithdraw(string username, double amount);
        DateTime GetCurrentDate(int number);
    }
}
