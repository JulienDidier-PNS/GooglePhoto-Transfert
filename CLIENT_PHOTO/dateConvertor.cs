﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIENT_PHOTO
{
    public static class dateConvertor
    {
        public static String getMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "Janvier";
                case 2:
                    return "Fevrier";
                case 3:
                    return "Mars";
                case 4:
                    return "Avril";
                case 5:
                    return "Mai";
                case 6:
                    return "Juin";
                case 7:
                    return "Juillet";
                case 8:
                    return "Aout";
                case 9:
                    return "Septembre";
                case 10:
                    return "Octobre";
                case 11:
                    return "Novembre";
                case 12:
                    return "Decembre";
                default:
                    return "Erreur";
            }
        }
    }
}
