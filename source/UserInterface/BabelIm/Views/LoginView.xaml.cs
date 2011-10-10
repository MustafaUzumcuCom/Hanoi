using System.Windows.Controls;
using BabelIm.ViewModels;

namespace BabelIm.Views {
    /// <summary>
    ///   Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView
        : UserControl {
        public LoginView() {
            InitializeComponent();

            DataContext = new LoginViewModel();
        }
        }
}