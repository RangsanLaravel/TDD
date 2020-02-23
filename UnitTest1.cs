using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace DemoATM.Test
{
    public class UnitTest1
    {
        private readonly ATMController sut ;
        private readonly Mock<ILogFile> logMocking;
        public UnitTest1()
        {
            var mock = new Moq.MockRepository(MockBehavior.Default);
            
            logMocking = mock.Create<ILogFile>();
            var dac = mock.Create<IAccountDAC>();
            sut = new ATMController(logMocking.Object,dac.Object);
            var accounts = new List<Account>()
            {
                new Account(){Username="john",Balance=500},
                new Account(){Username="Doe",Balance=50},
            };
            dac.Setup(dac => dac.GetAllAccounts())
                .Returns(accounts);
          
          //  this.sut = sut;
        }
        [Theory(DisplayName = "ผู้ใช้กดเงินออกจากตู้ ถูกต้อง ระบบทำการหักเงินออก")]
        [InlineData("john",500,0)]
        [InlineData("john",450,50)]
        [InlineData("john",1,499)]
        public void WithdrawWithAllDataCorrectSystemAcceptTheRest(string username,double withdrawAmount,double expectedMoney)
        {
           
            var actual = sut.Withdraw(username,withdrawAmount);
            Assert.True(actual);
            var selectedAccount = sut.GetAccountByUsername(username);
            Assert.Equal(expectedMoney, selectedAccount.Balance);

            logMocking.Verify(log => log.WriteWithdraw(
                It.Is<string>(it => it == username),
                It.Is<double>(it => it == withdrawAmount)));
        }
        [Theory(DisplayName = "ผู้ใช้กดเงินออกจากตู้ไม่ถูกต้อง ระบบทำการแจ้งเตือน")]
        [InlineData("john", 0, 500)]
        [InlineData("john", -1, 500)]
        public void WithdrawWithSomeDataIncorrectThenSystemMustNotAcceptTheRest(string username, double withdrawAmount, double expectedMoney)
        {
            
            var actual = sut.Withdraw(username, withdrawAmount);
            Assert.False(actual);
            var selectedAccount = sut.GetAccountByUsername(username);
            Assert.Equal(expectedMoney, selectedAccount.Balance);

            logMocking.Verify(log => log.WriteWithdraw(
                It.IsAny<string>(),
                It.IsAny<double>()),
                Times.Never());
        }

        [Theory(DisplayName = "ผู้ใช้กดเงินออกจากตู้ไม่ถูกต้อง ระบบทำการแจ้งเตือน")]
        [InlineData("Unknow", 100)]
        [InlineData(null, 100)]
        [InlineData("", 100)]
        public void InvalidUserTryToWithdrawThenSystemMustNotAcceptTheResult(string username, double withdrawAmount)
        {

            var actual = sut.Withdraw(username, withdrawAmount);
            Assert.False(actual);
            var selectedAccount = sut.GetAccountByUsername(username);
            Assert.Null(selectedAccount);
            logMocking.Verify(log => log.WriteWithdraw(
              It.IsAny<string>(),
              It.IsAny<double>()),
              Times.Never());
        }
        [Theory(DisplayName = "ผู้ใช้กดเงินออกจากระบบเงินในบัญชีไม่พอ ระบบทำการแจ้งเตือน")]
        [InlineData("Doe",100,50)]
        public void WithdrawWhenBalanceIsNotEnoughtThenSystemMustNotAcceptTheResult(string username, double withdrawAmount, double expectedMoney)
        {
            var actual = sut.Withdraw(username, withdrawAmount);
            Assert.False(actual);
            var selectedAccount = sut.GetAccountByUsername(username);
            Assert.Equal(expectedMoney,selectedAccount.Balance);
            logMocking.Verify(log => log.WriteWithdraw(
              It.IsAny<string>(),
              It.IsAny<double>()),
              Times.Never());
        }
    }
}
//Normal cases
//1.ผู้ใช้กดเงินออกจากตู้ ถูกต้อง ระบบทำการหักเงินออก
//2.ผู้ใช้กดเงินออกจากระบบเงินในบัญชีไม่พอ ระบบทำการแจ้งเตือน
//3.ผู้ใช้กดเงินออกจากตู้ถูกต้อง แต่ตู้มีเงินไม่พอ ระบบทำการแจ้งเตือน

//Alternative Cases
//4.ผู้ใช้บัญชีพิเศษกดเงินมากกว่าที่มีในบัญชี ระบบบันทึกเครดิตแล้วนำเงินออกมา
//5.ผู้ใช้บัญชีพิเศษกดเงินมากกว่าที่มีในบัญชี แต่เครดิตเต็มแล้ว ระบบทำการแจ้งเตือน

//Exception Cases
//6.ผู้ใช้กดเงินสำเร็จแต่ระบบไม่สามารถหักเงินในบัญชีได้ ระบบทำการแจ้งเตือน