using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CLIENT_PHOTO
{
    internal class FTPServer
    {
        string url;
        string username;
        string password;

        public FTPServer(string url, string port, string username, string password)
        {
            this.username = username;
            this.password = password;
            this.url = $"ftp://{url}:{port}";
        }
        public bool CreateDirectoryOnFtp(string folderPath)
        {
            Console.WriteLine($"{this.url}/{folderPath}");
            try
            {
                // Créez une demande FTP pour créer le dossier
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri($"{this.url}/{folderPath}"));
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(this.username, this.password);

                // Obtenez la réponse FTP
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Statut de la création du dossier : {response.StatusDescription}");
                }

                return true; // Indiquez que la création a réussi
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création du dossier sur le serveur FTP : {ex.Message}");
                return false; // Indiquez que la création a échoué
            }
        }
        public void UploadFileToFTP(string localFilePath, string distantFolderPath)
        {
            Console.WriteLine("localFilePath : "+localFilePath);
            Console.WriteLine("distantFolderPath : "+distantFolderPath);
            try
            {
                // Créez une demande FTP avec le nom du fichier à télécharger
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri($"{this.url}/{distantFolderPath}/{Path.GetFileName(localFilePath)}"));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(this.username, this.password);

                // Lisez le fichier à télécharger en octets
                byte[] fileContents = File.ReadAllBytes(localFilePath);

                // Transférez le fichier sur le serveur FTP
                request.ContentLength = fileContents.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                // Obtenez la réponse FTP
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Statut de téléchargement du fichier : {response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du téléchargement du fichier sur le serveur FTP : {ex.Message}");
            }
        }   

        public bool DirectoryExists(string directoryPath)
        {
            try
            {
                // Créez une demande FTP avec le nom du dossier
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri($"{this.url}/{directoryPath}"));
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(this.username, this.password);

                // Obtenez la réponse FTP
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { 
                    Console.WriteLine($"Statut de la vérification du dossier : {response.StatusCode}");
                    // Si le code de statut est DirectoryStatus, le dossier existe
                    return response.StatusCode == FtpStatusCode.DataAlreadyOpen;
                }
            }
            catch (WebException ex)
            {
                // Si la requête a échoué, le dossier n'existe pas
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
                // Si une autre erreur s'est produite, lancez-la
                throw;
            }
        }
    }
}
