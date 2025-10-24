using CommunityToolkit.Mvvm.ComponentModel;
using Grocery.Core.Models;

namespace Grocery.App.ViewModels
{
    public partial class GlobalViewModel : BaseViewModel
    {
        [ObservableProperty]
        Client client;
    }
}
