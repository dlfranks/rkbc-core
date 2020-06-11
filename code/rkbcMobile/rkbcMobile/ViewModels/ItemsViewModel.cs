﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using rkbcMobile.Models;
using rkbcMobile.Views;
using rkbcMobile.Services;
using rkbcMobile.Repository;

namespace rkbcMobile.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            Title = "Posts";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Item;
                Items.Add(newItem);
                await unitOfWork.ItemData.AddItemAsync(newItem);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await unitOfWork.ItemData.GetItemsAsync(true);
                foreach (var item in items)
                {
                    if(item.postType == (int)BlogPostType.Video)
                    {
                       item.imageUrl = "https://img.youtube.com/vi/" + item.getVideoId() + "/default.jpg";
                    }else if(item.postType == (int)BlogPostType.Sigle)
                    {
                        item.imageUrl = App.AzureBackendUrl + "/assets/blog/clickHereImage.jpg";
                    }
                    else
                    {
                        item.imageUrl = "http://rkbc.us" + item.imageUrl;
                    }
                        
                    Items.Add(item);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}