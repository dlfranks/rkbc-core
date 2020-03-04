using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.services
{
    public class ImageUtility
    {
        private IWebHostEnvironment env;

        public ImageUtility(IWebHostEnvironment _env)
        {
            env = _env;
        }
        
        //assetType = directory name of the stored files, extention: jpg, png,pdf
        public string newAssetFileName(string assetType, string extension)
        {
            var mappedName = "";
            var name = "";
            do
            {
                name = Guid.NewGuid().ToString() + (extension.IndexOf(".") == 0 ? extension : "." + extension);
                mappedName = env.WebRootPath + assetType + "/" + name;
            } while (File.Exists(mappedName));
            return (name);
        }
    }
}
