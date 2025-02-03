using Desnz.Chmm.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Desnz.Chmm.Common.Tests.Extensions
{
    public class ListExtensionsTests
    {
        [Fact]
        public void When_LessThanOnePage_Return_OnePage()
        {
            var listLength = 3;
            var list = Enumerable.Range(1, listLength).ToList();
            var page = list.ToPage(4, 1);

            Assert.False(page.HasNextPage);
            Assert.Equal(3, page.Result.Count);
            Assert.Equal(1, page.PageNumber);
        }

        [Fact]
        public void When_ManyPages_Return_Page_Two()
        {
            var listLength = 50;
            var list = Enumerable.Range(1, listLength).ToList();
            var page = list.ToPage(4, 2);

            Assert.True(page.HasNextPage);
            Assert.Equal(4, page.Result.Count);
            Assert.Equal(2, page.PageNumber);
        }

        [Fact]
        public void Whe_TwoPages_Page_Two_Is_Correct()
        {
            var listLength = 5;
            var list = Enumerable.Range(1, listLength).ToList();
            var page = list.ToPage(4, 2);

            Assert.False(page.HasNextPage);
            Assert.Equal(1, page.Result.Count);
            Assert.Equal(2, page.PageNumber);

            Assert.Equal(5, page.Result.First());
        }
    }
}
