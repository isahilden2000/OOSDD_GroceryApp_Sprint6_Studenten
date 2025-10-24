using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly GlobalViewModel _global;
        public ObservableCollection<Product> Products { get; set; }

        [ObservableProperty]
        bool isAdmin;

        public ProductViewModel(IProductService productService, GlobalViewModel global)
        {
            _productService = productService;
            _global = global;
            Products = new();
            LoadProducts();
            UpdateIsAdmin();
            _global.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(GlobalViewModel.Client)) UpdateIsAdmin();
            };

            WeakReferenceMessenger.Default.Register<ValueChangedMessage<Product>>(this, (r, m) =>
            {
                if (m?.Value != null)
                {
                    // add product to top of collection
                    Products.Insert(0, m.Value);
                }
            });
        }

        private void UpdateIsAdmin()
        {
            IsAdmin = _global.Client != null && _global.Client.Role == Role.Admin;
        }

        private void LoadProducts()
        {
            Products.Clear();
            foreach (Product p in _productService.GetAll()) Products.Add(p);
        }

        [RelayCommand]
        public async Task NewProduct()
        {
            await Shell.Current.GoToAsync(nameof(Views.NewProductView), true);
        }

        [RelayCommand]
        public async Task DeleteProduct(Product product)
        {
            if (product == null) return;
            if (!IsAdmin) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Verwijderen", $"Weet je zeker dat je '{product.Name}' wilt verwijderen?", "Ja", "Nee");
            if (!confirm) return;

            _productService.Delete(product);
            Products.Remove(product);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            LoadProducts();
        }
    }
}
