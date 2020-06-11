using rkbcMobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace rkbcMobile.Repository
{
    public interface IUnitOfWork
    {
        IDataStore<Item> ItemData { get; }
        IDataStore<PostItem> PostData { get; }
    }

}
