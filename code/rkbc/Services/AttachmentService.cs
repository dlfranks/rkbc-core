using Microsoft.AspNetCore.Http;
using rkbc.core.helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.Services
{
    public class AttachmentService
    {
        protected FileHelper fileHelper;
        public AttachmentService(FileHelper _fileHelper)
        {
            fileHelper = _fileHelper;
        }
        
    }
}
