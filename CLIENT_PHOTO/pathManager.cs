using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIENT_PHOTO
{
    public static class pathManager
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

        // C/USERS/USERNAME/DOCUMENTS/
        // C=3 USERS=2 USERNAME=1 DOCUMENTS=0 ....
        public static string getParentPath(string path,int parentNum)
        {
            string[] pathArray = path.Split('/');
            string parentPath = "";
            for (int i = 0; i < pathArray.Length - parentNum; i++)
            {
                parentPath += pathArray[i] + "/";
            }
            return parentPath;
        }
    }
}
