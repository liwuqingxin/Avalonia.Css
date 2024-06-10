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
            
            AppendConfiguration(builder, SpacingList,        "space");
            AppendConfiguration(builder, OrientationList,    "orientation");
            AppendConfiguration(builder, AlignmentList,      "align-items");
            AppendConfiguration(builder, JustifyContentList, "justify-content");
            AppendConfiguration(builder, AlignContentList,   "align-content");
            AppendConfiguration(builder, FlexWrapList,       "flex-wrap");
            
            TbxLayoutStyle.Text = builder.ToString();
            TbxLayout.Text = LayoutList.SelectedItem?.ToString();
        }

        private void AppendConfiguration(StringBuilder builder, ComboBox comboBox, string property)
        {
            var value = comboBox.SelectedItem?.ToString()?.Replace("_","");
            if (string.IsNullOrEmpty(value) == false)
            {
                builder.Append($"{property}:{value};");
            }
        }
    }
}
