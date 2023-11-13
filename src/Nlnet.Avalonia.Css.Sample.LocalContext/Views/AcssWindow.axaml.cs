using Avalonia.Controls;

namespace Nlnet.Avalonia.Css.Sample.LocalContext.Views
{
    public partial class AcssWindow : Window
    {
        public AcssWindow()
        {
            AcssContext.UseDefaultContext();

            AcssContext.Default.GetService<IAcssConfiguration>().Accent = "green";

            InitializeComponent();
        }
    }
}
