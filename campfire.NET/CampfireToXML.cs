// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Linq;
using System.IO;

namespace CampfireAPI
{
    public interface ICampfireToXml
    {
        XDocument Doit(string httpGetRequestURI);
        XDocument Doit(string httpRequestURI, string requestMethod);
        XDocument Doit(string httpRequestURI, string requestMethod, out HttpStatusCode httpStatus);
    }

    /// <summary>
    /// A simple wrapper around an HTTP request to campfire API and packing the response body into an Xml document
    /// </summary>
    public class CampfireToXml : ICampfireToXml
    {
        #region private
        private string CampfireName { get; set; }
        private string AuthToken { get; set; }

        private XDocument ToDocument(HttpWebResponse response)
        {
            XDocument xdoc;
            if (response != null)
            {
                xdoc = XDocument.Load(new StreamReader(response.GetResponseStream()));
            }
            else
            {
                xdoc = new XDocument();
            }

            return xdoc;
        }
        #endregion

        public CampfireToXml(string campfireName, string authToken)
        {
            this.CampfireName = campfireName;
            this.AuthToken = authToken;
        }

        public XDocument Doit(string cmd)
        {
            return Doit(cmd, "GET");
        }

        public XDocument Doit(string cmd, string requestMethod = "GET")
        {
            HttpStatusCode status;
            return Doit(cmd, requestMethod, out status);
        }

        public XDocument Doit(string cmd, string requestMethod, out HttpStatusCode statusCode)
        {
            try
            {
                string uri = string.Format("https://{0}.campfirenow.com{1}", CampfireName, cmd);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.Credentials = new NetworkCredential(this.AuthToken, "x");
                req.Method = requestMethod;
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                statusCode = response.StatusCode;
                return ToDocument(response);
            }
            catch (UriFormatException uex)
            {
                // campfire name not yet set.
                throw new WebException("CampfireName missing", uex, WebExceptionStatus.NameResolutionFailure, null);
            }
            catch (WebException ex)
            {
                HttpWebResponse webResponse = (ex.Response as HttpWebResponse);
                if ((webResponse == null) || (webResponse.StatusCode != HttpStatusCode.NotFound))
                {
                    throw;
                }
                // otherwise the error means that the data was not found (e.g. userID doesn't exist, etc). 
                // A semantic error meaning that code should return null.
                statusCode = webResponse.StatusCode;
            }
            return null;
        }
    }
}
