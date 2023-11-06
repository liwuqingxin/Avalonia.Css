namespace Nlnet.Avalonia.Css;

[ResourceType("int")]
internal class IntResource : AcssResourceBaseAndFac<IntResource>
{
    protected override object? BuildValue(IAcssContext context, string valueString)
    {
        return valueString.TryParseInt();
    }
}