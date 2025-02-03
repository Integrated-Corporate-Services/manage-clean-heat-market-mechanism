namespace Desnz.Chmm.Common.Attributes.SourceGeneration;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class StateAttribute : Attribute
{
    public string Variable { get; private set; }

    public StateAttribute(string variable)
    {
        Variable = variable;
    }
}
