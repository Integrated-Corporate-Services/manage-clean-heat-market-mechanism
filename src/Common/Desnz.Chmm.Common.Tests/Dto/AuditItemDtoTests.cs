using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.ValueObjects;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Desnz.Chmm.Common.Tests.Dto
{
    public class AuditItemDtoTests
    {
        [Fact]
        internal void ConvertToAuditItemDto_When_ValidAuditItemDetails()
        {
            var @guid = Guid.NewGuid();
            var @object = new
            {
                JsonField = JsonConvert.SerializeObject(new
                {
                    Id = @guid
                }),
                ObjectField = new
                {
                    Id = @guid
                },
                StringField = "Test string",
                IntField = 1,
                FloatField = 1.1,
                DateField = new DateTime(2024, 1, 1),
                StringArray = new List<string> { "Test string 1", "Test string 2" },
                ObjectArray = new List<object> {
                    new
                    {
                        Name = "Test string 1"
                    },
                        new
                    {
                        Name = "Test string 2"
                    }
                },
            };
            var userId = Guid.NewGuid();
            var users = new List<ChmmUserDto>
            {
                new ChmmUserDto
                {
                    Id = userId,
                    Name = "Test user name",
                }
            };

            var auditItem = new AuditItem("", true, "", userId, Guid.NewGuid(), "Test friendly name", "", @object, 1);
            var result = auditItem.ToDto(users);

            result.EventName.Should().Be("Test friendly name");
            result.CreatedBy.Should().Be("Test user name");
            result.AuditItemRows[0].Should().BeEquivalentTo(new AuditItemRow()
            {
                Label = "Json field",
                ObjectValue = new JObject
                {
                    ["Id"] = @guid
                }
            });
            result.AuditItemRows[1].Should().BeEquivalentTo(new AuditItemRow()
            {
                Label = "Object field",
                ObjectValue = new JObject
                {
                    ["Id"] = @guid
                }
            });
            result.AuditItemRows[2].Should().BeEquivalentTo(new AuditItemRow()
            {
                Label = "String field",
                SimpleValue = "Test string"
            });
            result.AuditItemRows[3].Should().BeEquivalentTo(new AuditItemRow()
            {
                Label = "Int field",
                SimpleValue = "1"

            });
            result.AuditItemRows[4].Should().BeEquivalentTo(new AuditItemRow()
            {
                Label = "Float field",
                SimpleValue = "1.1"
            });
            result.AuditItemRows[5].Should().BeEquivalentTo(new AuditItemRow()
            {
                Label = "Date field",
                SimpleValue = "01/01/2024 00:00:00"
            });

            result.AuditItemRows[6].SimpleValue = result.AuditItemRows[6].SimpleValue?.Replace(Environment.NewLine, "");
            result.AuditItemRows[7].SimpleValue = result.AuditItemRows[7].SimpleValue?.Replace(Environment.NewLine, "");

            result.AuditItemRows[6].Should().BeEquivalentTo(new AuditItemRow()
            {
                Label = "String array",
                SimpleValue = "  Test string 1,  Test string 2"
            });
            result.AuditItemRows[7].Should().BeEquivalentTo(new AuditItemRow()
            {
                Label = "Object array",
                SimpleValue = "  {    Name: Test string 1  },  {    Name: Test string 2  }"
            });
        }
    }
}
