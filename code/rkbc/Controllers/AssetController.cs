using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rkbc.core.helper;

namespace rkbc.web.controllers
{
    public class AssetController : Controller
    {
        private FileHelper fileHelper;
        public AssetController(FileHelper _fileHelper)
        {
            fileHelper = _fileHelper;
        }
        public async Task<FileResult> ImageView(string type, string fileName, bool thumbnail = false)
        {
            var byteImage = await fileHelper.assetToByteArray(type, fileName);
            
            return (File(byteImage, "image/jpeg"));
        }
    }
}