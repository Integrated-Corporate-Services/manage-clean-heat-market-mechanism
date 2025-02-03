[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExcludeFromAuditAttribute : Attribute
{
    public ExcludeFromAuditAttribute() { }
}
