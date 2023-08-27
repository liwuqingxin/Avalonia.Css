using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Nlnet.Avalonia.Controls;

namespace Samples.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var result= MessageBox.Show(
            TbxMessage.Text,
            TbxTitle.Text,
            (Buttons)(CbxButtons.SelectedItem ?? Buttons.OK),
            (Images)(CbxImages.SelectionBoxItem ?? Images.Success));

        MessageBox.Show($"You selected {result}.", "Note", Buttons.OK, Images.Info);
    }

    private void ButtonChangeTheme_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.VisualRoot is not Window window)
        {
            return;
        }

        window.RequestedThemeVariant = window.ActualThemeVariant == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;

        //
        // Try to change application' theme.
        //
        //if (Application.Current == null)
        //{
        //    return;
        //}

        //Application.Current.RequestedThemeVariant = Application.Current.ActualThemeVariant == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
    }
}
