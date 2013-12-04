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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using System.Threading.Tasks;
    using ManyWho.Flow.SDK;
    using ManyWho.Flow.SDK.Utils;
    using ManyWho.Flow.SDK.Security;
    using ManyWho.Flow.SDK.Draw.Elements.Group;
    using ManyWho.Flow.SDK.Run;
    using ManyWho.Flow.SDK.Run.Elements.Config;
    using ManyWho.Flow.SDK.Social;
    using ManyWho.Flow.SDK.Run.Elements.Type;
    using ManyWho.Service.Docordo;
    using DocordoAPI.Model.Domain;

    public class SalesforceInvokeSingleton
    {
        private static SalesforceInvokeSingleton salesforceInvokeSingleton;

        private SalesforceInvokeSingleton()
        {

        }

        public static SalesforceInvokeSingleton GetInstance()
        {
            if (salesforceInvokeSingleton == null)
            {
                salesforceInvokeSingleton = new SalesforceInvokeSingleton();
            }

            return salesforceInvokeSingleton;
        }

        public async Task<ServiceResponseAPI> InvokeCreateMatter(IAuthenticatedWho authenticatedWho, ServiceRequestAPI serviceRequest)
        {
            ServiceResponseAPI serviceResponse = null;
            DateTime whenDate = DateTime.Now;
            List<ObjectAPI> taskObjects = null;
            ObjectAPI taskObject = null;

            string authenticationUrl = null;
            string username = null;
            string password = null;

            string recordNumber = null;
            string description = null;

            string matterId = null;

            // Grab the configuration values from the service request
            authenticationUrl = SettingUtils.GetConfigurationValue(DocordoServiceSingleton.SERVICE_VALUE_DOCORDO_DOMAIN, serviceRequest.configurationValues, true);
            username = SettingUtils.GetConfigurationValue(DocordoServiceSingleton.SERVICE_VALUE_DOCORDO_USERNAME, serviceRequest.configurationValues, true);
            password = SettingUtils.GetConfigurationValue(DocordoServiceSingleton.SERVICE_VALUE_DOCORDO_PASSWORD, serviceRequest.configurationValues, true);            

            if (serviceRequest.authorization != null)
            {
                // Get the message from the inputs
                recordNumber = ValueUtils.GetContentValue(DocordoServiceSingleton.SERVICE_INPUT_RECORD_NUMBER, serviceRequest.inputs, true);
                description = ValueUtils.GetContentValue(DocordoServiceSingleton.SERVICE_INPUT_MATTER_DESCRIPTION, serviceRequest.inputs, true);

                // Create a task object to save back to the system
                taskObject = new ObjectAPI();
                taskObject.developerName = "Matter";
                taskObject.properties = new List<PropertyAPI>();
                taskObject.properties.Add(new PropertyAPI() { developerName = "RecordNumber", contentValue = description });                
                taskObject.properties.Add(new PropertyAPI() { developerName = "Description", contentValue = description });                

                // Add the object to the list of objects to save
                taskObjects = new List<ObjectAPI>();
                taskObjects.Add(taskObject);
                
                // Save the task object to docordo
                DocordoLoginResponse docordoLoginResponse = DocordoAPI.DocordoService.GetInstance().Login(authenticationUrl, username, password);

                taskObjects = DocordoDataSingleton.GetInstance().CreateMatter(authenticationUrl, docordoLoginResponse.EbikkoSessionId, docordoLoginResponse.CookieJSESSIONID, recordNumber, description);

                // Check to see if anything came back as part of the save - it should unless there was a fault
                if (taskObjects != null &&
                    taskObjects.Count > 0)
                {
                    // Grab the first object from the returned task objects
                    taskObject = taskObjects[0];

                    // Grab the id from that object - this needs to be returned in our outputs
                    matterId = taskObject.externalId;
                }
                else
                {
                    // If we didn't get any objects back, we need to throw an error
                    String errorMessage = "Task could not be created for an unknown reason.";

                    ErrorUtils.SendAlert(authenticatedWho, ErrorUtils.ALERT_TYPE_FAULT, "james@legalit.ie", "DocordoPlugin", errorMessage);

                    throw ErrorUtils.GetWebException(HttpStatusCode.InternalServerError, errorMessage);
                }

            }
            else
            {
                // Alert the admin that no one is in the authorization context
                ErrorUtils.SendAlert(authenticatedWho, ErrorUtils.ALERT_TYPE_WARNING, "james@legalit.ie", "DocordoPlugin", "The service request does not have an authorization context, so there's no one to notify.");
            }

            // Construct the service response
            serviceResponse = new ServiceResponseAPI();
            serviceResponse.invokeType = ManyWhoConstants.INVOKE_TYPE_FORWARD;
            serviceResponse.token = serviceRequest.token;
            serviceResponse.outputs = new List<EngineValueAPI>();
            serviceResponse.outputs.Add(new EngineValueAPI() { contentType = ManyWhoConstants.CONTENT_TYPE_STRING, contentValue = matterId, developerName = DocordoServiceSingleton.SERVICE_OUTPUT_ID });

            return serviceResponse;
        }
        
        public String GetContentValueForDeveloperName(String developerName, List<EngineValueAPI> inputs)
        {
            String value = null;

            foreach (EngineValueAPI input in inputs)
            {
                if (input.developerName == developerName)
                {
                    value = input.contentValue;
                    break;
                }
            }

            return value;
        }
    }
}