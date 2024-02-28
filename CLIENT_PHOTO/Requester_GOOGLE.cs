using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Diagnostics;

namespace CLIENT_PHOTO
{
    public class Requester_GOOGLE
    {
        string client_id;
        string client_secret;
        string redirect_uri;
        public string user;

        public Requester_GOOGLE(string client_id, string client_secret, string redirect_uri, string user)
        {
            this.client_id = client_id;
            this.client_secret = client_secret;
            this.redirect_uri = redirect_uri;
            this.user = user;
        }

        public string getO2AuthToken(string user)
        {
            string request = "https://accounts.google.com/o/oauth2/v2/auth?" +
                 "scope=https://www.googleapis.com/auth/photoslibrary.readonly&" +
                       "access_type=offline&" +
                       "include_granted_scopes=true&" +
                       "response_type=code&" +
                       "state=state_parameter_passthrough_value&" +
                       "redirect_uri=http://localhost:8080&" +
                       "client_id=" + this.client_id;

            //LANCE LE NAVIGATEUR POUR AUTORISATION
            Console.WriteLine("Entrez le code dans l'URL pour " + user);
            System.Diagnostics.Process.Start(request);
            return Console.ReadLine();
        }

        public void GetUrl(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AllowAutoRedirect = false;  // IMPORTANT

            webRequest.Timeout = 10000;           // timeout 10s
            webRequest.Method = "HEAD";
            // Get the response ...
            HttpWebResponse webResponse;
            using (webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                // Now look to see if it's a redirect
                if ((int)webResponse.StatusCode >= 300 &&
                    (int)webResponse.StatusCode <= 399)
                {
                    string uriString = webResponse.Headers["Location"];
                    Console.WriteLine("Redirect to " + uriString ?? "NULL");
                    webResponse.Close(); // don't forget to close it - or bad things happen
                }
            }
        }

        public async Task<string> GetToken(string code)
        {
            var options = new RestClientOptions("https://oauth2.googleapis.com")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/token", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", this.client_id);
            request.AddParameter("client_secret", this.client_secret);
            request.AddParameter("code", code);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("redirect_uri", redirect_uri);
            RestResponse response = await client.ExecuteAsync(request);

            Console.WriteLine("RESPONSE TOKEN:\n");
            Console.WriteLine(response.Content);

            dynamic responseObj = JsonConvert.DeserializeObject<dynamic>(response.Content);
            string access_token = responseObj.access_token;
            return access_token;
        }

        public async Task<List<Images_GOOGLE>> GetAllImages(string month, string year, string user)
        {
            //ATTENDRE LA GENERATION DU TOKEN FINAL
            var accessToken = GetToken(getO2AuthToken(user)).Result;
            Console.WriteLine("ACCESS TOKEN : \n" + accessToken);
            return await getImagesAsync(generateBody(month, year), accessToken);
            //Console.WriteLine("END OF DOWNLOAD FILE !\n");
        }
        private string generateBody(string month, string year)
        {
            string body = "";
            try
            {
                String line;
                string directoyToGet = pathManager.getParentPath(Directory.GetCurrentDirectory(),2);
                string FILTER_SAMPLE = Path.Combine(directoyToGet, "filter_sample.txt");
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(FILTER_SAMPLE);
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the line to console window
                    line = line.Replace("@MONTH", month);
                    line = line.Replace("@YEAR", year);
                    body += line;
                    //Read the next line
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
            }
            catch (Exception e) { Console.WriteLine("Exception: " + e.Message); }
            return body;
        }

        private async Task<List<Images_GOOGLE>> getImagesAsync(string body, string accessToken)
        {
            //CONFIG DU CLIENT REST
            var options = new RestClientOptions("https://photoslibrary.googleapis.com")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);

            //CONSTRUCTION DE LA REQUETE
            var request = new RestRequest("/v1/mediaItems:search?access_token=" + accessToken, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(body, DataFormat.Json);
            //EXECUTION DE LA REQUETE
            RestResponse response = await client.ExecuteAsync(request);

            //A CE MOMENT CI - ON A LA PREMIERE PAGE DE RESULTATS
            bool hasNextPage = true;
            List<Images_GOOGLE> images = new List<Images_GOOGLE>();
            while (hasNextPage)
            {
                //ON CONVERTIT LA LISTE DE PHOTOS EN OBJET C#
                dynamic responseObj = JsonConvert.DeserializeObject<dynamic>(response.Content);
                if (responseObj.mediaItems == null)
                {
                    Console.WriteLine("Aucune photo n'a été trouvée");
                }
                else
                {
                    //ON CREE LES PHOTOS EN OBJET
                    foreach (dynamic mediaItem in responseObj.mediaItems)
                    {
                        Console.WriteLine("ID = " + mediaItem.id);
                        Console.WriteLine("BASE URL = " + mediaItem.baseUrl);
                        Console.WriteLine("filename = " + mediaItem.filename);
                        images.Add(new Images_GOOGLE(Convert.ToString(mediaItem.id), Convert.ToString(mediaItem.baseUrl), Convert.ToString(mediaItem.filename), Convert.ToString(mediaItem.mimeType)));
                    }
                    Console.WriteLine("Nombre de photos trouvées : " + responseObj.mediaItems.Count);
                }

                // =dv POUR TELECHARGER UNE VIDEO

                if (responseObj.nextPageToken != null)
                {
                    hasNextPage = true;
                    request = new RestRequest("/v1/mediaItems:search?access_token=" + accessToken + "&pageToken=" + responseObj.nextPageToken, Method.Post);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddStringBody(body, DataFormat.Json);
                    response = await client.ExecuteAsync(request);
                }
                else { hasNextPage = false; }
            }
            return images;
        }

        public async Task<bool> downloadFilesAsync(List<Images_GOOGLE> images, string username,string folderPathForMedias)
        {
            folderPathForMedias = Path.Combine(folderPathForMedias, username);
            if (!Directory.Exists(folderPathForMedias))
            {
                Directory.CreateDirectory(folderPathForMedias);
            }

            // Utilisation de HttpClient pour le téléchargement des fichiers en parallèle
            using (HttpClient client = new HttpClient())
            {
                foreach (Images_GOOGLE image in images)
                {
                    Console.WriteLine("Téléchargement de l'image : " + image.filename);
                    var watch = Stopwatch.StartNew();
                    string url = null; // Remplacez ceci par l'URL de votre média

                    // Détermine l'URL en fonction du type de média (vidéo ou image)
                    if (image.mimeType.Contains("video"))
                    {
                        url = image.baseUrl + "=dv";
                    }
                    else
                    {
                        url = image.baseUrl + "=d";
                    }

                    string destinationPath = Path.Combine(folderPathForMedias, image.filename);

                    try
                    {
                        // Télécharge le fichier
                        byte[] mediaData = await client.GetByteArrayAsync(url);

                        // Écrit le contenu téléchargé dans le fichier
                        File.WriteAllBytes(destinationPath, mediaData);

                        Console.WriteLine("Fichier enregistré avec succès : " + destinationPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Une erreur s'est produite lors du téléchargement de l'image {image.filename} : {ex.Message}");
                    }
                    watch.Stop();
                    Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
                }
            }

            Console.WriteLine("Tous les téléchargements sont terminés !");
            return true;
        }
    }
}
