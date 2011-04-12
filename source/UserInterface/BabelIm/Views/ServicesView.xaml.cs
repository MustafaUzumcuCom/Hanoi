using System.Windows.Controls;
using BabelIm.ViewModels;

namespace BabelIm.Views
{
    /// <summary>
    /// Activity view
    /// </summary>
    public partial class ServicesView 
        : UserControl
    {
        #region · Constructors ·

        public ServicesView()
        {
            InitializeComponent();

            this.DataContext = new ServicesViewModel();
        }

        #endregion
    }
}
