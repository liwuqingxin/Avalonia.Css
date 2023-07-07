namespace Nlnet.Avalonia.Css.App.Controls;

public class Reference<T>
{
    public Reference()
    {

    }

    public Reference(T? obj)
    {
        Object = obj;
    }

    public T? Object { get; set; }
}
