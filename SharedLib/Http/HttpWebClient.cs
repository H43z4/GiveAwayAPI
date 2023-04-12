using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Http
{
    internal class HttpWebClient
    {
        readonly string DataServerUrl;
        readonly string AccessToken;

        public HttpWebClient(string serverURL, string accessToken)
        {
            this.DataServerUrl = serverURL;
            this.AccessToken = accessToken;
        }

        public async Task<T> GetCitizen<T>(string endPoint) where T : class
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);

                    var Uri = new Uri($"{this.DataServerUrl}/{endPoint}");

                    //GET Method  
                    var response = await client.GetAsync(Uri)
                        .ContinueWith((task) =>
                        {
                            task.Result.EnsureSuccessStatusCode();

                            var content = task.Result.Content.ReadAsStringAsync().Result;
                            return JsonConvert.DeserializeObject<T>(content);
                        });

                    return response;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> SaveThumbImressionData(Citizen citizen)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);

                    var stringContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("CITIZEN_ID", citizen.CITIZEN_ID.ToString()),
                        new KeyValuePair<string, string>("CITIZEN_DISTRICT", citizen.CITIZEN_DISTRICT.ToString()),
                        new KeyValuePair<string, string>("NEW_NIC", citizen.NEW_NIC),
                        new KeyValuePair<string, string>("LEFT_THUMB", citizen.LEFT_THUMB),
                        new KeyValuePair<string, string>("RIGHT_THUMB", citizen.RIGHT_THUMB),
                    });

                    var response = await client.PostAsync(new Uri($"{this.DataServerUrl}/Biometric/SaveFingerPrint"), stringContent)
                        .ContinueWith((task) =>
                        {
                            task.Result.EnsureSuccessStatusCode();

                            var content = task.Result.Content.ReadAsStringAsync().Result;
                            //return JsonConvert.DeserializeObject<bool>(content);

                            return content.Contains("true") ? true : false;
                        });

                    return response;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
