using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using rkbc.core.helper;
using System.Text.RegularExpressions;

namespace rkbc.core.helper
{
    public class FileHelper
    {
        private IWebHostEnvironment env;
        private IConfiguration config;

        public FileHelper(IWebHostEnvironment _env, IConfiguration _config)
        {
            env = _env;
            config = _config;
            
        }
        private string[] getFileNameAndExtension(string assetFileName)
        {
            var name = assetFileName;
            var extension = "";
            var i = assetFileName.LastIndexOf('.');
            if (i == 0)
            {
                name = "";
                extension = assetFileName.Substring(i);
            }
            if (i > 0)
            {
                name = assetFileName.Substring(0, i);
                extension = assetFileName.Substring(i + 1);
            }
            return new string[] { name, extension };
        }
        public string getFileName(string assetFileName)
        {
            return getFileNameAndExtension(assetFileName)[0];
        }
        public string getExtension(string assetFileName)
        {
            return getFileNameAndExtension(assetFileName)[1];
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
        private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
            {".jpe", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".pdf", "application/pdf"},
            {".png", "image/png"},
        };

        private string[] fileNameAndExtension(string assetFileName)
        {
            var name = assetFileName;
            var extension = "";
            var i = assetFileName.LastIndexOf('.');
            if (i == 0)
            {
                name = "";
                extension = assetFileName.Substring(i);
            }
            if (i > 0)
            {
                name = assetFileName.Substring(0, i);
                extension = assetFileName.Substring(i);
            }
            return new string[] { name, extension };
        }

        public string assetMimeType(string assetFileName)
        {
            var extension = fileNameAndExtension(assetFileName)[1];

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;
            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        protected string assetTypeToPath(string assetType)
        {
            var path = "";
            switch (assetType)
            {
                case "banner":
                    path = "banner";
                    break;
                case "gallery":
                    path = "gallery";
                    break;
                case "blog":
                    path = "blog";
                    break;

                default:
                    throw new InvalidOperationException("Invalid office asset type " + assetType + " specified.");
            }
            return (path);
        }

        public string mapAssetPath(string assetType, string assetFileName, bool thumbnail)
        {
           var path = assetTypeToPath(assetType);
            
            if (thumbnail) assetFileName = ImageHelper.GetThumbnailFileName(assetFileName);
            path = env.WebRootPath + "\\assets\\" + path + "\\" + assetFileName;
            Console.WriteLine("env.WebRootPath: " + env.WebRootPath);
            Console.WriteLine("Full path: " + path);
            return (path);
        }
        public string generateAssetURL(string assetType, string assetFileName)
        {
            if (String.IsNullOrWhiteSpace(assetFileName)) return "";
            return ("/assets/" + assetType + "/" + assetFileName);
        }
        public string generateAssetURL(string assetType, string assetFileName, bool thumbnail)
        {
            if (String.IsNullOrWhiteSpace(assetFileName)) return "";
            var file = fileNameAndExtension(assetFileName);
            if(thumbnail) return ("/assets/" + assetType + "/" + file[0] + "-thumb" + file[1]);
            return ("/assets/" + assetType + "/" + assetFileName);
        }

        public void deleteAsset(string assetType, string assetFileName, bool throwException)
        {
            var assetName = mapAssetPath(assetType, assetFileName, false);
            var assetThumbName = mapAssetPath(assetType, assetFileName, true);

            try
            {
                if (File.Exists(assetName)) System.IO.File.Delete(assetName);
            }
            catch (Exception e)
            {
                if (throwException) throw e;
                else
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new InvalidOperationException("Unable to delete office asset " + assetName, e));
                }
            }

            try
            {
                if (File.Exists(assetThumbName)) System.IO.File.Delete(assetThumbName);
            }
            catch (Exception e)
            {
                if (throwException) throw e;
                else
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new InvalidOperationException("Unable to delete office asset thumb " + assetThumbName, e));
                }
            }
        }

        public Bitmap assetToImage(string assetType, string assetFileName)
        {
            var assetName = mapAssetPath(assetType, assetFileName, false);
            Bitmap img = null;

            if (File.Exists(assetName))
            {
                try
                {
                    img = new Bitmap(assetName);
                }
                catch (Exception ex)
                {
                    //Gobble and just return missing or invalid... 
                }
            }

            return (img);
        }

        public async Task<byte[]> assetToByteArray(string assetType, string assetFileName)
        {
            var assetName = mapAssetPath(assetType, assetFileName, false);
            byte[] contents = null;

            //if (File.Exists(assetName))
            //{
                using (var f = File.Open(assetName, FileMode.Open))
                {
                    try
                    {
                        contents = new byte[f.Length];
                        await f.ReadAsync(contents, 0, (int)f.Length);
                        f.Close();
                    }
                    catch (Exception ex)
                    {
                        //Gobble and just return missing or invalid... 
                    }
                }
                    
            //}

            return (contents);
        }
        
        public string youtubeEmbedUrl(string url)
        {
            string youtubeEmbedUrl = null;
            var id = ExtractVideoIdFromUri(url);
            if (!String.IsNullOrWhiteSpace(id))
            {
                youtubeEmbedUrl = "https://www.youtube.com/embed/" + id;
            }
            return (youtubeEmbedUrl);
        }
        

        public string ExtractVideoIdFromUri(string url)
        {
            string videoId = "";
            if (!String.IsNullOrWhiteSpace(url))
            {
                videoId = Regex.Match(url, "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+").Groups[1].Value;
            }
            return videoId;

        }
    }
}
