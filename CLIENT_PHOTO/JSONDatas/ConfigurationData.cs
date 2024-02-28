using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIENT_PHOTO.JSONDatas
{
    public class ConfigurationData
    {
        public string username { get; set; }
        public string monthToGet { get; set; }
        public string yearToGet { get; set; }
        public string ftpServerURL { get; set; }
        public string ftpServerPort { get; set; }
        public string ftpUsername { get; set; }
        public string ftpPasswd { get; set; }
        public string pathToSaveMedias { get; set; }
        public string distantPath { get; set; }
    }
}
