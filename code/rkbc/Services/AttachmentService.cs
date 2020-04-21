using Microsoft.Extensions.Options;
using rkbc.config.models;
using rkbc.core.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.service
{
    public class AttachmentService
    {
        protected FileHelper fileHelper;
        protected IOptions<RkbcConfig> rkbcSetting;
        
        public AttachmentService(FileHelper _fileHelper, IOptions<RkbcConfig> _rkbcConfig)
        {
            fileHelper = _fileHelper;
            rkbcSetting = _rkbcConfig;
        }


    }
}
