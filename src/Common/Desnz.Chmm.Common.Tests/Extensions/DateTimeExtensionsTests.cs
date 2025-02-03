using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Desnz.Chmm.Common.Extensions;
using Xunit;

namespace Desnz.Chmm.Common.Tests.Extensions
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void DeconstructingDate_Gives_CorrectDateAndTime()
        {
            var dateTime = new DateTime(2023, 02, 01, 12, 32, 42);

            var (date, time) = dateTime;

            Assert.Equal(new DateOnly(2023, 02, 01), date);
            Assert.Equal(new TimeOnly(12, 32, 42), time);
        }
    }
}
