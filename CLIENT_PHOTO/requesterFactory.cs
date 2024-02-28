using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace CLIENT_PHOTO
{
    public static class requesterFactory
    {
        private static string REDIRECT_URI = "http://localhost:8080";

        public static List<Requester_GOOGLE> buildRequester(string pathToGoogleLogs)
        {
            Console.WriteLine("Construction des requesters...");
            try
            {
                // Vérifiez si le fichier JSON existe
                if (!File.Exists(pathToGoogleLogs)){
                    Console.WriteLine("Le fichier JSON n'existe pas.");
                }

                // Lisez le contenu du fichier JSON
                string jsonContent = File.ReadAllText(pathToGoogleLogs);

                // Désérialisez le JSON en objet C#
                dynamic[] jsonObject = JsonConvert.DeserializeObject<dynamic[]>(jsonContent);
                List<dynamic> objects = new List<dynamic>(jsonObject);
                List<Requester_GOOGLE> api_ids = new List<Requester_GOOGLE>();
                foreach (dynamic obj in objects) { api_ids.Add(new Requester_GOOGLE((obj.client_id).ToString(), (obj.client_secret).ToString(), REDIRECT_URI, (obj.user).ToString())); }
                return api_ids;
            }
            catch (Exception ex){
                Console.WriteLine($"Erreur lors de la lecture du fichier JSON : {ex.Message}");
                return null;
            }
        }

        public static Requester_GOOGLE getRequesterFromName(string username,List<Requester_GOOGLE> list,string pathToGoogleLogs)
        {
            Requester_GOOGLE toSend = null;
            if (list.Count == 0)
            {
                Console.WriteLine("Aucun utilisateur n'a été trouvé..;\n tentative de construction...");
                buildRequester(pathToGoogleLogs);
                if (list.Count == 0)
                {
                    throw new Exception("Aucun fichier n'est trouvé");
                }
            }
            foreach (Requester_GOOGLE requester in list)
            {
                if (requester.user == username) { toSend = requester; break; }
            }
            return toSend;
        }

    }
}
