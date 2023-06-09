namespace Nlnet.Avalonia.Css;

public class DoubleResource : CssResource<DoubleResource>
{
    protected override object? Accept(string valueString)
    {
        if (double.TryParse(valueString, out var doubleValue))
        {
            return doubleValue;
        }

        return null;
    }
}