using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIENT_PHOTO
{
    public class Images_GOOGLE
    {
        public string id { get; set;}
        public string baseUrl { get;set; }
        public string filename { get; set;}
        public string mimeType { get; set;}

        public Images_GOOGLE(string id, string baseUrl, string filename,string mimeType)
        {
            this.id = id;
            this.baseUrl = baseUrl;
            this.filename = filename;
            this.mimeType = mimeType;
        }
        
       
    }
}
