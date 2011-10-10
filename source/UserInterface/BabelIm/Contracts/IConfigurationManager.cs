using System.Windows.Input;
using BabelIm.Configuration;

namespace BabelIm.Contracts {
    internal interface IConfigurationManager {
        BabelImConfiguration Configuration { get; }

        ICommand OpenAccountPreferencesViewCommand { get; }

        ICommand OpenPreferencesViewCommand { get; }

        ICommand OpenServerPreferencesViewCommand { get; }

        Account SelectedAccount { get; set; }

        BabelImConfiguration GetConfiguration();
    }
}