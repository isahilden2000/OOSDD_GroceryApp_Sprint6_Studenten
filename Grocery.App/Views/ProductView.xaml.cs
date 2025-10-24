using Grocery.App.ViewModels;

namespace Grocery.App.Views;

public partial class ProductView : ContentPage
{
	public ProductView(ProductViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

		// add toolbar item dynamically based on IsAdmin
		if (viewModel.IsAdmin)
		{
			var toolbar = new ToolbarItem { Text = "Nieuw" };
			toolbar.Clicked += OnNewProductClicked;
			ToolbarItems.Add(toolbar);
		}

		// listen for changes
		viewModel.PropertyChanged += (s, e) =>
		{
			if (e.PropertyName == nameof(viewModel.IsAdmin))
			{
				if (viewModel.IsAdmin && !ToolbarItems.Any(t => t.Text == "Nieuw"))
				{
					var toolbar = new ToolbarItem { Text = "Nieuw" };
					toolbar.Clicked += OnNewProductClicked;
					ToolbarItems.Add(toolbar);
				}
				else if (!viewModel.IsAdmin)
				{
					var existing = ToolbarItems.FirstOrDefault(t => t.Text == "Nieuw");
					if (existing != null) ToolbarItems.Remove(existing);
				}
			}
		};
	}

	private async void OnNewProductClicked(object sender, EventArgs e)
	{
		if (BindingContext is ProductViewModel vm)
		{
			await vm.NewProductCommand.ExecuteAsync(null);
		}
	}
}