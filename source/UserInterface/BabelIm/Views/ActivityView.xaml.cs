using System.Windows;
using System.Windows.Controls;
using BabelIm.ViewModels;

namespace BabelIm.Views
{
    /// <summary>
    /// Activity view
    /// </summary>
    public partial class ActivityView 
        : UserControl    
    {
        #region · Constructors ·

        public ActivityView()
        {
            InitializeComponent();

            this.DataContext = new ActivityViewModel();
        }

        #endregion

        #region · Event Handlers ·

        private void SetMoodButton_Click(object sender, RoutedEventArgs e)
        {
#warning TODO: Move this to the ViewModel using Commands
            ((ActivityViewModel)this.DataContext).PublishMood();
        }

        #endregion
    }
}
