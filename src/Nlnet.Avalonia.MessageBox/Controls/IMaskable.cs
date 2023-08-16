namespace Nlnet.Avalonia.Controls;

/// <summary>
/// Make the window a maskable that can show a mask when child MessageBox is shown.
/// </summary>
public interface IMaskable
{
    /// <summary>
    /// Show mask in the window when show a MessageBox onto it.
    /// </summary>
    void ShowMask();

    /// <summary>
    /// Hide the mask that is shown in owner window of MessageBox.
    /// </summary>
    void HideMask();
}