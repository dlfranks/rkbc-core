using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using rkbcMobile.Models;
using rkbcMobile.Views;
using rkbcMobile.Services;
using rkbcMobile.Repository;
using rkbcMobile.ViewModels.Base;
using rkbcMobile.Services.Settings;
using rkbcMobile.Services.Blog;
using System.Windows.Input;

namespace rkbcMobile.ViewModels
{
    public class ItemsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IBlogService _blogService;
        private ObservableCollection<Item> _items;
        public ObservableCollection<Item> Items {
            get => _items;
            set
            {
                _items = value;
                RaisePropertyChanged(() => Items);
            }
        }
        public Command LoadItemsCommand { get; set; }
        public string Title { get; set; }
        public ItemsViewModel(ISettingsService settingsService, IBlogService blogService) 
        {
            _settingsService = settingsService;
            _blogService = blogService;
            Title = "Posts";
            Items = new ObservableCollection<Item>();
            //MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            //{
            //    var newItem = item as Item;
            //    Items.Add(newItem);
            //    await unitOfWork.ItemData.AddItemAsync(newItem);
            //});
        }

        public ICommand GetItemDetailCommand => new Command<Item>(async (item) => await GetItemDetailAsync(item));
        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            // Get campaigns by user
            Items = await _blogService.GetItemsAsync();
            IsBusy = false;
        }

        private async Task GetItemDetailAsync(Item item)
        {
            await NavigationService.NavigateToAsync<ItemDetailViewModel>(item.id);
        }
        
    }
}