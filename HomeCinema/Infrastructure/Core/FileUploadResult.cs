using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Infrastructure.Core
{
    public class FileUploadResult
    {
        public string FileName { get; set; }
        public string LocalFilePath { get; set; }
        public long FileLength { get; set; }
    }
}