using CLIENT_PHOTO.JSONDatas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web.UI;

namespace CLIENT_PHOTO
{
    internal class main
    {
        static int Main(string[] args)
        {
            string JSONSetupPath = null;
            string JSONGGApiLogs = null;
            string JSONImagesPath = null;

            if (args.Length != 3) { Console.Error.WriteLine("Usage: CLIENT_PHOTO.exe <JSONSetupPath> <JSONGGApiLogs> <JSONImagesPath>");return -1; }
            else{
               JSONSetupPath = args[0];
               JSONGGApiLogs = args[1];
               JSONImagesPath = args[2];
            }

            //let's retrive elal necessary informations
            ConfigurationData configData = Setup.ReadAndValidateJsonSetup(JSONSetupPath);
            Console.WriteLine("Configuration Data: " + configData);
            if (configData == null){ Console.WriteLine("Impossible de lire et de valider le fichier JSON.");return -1; }

            //lets buil all requesters (1 per user)
            List<Requester_GOOGLE> requesters = requesterFactory.buildRequester(JSONGGApiLogs);

            //lets get the requester for the specified user
            Requester_GOOGLE requester = requesterFactory.getRequesterFromName(configData.username, requesters,JSONGGApiLogs);
            
            //lets get all images for the specified user and specified month and year
            List<Images_GOOGLE> images = requester.GetAllImages(configData.monthToGet, configData.yearToGet, configData.username).Result;

            //lets save the images informations in a json file (in case of the download fails during process)
            string json = JsonSerializer.Serialize(images);
            File.WriteAllText(JSONImagesPath, json);

            //lets retrieve json file with all medias informations
            string jsonAfter = File.ReadAllText(JSONImagesPath);
            List<Images_GOOGLE> imagesAfter = JsonSerializer.Deserialize<List<Images_GOOGLE>>(jsonAfter);

            //download all medias present in the json file
            //bool cont = requester.downloadFilesAsync(imagesAfter, configData.username, FolderPathForLocalMedias).Result;

            //creation of FTP folder
            Console.WriteLine("Creation of FTP Folder");
            string finalDistantPath = pathManager.generatePath(configData.distantPath,configData.username,configData.yearToGet,configData.monthToGet);
            Console.WriteLine("Path to create: " + finalDistantPath);

            //Initialisation of the FTP connection
            FTPServer ftp = new FTPServer(configData.ftpServerURL, configData.ftpServerPort, configData.ftpUsername, configData.ftpPasswd);

            if (!ftp.DirectoryExists(finalDistantPath)){ftp.CreateDirectoryOnFtp(finalDistantPath); }

            //upload les images téléchargées sur le NAS
            foreach (Images_GOOGLE image in imagesAfter)
            {
                string localFilePath = Path.Combine(pathManager.generatePath(configData.pathToSaveMedias, configData.username), image.filename);
                ftp.UploadFileToFTP(localFilePath, finalDistantPath);
            }
            
            return 0;
        }   
    }
}
