using System;

using rkbcMobile.Models;

namespace rkbcMobile.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ItemDetailViewModel(Item item = null)
        {

            Title = item?.title;
            Item = item;
        }
    }
}
