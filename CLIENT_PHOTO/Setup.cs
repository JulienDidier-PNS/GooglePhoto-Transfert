using CLIENT_PHOTO.JSONDatas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIENT_PHOTO
{
    public static class Setup
    {
        public static ConfigurationData ReadAndValidateJsonSetup(string jsonFilePath)
        {
            try
            {
                // Vérifie si le fichier JSON existe
                if (!File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("Le fichier JSON spécifié est introuvable.");
                }

                // Lecture du contenu du fichier JSON
                string jsonContent = File.ReadAllText(jsonFilePath);

                // Désérialisation du JSON en un objet ConfigurationData
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonContent);
                Console.WriteLine("Objet JSON : " + jsonObject);

                return new ConfigurationData
                {
                    username = jsonObject.username,
                    monthToGet = jsonObject.monthToGet,
                    yearToGet = jsonObject.yearToGet,
                    ftpServerURL = jsonObject.ftpServerURL,
                    ftpServerPort = jsonObject.ftpServerPort,
                    ftpUsername = jsonObject.ftpUsername,
                    ftpPasswd = jsonObject.ftpPasswd,
                    pathToSaveMedias = jsonObject.pathToSave,
                    distantPath = jsonObject.distantPath
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite lors de la lecture et de la validation du fichier JSON : {ex.Message}");
                return null;
            }
        }

    }
}
