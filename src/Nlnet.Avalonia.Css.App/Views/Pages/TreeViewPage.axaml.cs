using Avalonia.Controls;
using Nlnet.Avalonia.SampleAssistant;
using System.ComponentModel;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
    [GalleryItem("TreeView")]
    public partial class TreeViewPage : UserControl
    {
        public TreeViewPage()
        {
            DataContext = new TreeViewPageViewModel();
            InitializeComponent();
        }
    }
}
