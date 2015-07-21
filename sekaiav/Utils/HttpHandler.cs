using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace sekaiav.Utils
{
    public enum HttpMethd
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class HttpHandler
    {
        public string WebRequest(HttpMethd method, string url, string postData)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            //webRequest.UserAgent  = "Identify your application please.";
            //webRequest.Timeout = 20000;

            if (method == HttpMethd.POST || method == HttpMethd.PUT)
            {
                if (method == HttpMethd.PUT)
                {
                    webRequest.ContentType = "text/xml";
                    webRequest.Method = "PUT";
                }
                else
                {
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    //webRequest.ContentType = "multipart/form-data";
                }

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);
            webRequest = null;
            return responseData;
        }

        private string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }
    }
}
