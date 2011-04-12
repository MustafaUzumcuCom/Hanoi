using System.Windows.Input;
using BabelIm.Configuration;

namespace BabelIm.Contracts
{
    interface IConfigurationManager
    {
        #region · Properties ·

        BabelImConfiguration Configuration
        {
            get;
        }

        ICommand OpenAccountPreferencesViewCommand
        {
            get;
        }

        ICommand OpenPreferencesViewCommand
        {
            get;
        }

        ICommand OpenServerPreferencesViewCommand
        {
            get;
        }

        Account SelectedAccount
        {
            get;
            set;
        }

        #endregion

        #region · Constructors ·

        BabelImConfiguration GetConfiguration();

        #endregion
    }
}
