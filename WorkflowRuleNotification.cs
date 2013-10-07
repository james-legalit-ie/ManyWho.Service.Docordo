using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using System.Xml;

/*!

Copyright 2013 Manywho, Inc.

Licensed under the Manywho License, Version 1.0 (the "License"); you may not use this
file except in compliance with the License.

You may obtain a copy of the License at: http://manywho.com/sharedsource

Unless required by applicable law or agreed to in writing, software distributed under
the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.

*/

namespace ManyWho.Service.Docordo
{
    public class WorkflowRuleNotification
    {
        public ArrayList notificationIDs { get; set; } //salesforce can send upto 100 notifications in a single soap request
        public ArrayList objectIDs { get; set; } //salesforce can send upto 100 notifications, ergo 100 record ids can exist in a single soap request
        public string sessionID { get; set; }
        public string objectName { get; set; }

        public WorkflowRuleNotification()
        {
            notificationIDs = new ArrayList();
            objectIDs = new ArrayList();
            sessionID = String.Empty;
            objectName = String.Empty;
        }

        public HttpResponseMessage PrepareResponse(HttpRequestMessage request)
        {
            try
            {
                StringBuilder acknowledgement = new StringBuilder();

                acknowledgement.Append("<?xml version = \"1.0\" encoding = \"utf-8\"?>");
                acknowledgement.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
                acknowledgement.Append("<soapenv:Body>");
                acknowledgement.Append("<notifications xmlns=\"http://soap.sforce.com/2005/09/outbound\">");
                acknowledgement.Append("<Ack>true</Ack>");
                acknowledgement.Append("</notifications>");
                acknowledgement.Append("</soapenv:Body>");
                acknowledgement.Append("</soapenv:Envelope>");

                HttpResponseMessage response = request.CreateResponse();
                response.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(acknowledgement.ToString())));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                response.StatusCode = HttpStatusCode.OK;

                return response;
            }
            catch (Exception)
            {
                //TODO: log exception
            }

            return request.CreateResponse();

        }

        public void ExtractData(string soapBody)
        {
            if (soapBody != String.Empty)
            {
                try
                {
                    XmlTextReader xtr = new XmlTextReader(new System.IO.StringReader(soapBody));
                    XmlDocument doc = new XmlDocument();
                    XmlNode node = doc.ReadNode(xtr);

                    while (xtr.Read())
                    {
                        if (xtr.IsStartElement())
                        {
                            // Get element name
                            switch (xtr.Name)
                            {
                                //extract session id
                                case "SessionId":
                                    if (xtr.Read())
                                    {
                                        sessionID = xtr.Value.Trim();
                                    }
                                    break;
                                //extract object's name
                                case "sObject":
                                    if (xtr["xsi:type"] != null)
                                    {
                                        string sObjectName = xtr["xsi:type"];
                                        if (sObjectName != null)
                                        {
                                            objectName = sObjectName.Split(new char[] { ':' })[1];
                                        }
                                    }
                                    break;
                                //extract notification id [note: salesforce can send a notification multiple times. it is, therefore, a good idea to keep track of this id.]
                                case "Id":
                                    if (xtr.Read())
                                    {
                                        notificationIDs.Add(xtr.Value.Trim());
                                    }
                                    break;
                                //extract record id
                                case "sf:Id":
                                    if (xtr.Read())
                                    {
                                        objectIDs.Add(xtr.Value.Trim());
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //TODO: log exception
                }
            }
        }
    }
}