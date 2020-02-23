using System;
using Moq;
using Xunit;

namespace DemoATM.Test
{
    public class DemoMock
    {
        public DemoMock()
        {
        }
        [Fact]
        public void DemoMock01()
        {
            var mock =new Moq.MockRepository(Moq.MockBehavior.Default);
           var logMocking = mock.Create<ILogFile>();
            //  var log = logMocking.Object;
            //log.WriteWithdraw("sakul123", 199);

            var expectedDate = DateTime.Now;

            logMocking.Setup(it => it.GetCurrentDate(It.IsAny<int>()))
                .Returns<int>(number =>
                {
                    if (number == 1) return expectedDate;
                    else if (number == 2) return expectedDate.AddDays(1);
                    else return DateTime.Now;
                }
                );
            var log = logMocking.Object;

            var currentTime = log.GetCurrentDate(1);
            // logMocking.Verify(it => it.WriteWithdraw(
            //    It.Is<string>(acut => acut == "sakul"),
            //    It.Is<double>(acut => acut == 199)));

            Assert.Equal(expectedDate, currentTime);
        }
    }
}
