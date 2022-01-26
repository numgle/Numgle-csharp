using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Numgle
{
    public struct NumgleData
    {
        public string[] converted_cho;
        public string[] converted_jong;
        public string[] converted_jung;
        public string[,] converted_cj;
        public string[] converted_han;
        public string[] converted_english_upper;
        public string[] converted_english_lower;
        public string[] converted_number;
        public string[] converted_special;
    }
    public class DataLoader
    {
        public string dataURL { get; private set; } = "https://raw.githubusercontent.com/numgle/dataset/main/src/data.json";

        public NumgleData Load()
        {
            var data = new NumgleData();
            var jsonStr = RequestJson(dataURL);
            var jsonObject = JObject.Parse(jsonStr);

            data.converted_cho = jsonObject["cho"].ToObject<string[]>();
            data.converted_jung = jsonObject["jung"].ToObject<string[]>();
            data.converted_jong = jsonObject["jong"].ToObject<string[]>();
            data.converted_han = jsonObject["han"].ToObject<string[]>();
            data.converted_cj = jsonObject["cj"].ToObject<string[,]>();
            data.converted_english_upper = jsonObject["englishUpper"].ToObject<string[]>();
            data.converted_english_lower = jsonObject["englishLower"].ToObject<string[]>();
            data.converted_number = jsonObject["number"].ToObject<string[]>();
            data.converted_special = jsonObject["special"].ToObject<string[]>();

            return data;
        }

        private string RequestJson(string url)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader stream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            result = stream.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }
    }
}
