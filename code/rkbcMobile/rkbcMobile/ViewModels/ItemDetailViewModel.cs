using System;

using rkbcMobile.Models;
using rkbcMobile.Repository;

namespace rkbcMobile.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ItemDetailViewModel(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {

           
        }
    }
}
