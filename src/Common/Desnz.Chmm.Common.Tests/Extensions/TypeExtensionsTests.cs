using Amazon.S3.Model;
using Desnz.Chmm.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Desnz.Chmm.Common.Tests.Extensions
{
    public class TypeExtensionsTests
    {
        class MyType { }
        class GetMeMyThingQuery { }
        class UnitTest { }

        [ExcludeFromAudit]
        class DoSomethingCommand { }

        [Fact]
        internal void Type_Friendly_Name_Returns_Correct_Format()
        {
            var type = typeof(MyType);

            var friendlyName = type.GetFriendlyName();

            Assert.Equal("My type", friendlyName);
        }

        [Fact]
        internal void When_Passing_Exclusions_Friendly_Name_Removes_Them()
        {
            var type = typeof(GetMeMyThingQuery);

            var friendlyName = type.GetFriendlyName("Query", "Command");

            Assert.Equal("Get me my thing", friendlyName);
        }

        [Fact]
        internal void Type_Friendly_Name_Mapped_Returns_Correct_Value()
        {
            var type = typeof(UnitTest);

            var friendlyName = type.GetFriendlyName("Query", "Command");

            Assert.Equal("Only for unit testing", friendlyName);
        }

        [Fact]
        internal void When_Attribute_Exists_Has_Attribute_Reutns_True()
        {
            var type = typeof(DoSomethingCommand);

            var hasAttribute = type.HasAttribute<ExcludeFromAuditAttribute>();

            Assert.True(hasAttribute);
        }

        [Fact]
        internal void When_Attribute_Missing_Has_Attribute_Returns_False()
        {
            var type = typeof(GetMeMyThingQuery);

            var hasAttribute = type.HasAttribute<ExcludeFromAuditAttribute>();

            Assert.False(hasAttribute);
        }

        [Fact]
        internal void When_Different_Attribute_Exists_Has_Attribute_Returns_False()
        {
            var type = typeof(DoSomethingCommand);

            var hasAttribute = type.HasAttribute<ClassDataAttribute>();

            Assert.False(hasAttribute);
        }
    }
}
