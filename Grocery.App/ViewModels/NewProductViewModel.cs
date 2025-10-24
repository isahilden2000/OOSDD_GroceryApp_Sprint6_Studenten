using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;
using System.Linq;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly GlobalViewModel _global;

        [ObservableProperty]
        string name;

        [ObservableProperty]
        int stock;

        [ObservableProperty]
        DateOnly shelfLife = DateOnly.MinValue;

        [ObservableProperty]
        decimal price;

        public ObservableCollection<Category> Categories { get; set; } = new();

        // explicit property instead of source-generated one (avoids generator timing issues)
        public Category SelectedCategory { get; set; }

        public NewProductViewModel(IProductService productService, ICategoryService categoryService, IProductCategoryService productCategoryService, GlobalViewModel global)
        {
            _productService = productService;
            _categoryService = categoryService;
            _productCategoryService = productCategoryService;
            _global = global;
            Title = "Nieuw product";

            LoadCategories();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            // add an empty option
            Categories.Add(new Category(0, "Geen"));
            foreach (var c in _categoryService.GetAll())
            {
                Categories.Add(c);
            }
            SelectedCategory = Categories.FirstOrDefault();
        }

        [RelayCommand]
        public async Task Save()
        {
            if (_global.Client == null || _global.Client.Role != Role.Admin) return;
            Product p = new(0, Name ?? string.Empty, Stock, ShelfLife, Price);
            Product added = _productService.Add(p);

            // link category if selected and not the 'Geen' option
            if (SelectedCategory != null && SelectedCategory.Id != 0)
            {
                ProductCategory pc = new(0, SelectedCategory.Id, added.Id);
                _productCategoryService.Add(pc);
            }

            // notify listeners that a product was added
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Product>(added));

            // navigate back
            await Shell.Current.GoToAsync("..", true);
        }
    }
}
