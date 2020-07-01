using rkbcMobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace rkbcMobile.Services.Blog
{
    public interface IBlogService
    {
       Task<ObservableCollection<Item>> GetItemsAsync();
    
    }
}
