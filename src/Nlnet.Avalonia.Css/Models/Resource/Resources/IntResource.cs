namespace Nlnet.Avalonia.Css;

[ResourceType("int")]
internal class IntResource : AcssResourceBaseAndFac<IntResource>
{
    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
    {
        if (int.TryParse(valueString, out var intValue))
        {
            return intValue;
        }

        this.WriteError($"Can not parse int from string '{valueString}'.");
        return null;
    }
}