using Avalonia.Controls;
using Nlnet.Avalonia.SampleAssistant;
using Avalonia.Interactivity;
using System.Text;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
    public enum Orientations { H, V, Horizontal, Vertical, }

    public enum Spacings { _5 = 5, _10 = 10, _20 = 20, _30 = 30, }
    
    public enum Layouts { Stack, Wrap, Canvas, Flex, Grid }
    
    [GalleryItem( GalleryItemKind.View, "MagicPanel", null)]
    public partial class MagicPanelPage : UserControl
    {
        public MagicPanelPage()
        {
            InitializeComponent();
        }


        private void List_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            UpdateLayoutAndStyles();
        }
        
        private void CbxReverse_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
        {
            UpdateLayoutAndStyles();
        }

        private void UpdateLayoutAndStyles()
        {
            var builder = new StringBuilder();

            builder.Append(CbxReverse.IsChecked == true ? "reverse;" : "reverse:false;");

            var spacing = SpacingList.SelectedItem?.ToString().Substring(1);
            if (string.IsNullOrEmpty(spacing) == false)
            {
                builder.Append($"space:{spacing};");
            }

            var orientation = OrientationList.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(orientation) == false)
            {
                builder.Append($"orientation:{orientation};");
            }

            var alignment = AlignmentList.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(alignment) == false)
            {
                builder.Append($"align-items:{alignment};");
            }

            var justifyContent = JustifyContentList.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(justifyContent) == false)
            {
                builder.Append($"justify-content:{justifyContent};");
            }
            
            var alignContent = AlignContentList.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(alignContent) == false)
            {
                builder.Append($"align-content:{alignContent};");
            }
            
            TbxLayoutStyle.Text = builder.ToString();
            TbxLayout.Text = LayoutList.SelectedItem?.ToString();
        }
    }
}
