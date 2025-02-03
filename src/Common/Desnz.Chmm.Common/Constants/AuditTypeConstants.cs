namespace Desnz.Chmm.Common.Constants
{
    public class AuditTypeConstants
    {
        public static string Command = "Command";
        public static string Query = "Query";
        public static string Unknown = "Unknown";

        public static string GetAuditType(string typeName)
        {
            if (typeName.EndsWith(Command))
            {
                return Command;
            }
            else if (typeName.EndsWith(Query))
            {
                return Query;
            }
            else
            {
                return Unknown;
            }
        }
    }
}
