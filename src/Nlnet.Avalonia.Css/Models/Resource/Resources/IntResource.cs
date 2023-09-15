namespace Nlnet.Avalonia.Css;

[ResourceType("int")]
internal class IntResource : AcssResourceBaseAndFac<IntResource>
{
    protected override object? BuildValue(IAcssBuilder acssBuilder, string valueString)
    {
        return valueString.TryParseInt();
    }
}