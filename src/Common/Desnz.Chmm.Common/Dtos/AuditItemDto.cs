
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
using Desnz.Chmm.Common.ValueObjects;
using Desnz.Chmm.Common.Extensions;

namespace Desnz.Chmm.Common.Dtos
{
    public class AuditItemRow
    {
        public string Label { get; set; }
        public string? SimpleValue { get; set; }
        public JObject? ObjectValue { get; set; }
    }

    public class AuditItemDto
    {
        public string EventName { get; private set; }
        public string CreatedBy { get; private set; }
        public DateTime CreationDate { get; private set; }
        public List<AuditItemRow> AuditItemRows { get; private set; }

        public AuditItemDto(List<ChmmUserDto> users, AuditItem auditItem) 
        {
            EventName = auditItem.FriendlyName;
            CreatedBy = auditItem.CreatedBy;
            if (auditItem.UserId.HasValue)
            {
                CreatedBy = users.Single(u => u.Id == auditItem.UserId.Value).Name;
            }
            CreationDate = auditItem.CreationDate;
            AuditItemRows = new List<AuditItemRow>();
            if (auditItem.Details != null)
            {
                SetAuditItemRows(auditItem.Details);
            }
        }

        private void SetAuditItemRows(string json)
        {

            var jObject = JObject.Parse(json);
            foreach (var kvp in jObject)
            {
                var key = kvp.Key;
                var jToken = kvp.Value;

                if (key == "OrganisationId")
                {
                    continue;
                }

                var auditItemRow = new AuditItemRow()
                {
                    Label = key.GetFriendlyName(),
                };

                if (jToken != null)
                {
                    // Handle cases such as Desnz.Chmm.Identity.Common.Commands -> OnboardManufacturerCommand where 
                    // some fields store objects as json strings
                    try
                    {
                        auditItemRow.ObjectValue = JObject.Parse(jToken.ToString());
                        AuditItemRows.Add(auditItemRow);
                        continue;
                    }
                    catch (JsonReaderException)
                    {
                    }

                    var type = jToken.Type;
                    switch (type)
                    {
                        case JTokenType.Object:
                            auditItemRow.ObjectValue = (JObject)jToken;
                            break;
                        case JTokenType.Integer:
                            auditItemRow.SimpleValue = (jToken.ToObject<int>()).ToString("N0", CultureInfo.CreateSpecificCulture("en-GB"));
                            break;
                        case JTokenType.Float:
                            auditItemRow.SimpleValue = (jToken.ToObject<float>()).ToString("N1", CultureInfo.CreateSpecificCulture("en-GB"));
                            break;
                        case JTokenType.Date:
                            auditItemRow.SimpleValue = (jToken.ToObject<DateTime>()).ToString("dd/MM/yyyy HH:mm:ss");
                            break;
                        default:
                            auditItemRow.SimpleValue = jToken.ToString()
                                .Replace($"[{Environment.NewLine}", "")
                                .Replace($"{Environment.NewLine}]", "")
                                .Replace("\"", "");
                            break;
                    }
                }

                AuditItemRows.Add(auditItemRow);
            }
        }
    }
}
