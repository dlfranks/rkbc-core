using rkbcMobile.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using rkbcMobile.Repository;

namespace rkbcMobile.ViewModels
{
    public class PostItemsViewModel : BaseViewModel
    {
        public ObservableCollection<PostItem> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public PostItemsViewModel(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            Title = "Posts";
            Items = new ObservableCollection<PostItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            //MessagingCenter.Subscribe<NewItemPage, PostItem>(this, "AddItem", async (obj, item) =>
            //{
            //    var newItem = item as Item;
            //    Items.Add(newItem);
            //    await DataStore.AddItemAsync(newItem);
            //});
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await unitOfWork.PostData.GetItemsAsync(true);
                foreach (var item in items)
                {
                    if (item.postType == (int)BlogPostType.Video)
                    {
                        item.imageUrl = "https://img.youtube.com/vi/" + item.getVideoId() + "/default.jpg";
                    }
                    else if (item.postType == (int)BlogPostType.Sigle)
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
