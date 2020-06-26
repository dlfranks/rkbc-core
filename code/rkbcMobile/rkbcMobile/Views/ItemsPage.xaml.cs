using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using rkbcMobile.Models;
using rkbcMobile.Views;
using rkbcMobile.ViewModels;
using rkbcMobile.Repository;
using rkbcMobile.ViewModels.Base;

namespace rkbcMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        ViewModelLocator locator;
        public ItemsPage(IUnitOfWork _unitOfWork, ViewModelLocator _locator)
        {
            InitializeComponent();
            locator = _locator;
            BindingContext = viewModel = new ItemsViewModel(_unitOfWork);
        }

        async void OnItemSelected(object sender, EventArgs args)
        {
            var layout = (BindableObject)sender;
            var item = (Item)layout.BindingContext;
            var itemDetailVm = locator.ItemDetail;
            itemDetailVm.Item = item;
            var itemDetailpage = new ItemDetailPage();
            itemDetailpage.BindingContext = itemDetailpage;
            await Navigation.PushAsync(itemDetailpage);
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.IsBusy = true;
        }
    }
}