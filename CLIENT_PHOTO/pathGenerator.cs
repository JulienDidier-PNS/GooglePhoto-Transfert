using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIENT_PHOTO
{
    public static class pathGenerator
    {
        public static string generatePath(string path,string username=null, string year=null, string month=null)
        {
            if(username != null)
                path = path.Replace("{username}", username);
            if(year != null)
                path = path.Replace("{year}", year);
            if(month != null)
                path = path.Replace("{month}", dateConvertor.getMonth(int.Parse(month)));
            return path;
        }
    }
}
