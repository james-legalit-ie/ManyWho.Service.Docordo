using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

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
    public class WorkflowRuleMessageHandler : DelegatingHandler
    {
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            WorkflowRuleNotification receivedNotification = new WorkflowRuleNotification();

            //ExtractData would populate notification class' variables, which can be used to get desired data.
            receivedNotification.ExtractData(request.Content.ReadAsStringAsync().Result);

            //following code is commented but is left to show how data can be extracted from notification class' object.

            //using (StreamWriter outfile = new StreamWriter(@"D:\file\ExtractedData.txt"))
            //{
            //    outfile.WriteLine("Object Name: " + receivedNotification.objectName);
            //    outfile.WriteLine("Session ID: " + receivedNotification.sessionID);
            //    foreach (object o in receivedNotification.objectIDs) 
            //    {
            //        outfile.WriteLine("ID: " + o.ToString());
            //    }
            //    foreach (object o in receivedNotification.notificationIDs) {
            //        outfile.WriteLine("Notification ID: " + o.ToString());
            //    }
            //}

            //Send a response back to SFDC
            return Task.FromResult(receivedNotification.PrepareResponse(request));

            //Note: since we are not calling base class' SendAsync function, the request will return from here, and will not reach our POST function.
        }
    }
}