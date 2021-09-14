
using UWPSocketServer.ViewModels;

using Windows.UI.Xaml.Controls;

namespace UWPSocketServer.Views
{
    public sealed partial class BlankPage : Page
    {
        public BlankViewModel ViewModel { get; } = new BlankViewModel();

        public BlankPage()
        {
            InitializeComponent();
        }
    }
}
