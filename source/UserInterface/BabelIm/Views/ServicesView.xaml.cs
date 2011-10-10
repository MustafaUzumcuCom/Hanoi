using System.Windows.Controls;
using BabelIm.ViewModels;

namespace BabelIm.Views {
    /// <summary>
    ///   Activity view
    /// </summary>
    public partial class ServicesView
        : UserControl {
        public ServicesView() {
            InitializeComponent();

            DataContext = new ServicesViewModel();
        }
        }
}