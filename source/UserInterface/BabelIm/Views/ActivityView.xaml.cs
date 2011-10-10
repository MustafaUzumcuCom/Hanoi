using System.Windows;
using System.Windows.Controls;
using BabelIm.ViewModels;

namespace BabelIm.Views {
    /// <summary>
    ///   Activity view
    /// </summary>
    public partial class ActivityView
        : UserControl {
        public ActivityView() {
            InitializeComponent();

            DataContext = new ActivityViewModel();
        }

        private void SetMoodButton_Click(object sender, RoutedEventArgs e) {
#warning TODO: Move this to the ViewModel using Commands
            ((ActivityViewModel) DataContext).PublishMood();
        }
        }
}