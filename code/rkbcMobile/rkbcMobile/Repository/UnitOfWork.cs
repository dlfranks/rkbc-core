using rkbcMobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace rkbcMobile.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IDataStore<Item> _ItemData = null;

        public IDataStore<PostItem> _PostData = null;

        public IDataStore<Item> ItemData
        {
            get
            {
                if(_ItemData == null)
                {
                    _ItemData = new DataStore<Item>("Item");
                }
                return _ItemData;
            }
        }
        public IDataStore<PostItem> PostData
        {
            get
            {
                if (_PostData == null)
                {
                    _PostData = new DataStore<PostItem>("Post");
                }
                return _PostData;
            }
        }
    }
}
