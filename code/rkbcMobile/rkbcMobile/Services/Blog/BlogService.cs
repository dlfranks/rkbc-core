using rkbcMobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace rkbcMobile.Services.Blog
{
    public class BlogService : IBlogService
    {
        public Task<ObservableCollection<Item>> GetItemsAsync()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await unitOfWork.ItemData.GetItemsAsync(true);
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
