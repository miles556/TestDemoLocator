using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;//system web extensions

using System.Net;
using System.Net.Http;

using System.IO;

namespace MODEL {
    public static class API_ACCESS {
        public static void QueryGoogleMapsAPI<FROM>(Uri serverURL, ref FROM responseJSON, string requestMethod = "GET") {
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            int statusCode = -1;
            //
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(serverURL.ToString());
            request.ContentType = "application/json";
            request.Method = requestMethod;
            request.UseDefaultCredentials = true;
            request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

            HttpWebResponse httpResponse;
            try {
                using (httpResponse = (HttpWebResponse)request.GetResponse()) {
                    statusCode = (int)httpResponse.StatusCode;
                    if (statusCode == 200) {
                        var jsonSerializer = new DataContractJsonSerializer(typeof(FROM));
                        responseJSON = (FROM)jsonSerializer.ReadObject(httpResponse.GetResponseStream());
                    }
                }
            }
            catch (WebException we) {
                string errorResponse = "n/a";
                using (WebResponse response = we.Response) {
                    HttpWebResponse httpResponse2 = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data)) {
                        errorResponse = reader.ReadToEnd();
                    }
                }
                throw we;
            };
        }
    }
}
