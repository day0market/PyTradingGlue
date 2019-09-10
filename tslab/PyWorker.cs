using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace TSLab.Example
{
    class PyWorker
    {
        private string url;

        public PyWorker(string url)
        {
            this.url = url;
        }

        public double GetPyResult(double[] data)
        {
            var json = JsonConvert.SerializeObject(data);
            var rr = this.SendData(json);
            return this.ConvertResponse(rr);
        }

        private string SendData(string json)
        {
            //Send request and get string response. If status is not 200 then return empty string
            var request = HttpWebRequest.Create(this.url);
            var byteData = Encoding.ASCII.GetBytes(json);
            request.ContentType = "application/json";
            request.Method = "POST";

            using (var stream = request.GetRequestStream())
            {
                stream.Write(byteData, 0, byteData.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "";
            }
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }


        private double ConvertResponse(string response)
        {
            // Parse string to double. If it's not possible return NaN

            double result = double.NaN;

            if (response == null)
            {
                return result;
            }

            bool ok = double.TryParse(response, out result);

            if (!ok)
            {
                return double.NaN;
            }

            return result;

        }
    }
}
