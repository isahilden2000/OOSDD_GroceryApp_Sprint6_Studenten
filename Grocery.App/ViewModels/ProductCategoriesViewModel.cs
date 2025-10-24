using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    [QueryProperty(nameof(Category), nameof(Category))]
    public partial class ProductCategoriesViewModel : BaseViewModel
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        private readonly GlobalViewModel _global;
        private string searchText = "";
        public ObservableCollection<ProductCategory> ProductCategories { get; set; } = new();
        public ObservableCollection<Product> AvailableProducts { get; set; } = new();

        [ObservableProperty]
        Category category;

        [ObservableProperty]
        bool isAdmin;

        public ProductCategoriesViewModel(IProductCategoryService productCategoryService, IProductService productService, GlobalViewModel global)
        {
            _productCategoryService = productCategoryService;
            _productService = productService;
            _global = global;
            UpdateIsAdmin();
            _global.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(GlobalViewModel.Client)) UpdateIsAdmin();
            };
        }

        private void UpdateIsAdmin()
        {
            IsAdmin = _global.Client != null && _global.Client.Role == Role.Admin;
        }

        partial void OnCategoryChanged(Category? oldValue, Category newValue)
        {
            ProductCategories.Clear();
            foreach (var p in _productCategoryService.GetAllOnCategoryId(newValue.Id)) ProductCategories.Add(p);
            GetAvailableProducts();
        }

        private void GetAvailableProducts()
        {
            AvailableProducts.Clear();
            foreach (Product p in _productService.GetAll())
                if (ProductCategories.FirstOrDefault(pc => pc.ProductId == p.Id) == null)
                    if (searchText=="" || p.Name.Contains(searchText)) AvailableProducts.Add(p);

        }

        [RelayCommand]
        public void AddProduct(Product product)
        {
            if (product == null) return;
            ProductCategory item = new(0, Category.Id, product.Id);
            item.Product = product;
            item.Category = Category;
            _productCategoryService.Add(item);
            AvailableProducts.Remove(product);
            OnCategoryChanged(null, Category);
        }

        [RelayCommand]
        public void PerformSearch(string searchText)
        {
            this.searchText = searchText;
            GetAvailableProducts();
        }

        [RelayCommand]
        public void DeleteProductCategory(ProductCategory pc)
        {
            if (pc == null) return;
            if (!IsAdmin) return;
            _productCategoryService.Delete(pc);
            // refresh lists
            OnCategoryChanged(null, Category);
        }
    }
}
