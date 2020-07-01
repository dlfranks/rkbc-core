using System;

using rkbcMobile.Models;
using rkbcMobile.Repository;
using rkbcMobile.ViewModels.Base;

namespace rkbcMobile.ViewModels
{
    public class ItemDetailViewModel : ViewModelBase
    {
        public Item Item { get; set; }
        public ItemDetailViewModel()
        {

           
        }
    }
}
