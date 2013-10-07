using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ManyWho.Flow.SDK;
using ManyWho.Flow.SDK.Utils;
using ManyWho.Flow.SDK.Social;
using ManyWho.Flow.SDK.Security;
using ManyWho.Flow.SDK.Describe;
using ManyWho.Flow.SDK.Draw.Elements.UI;
using ManyWho.Flow.SDK.Draw.Elements.Type;
using ManyWho.Flow.SDK.Draw.Elements.Config;
using ManyWho.Flow.SDK.Draw.Elements.Group;
using ManyWho.Flow.SDK.Run.Elements.UI;
using ManyWho.Flow.SDK.Run.Elements.Type;
using ManyWho.Flow.SDK.Run.Elements.Config;
using ManyWho.Service.Docordo.Utils;

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
    public class DocordoServiceSingleton
    {        
        public const String SERVICE_ACTION_CREATE_MATTER = "newmatter";

        public const String SERVICE_VALUE_DOCORDO_USERNAME = "DocordoUsername";
        public const String SERVICE_VALUE_DOCORDO_PASSWORD = "DocordoPassword";
        public const String SERVICE_VALUE_DOCORDO_DOMAIN = "DocordoDomain";

        public const String SERVICE_VALUE_DOCORDO_EBIKKO_SESSION_ID = "DocordoEbikkoSessionId";
        public const String SERVICE_VALUE_DOCORDO_COOKIE_JSESSIONID = "DocordoCookieJSESSIONID";       

        public const String SERVICE_INPUT_DESCRIPTION = "Description";

        public const String SERVICE_OUTPUT_ID = "Id";        

        private static DocordoServiceSingleton docordoServiceSingleton;

        private DocordoServiceSingleton()
        {

        }

        public static DocordoServiceSingleton GetInstance()
        {
            if (docordoServiceSingleton == null)
            {
                docordoServiceSingleton = new DocordoServiceSingleton();
            }

            return docordoServiceSingleton;
        }

        /// <summary>
        /// This method performs the actual describe for the service setup. The code here provides the configuration information needed to use the docordo.com plugin.
        /// </summary>
        public DescribeServiceResponseAPI Describe(DescribeServiceRequestAPI describeServiceRequest)
        {
            System.Diagnostics.Trace.Indent();
            System.Diagnostics.Trace.TraceInformation("public DescribeServiceResponseAPI Describe");

            if (describeServiceRequest == null)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "DescribeServiceRequest object cannot be null.");
            }

            DescribeServiceInstallResponseAPI describeServiceInstallResponse = null;
            DescribeServiceActionResponseAPI describeServiceAction = null;
            DescribeServiceResponseAPI describeServiceResponse = null;

            string docordoDomain = null;
            string docordoUsername = null;
            string docordoPassword = null;

            string docordoCookieJSESSIONID = null;
            string docordoEbikkoSessionId = null;

            System.Diagnostics.Trace.TraceInformation("// We do not require configuration values in the describe call as this is a refresh type operation");
            if (describeServiceRequest.configurationValues != null && describeServiceRequest.configurationValues.Count > 0)
            {
                System.Diagnostics.Trace.TraceInformation("// If the configuration values are provided, then all of them are required");                
                docordoDomain = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_DOMAIN, describeServiceRequest.configurationValues, true);
                docordoUsername = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_USERNAME, describeServiceRequest.configurationValues, true);
                docordoPassword = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_PASSWORD, describeServiceRequest.configurationValues, true);

                DocordoAPI.Model.Domain.DocordoLoginResponse docordoLoginResponse = DocordoAPI.DocordoService.GetInstance().Login(docordoDomain, docordoUsername, docordoPassword);
                docordoCookieJSESSIONID = docordoLoginResponse.CookieJSESSIONID;
                docordoEbikkoSessionId = docordoLoginResponse.EbikkoSessionId;
            }

            System.Diagnostics.Trace.TraceInformation("// Start building the describe service response so the caller knows what they need to provide to use this service");
            describeServiceResponse = new DescribeServiceResponseAPI();
            describeServiceResponse.culture = new CultureAPI();
            describeServiceResponse.culture.country = "US";
            describeServiceResponse.culture.language = "EN";
            describeServiceResponse.culture.variant = null;
            describeServiceResponse.exposesTables = true;
            describeServiceResponse.exposesLogic = true;
            describeServiceResponse.exposesViews = false;
            describeServiceResponse.providesIdentity = true; 
            describeServiceResponse.providesSocial = false;

            System.Diagnostics.Trace.TraceInformation("// Create the main configuration values");            
            describeServiceResponse.configurationValues = new List<DescribeValueAPI>();
            describeServiceResponse.configurationValues.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_STRING, SERVICE_VALUE_DOCORDO_DOMAIN, docordoDomain, true));
            describeServiceResponse.configurationValues.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_STRING, SERVICE_VALUE_DOCORDO_USERNAME, docordoUsername, true));
            describeServiceResponse.configurationValues.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_PASSWORD, SERVICE_VALUE_DOCORDO_PASSWORD, docordoPassword, true));

            describeServiceResponse.configurationValues.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_STRING, SERVICE_VALUE_DOCORDO_COOKIE_JSESSIONID, docordoCookieJSESSIONID, false));
            describeServiceResponse.configurationValues.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_STRING, SERVICE_VALUE_DOCORDO_EBIKKO_SESSION_ID, docordoEbikkoSessionId, false));

            System.Diagnostics.Trace.TraceInformation("// If the user has provided these values as part of a re-submission, we can then go about configuring the rest of the service");                       
            if (!string.IsNullOrWhiteSpace(docordoDomain) && !string.IsNullOrWhiteSpace(docordoUsername) && !string.IsNullOrWhiteSpace(docordoPassword))
            {
                describeServiceResponse.actions = new List<DescribeServiceActionResponseAPI>();

                System.Diagnostics.Trace.TraceInformation("// We have another message available under this service for creating simple tasks with no async");
                describeServiceAction = new DescribeServiceActionResponseAPI();
                describeServiceAction.uriPart = SERVICE_ACTION_CREATE_MATTER;
                describeServiceAction.developerName = "New Matter";
                describeServiceAction.developerSummary = "This action creates a matter in docordo.";
                describeServiceAction.isViewMessageAction = false;
                describeServiceAction.pageResponse = null;

                System.Diagnostics.Trace.TraceInformation("// Create the inputs for the task creation");
                describeServiceAction.serviceInputs = new List<DescribeValueAPI>();
                describeServiceAction.serviceInputs.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_STRING, SERVICE_INPUT_DESCRIPTION, null, true));

                System.Diagnostics.Trace.TraceInformation("// Create the outputs for the task creation");
                describeServiceAction.serviceOutputs = new List<DescribeValueAPI>();
                describeServiceAction.serviceOutputs.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_STRING, SERVICE_OUTPUT_ID, null, false));

                System.Diagnostics.Trace.TraceInformation("// Add the task action to the response");
                describeServiceResponse.actions.Add(describeServiceAction);

                System.Diagnostics.Trace.TraceInformation("// We now create the associated things for this service that we'd like to install into the manywho account");
                describeServiceInstallResponse = new DescribeServiceInstallResponseAPI();
                describeServiceInstallResponse.types = new List<TypeElementRequestAPI>();
                describeServiceInstallResponse.types = DocordoDataSingleton.GetInstance().GetTypeElements(docordoDomain, docordoUsername, docordoPassword, docordoEbikkoSessionId, docordoCookieJSESSIONID);

                System.Diagnostics.Trace.TraceInformation("// Assign the installation object to our main describe response");
                describeServiceResponse.install = describeServiceInstallResponse;
            }

            return describeServiceResponse;
        }

        /// <summary>
        /// This method returns the list of tables available for the org being queried.
        /// </summary>
        public List<TypeElementBindingAPI> DescribeTables(ObjectDataRequestAPI objectDataRequestAPI)
        {
            List<TypeElementBindingAPI> typeElementBindings = null;          

            return typeElementBindings;
        }

        /// <summary>
        /// This method returns the list of fields for the specified object and org being queried.
        /// </summary>
        public List<TypeElementFieldBindingAPI> DescribeFields(ObjectDataRequestAPI objectDataRequestAPI)
        {
            List<TypeElementFieldBindingAPI> typeElementFieldBindings = null;           

            return typeElementFieldBindings;
        }

        /// <summary>
        /// This method is used to invoke particular messages on the service.
        /// </summary>
        public async Task<ServiceResponseAPI> Invoke(IAuthenticatedWho authenticatedWho, String action, ServiceRequestAPI serviceRequest)
        {            
            ServiceResponseAPI serviceResponse = null;

            return serviceResponse;
        }

        /// <summary>
        /// This method is used to save data back to docordo.com.
        /// </summary>
        public ObjectDataResponseAPI Save(IAuthenticatedWho authenticatedWho, ObjectDataRequestAPI objectDataRequestAPI)
        {
            ObjectDataResponseAPI objectDataResponseAPI = null;

            string docordoDomain = null;
            string username = null;
            string password = null;
            string ebikkoSessionId = null;
            string ebikkoCookieJSESSIONID = null;
            
            if (objectDataRequestAPI == null || objectDataRequestAPI.configurationValues == null || objectDataRequestAPI.configurationValues.Count == 0)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "ObjectDataRequest.ConfigurationValues cannot be null or empty.");
            }

            // Get the configuration values out that are needed to save data to docordo.com
            docordoDomain = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_DOMAIN, objectDataRequestAPI.configurationValues, true);
            username = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_USERNAME, objectDataRequestAPI.configurationValues, true);
            password = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_PASSWORD, objectDataRequestAPI.configurationValues, true);
            ebikkoSessionId = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_EBIKKO_SESSION_ID, objectDataRequestAPI.configurationValues, true);
            ebikkoCookieJSESSIONID = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_COOKIE_JSESSIONID, objectDataRequestAPI.configurationValues, true);
            

            // We only perform the save if there's actually something to save!
            if (objectDataRequestAPI.objectData != null && objectDataRequestAPI.objectData.Count > 0)
            {
                // Save the data back
            }
            
            // Create the object data response object
            objectDataResponseAPI = new ObjectDataResponseAPI();
            // TODO: Should really get the culture that the authenticated user is running under
            objectDataResponseAPI.culture = objectDataRequestAPI.culture;
            // We can do this as we've applied the changes to the request objects
            objectDataResponseAPI.objectData = objectDataRequestAPI.objectData;

            return objectDataResponseAPI;
        }

        /// <summary>
        /// This method is used to load data from salesforce.com.
        /// </summary>
        public ObjectDataResponseAPI Load(ObjectDataRequestAPI objectDataRequestAPI)
        {
            ObjectDataResponseAPI objectDataResponseAPI = null;
            ObjectDataTypeAPI objectDataType = null;
            
            string docordoDomain = null;
            string username = null;
            string password = null;
            string ebikkoSessionId = null;
            string ebikkoCookieJSESSIONID = null;
            
            if (objectDataRequestAPI == null || objectDataRequestAPI.configurationValues == null || objectDataRequestAPI.configurationValues.Count == 0)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "ObjectDataRequest.ConfigurationValues cannot be null or empty.");
            }
            
            // Get the configuration values out that are needed to save data to docordo.com
            docordoDomain = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_DOMAIN, objectDataRequestAPI.configurationValues, true);
            username = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_USERNAME, objectDataRequestAPI.configurationValues, true);
            password = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_PASSWORD, objectDataRequestAPI.configurationValues, true);
            ebikkoSessionId = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_EBIKKO_SESSION_ID, objectDataRequestAPI.configurationValues, true);
            ebikkoCookieJSESSIONID = SettingUtils.GetConfigurationValue(SERVICE_VALUE_DOCORDO_COOKIE_JSESSIONID, objectDataRequestAPI.configurationValues, true);

            // Create a new response object to house our results
            objectDataResponseAPI = new ObjectDataResponseAPI();

            // TODO: Should really get the culture that the authenticated user is running under
            objectDataResponseAPI.culture = objectDataRequestAPI.culture;
            // We can do this as we've applied the changes to the request objects
            objectDataResponseAPI.objectData = objectDataRequestAPI.objectData;

            // We take the object data type information to give us the properties definition as defined by the calling flow
            objectDataType = objectDataRequestAPI.objectDataType;

            // Do the actual selection to populate one or many of these object types
            objectDataResponseAPI.objectData = new List<ObjectAPI>();

            return objectDataResponseAPI;
        }

        /// <summary>
        /// This method is used to check to see if the user is in the provided authorization context.
        /// </summary>
        public ObjectDataResponseAPI GetUserInAuthorizationContext(IAuthenticatedWho authenticatedWho, ObjectDataRequestAPI objectDataRequestAPI)
        {
            ObjectDataResponseAPI objectDataResponseAPI = null;
                        
            return objectDataResponseAPI;
        }

        /// <summary>
        /// This method is used to load the attributes that are available for user authentication queries.
        /// </summary>
        public ObjectDataResponseAPI LoadUserAttributes(ObjectDataRequestAPI objectDataRequestAPI)
        {
            ObjectDataResponseAPI objectDataResponseAPI = null;
            List<ObjectAPI> attributeObjects = null;

            // Populate the list of available attributes
            attributeObjects = new List<ObjectAPI>();
            //attributeObjects.Add(DescribeUtils.CreateAttributeObject("Colleagues", SERVICE_VALUE_COLLEAGUES));

            // Send the attributes back in the object data
            objectDataResponseAPI = new ObjectDataResponseAPI();
            objectDataResponseAPI.objectData = attributeObjects;

            return objectDataResponseAPI;
        }

        /// <summary>
        /// This method is used to load the attributes that are available for group authentication queries.
        /// </summary>
        public ObjectDataResponseAPI LoadGroupAttributes(ObjectDataRequestAPI objectDataRequestAPI)
        {            
            ObjectDataResponseAPI objectDataResponseAPI = null;
            List<ObjectAPI> attributeObjects = null;

            // Populate the list of available attributes
            attributeObjects = new List<ObjectAPI>();
            //attributeObjects.Add(DescribeUtils.CreateAttributeObject("Members", "MEMBERS"));
            //attributeObjects.Add(DescribeUtils.CreateAttributeObject("Owners", "OWNERS"));

            // Send the attributes back in the object data
            objectDataResponseAPI = new ObjectDataResponseAPI();
            objectDataResponseAPI.objectData = attributeObjects;

            return objectDataResponseAPI;
        }

        /// <summary>
        /// This method is used to load the list of users that are available to select from.
        /// </summary>
        public ObjectDataResponseAPI LoadUsers(ObjectDataRequestAPI objectDataRequestAPI)
        {            
            ObjectDataResponseAPI objectDataResponseAPI = null;            

            return objectDataResponseAPI;
        }

        /// <summary>
        /// This method is used to load the list of groups that are available to select from.
        /// </summary>
        public ObjectDataResponseAPI LoadGroups(ObjectDataRequestAPI objectDataRequestAPI)
        {            
            ObjectDataResponseAPI objectDataResponseAPI = null;            

            return objectDataResponseAPI;
        }

        public ObjectDataResponseAPI Delete(ObjectDataRequestAPI objectDataRequestAPI)
        {
            // Ixnay on the implementay
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to log the user into salesforce based on the provided credentials.
        /// </summary>
        public AuthenticatedWhoResultAPI Login(AuthenticationCredentialsAPI authenticationCredentialsAPI)
        {
            AuthenticatedWhoResultAPI authenticatedUser = null;
            
            if (authenticationCredentialsAPI == null)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "AuthenticationCredentials object cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(authenticationCredentialsAPI.sessionToken))
            {
                if (string.IsNullOrWhiteSpace(authenticationCredentialsAPI.username))
                {
                    throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "AuthenticationCredentials.Username cannot be null or blank.");
                }

                if (string.IsNullOrWhiteSpace(authenticationCredentialsAPI.password))
                {
                    throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "AuthenticationCredentials.Password cannot be null or blank.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(authenticationCredentialsAPI.sessionUrl))
                {
                    throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "AuthenticationCredentials.SessionUrl cannot be null or blank.");
                }
            }

            // Assuming everything went ok so far
            authenticatedUser = new AuthenticatedWhoResultAPI();
            authenticatedUser.identityProvider = "https://lit.docordo.com";

            try
            {
                if (authenticationCredentialsAPI.sessionToken != null &&
                    authenticationCredentialsAPI.sessionToken.Trim().Length > 0)
                {
                    // The user has already logged into salesforce so we simply check them against the session
                    //DocordoAPI.DocordoService.GetInstance().Login(authenticationCredentialsAPI., authenticationCredentialsAPI.sessionUrl);
                }
                else
                {
                    // Log the user into salesforce using the details provided
                    DocordoAPI.Model.Domain.DocordoLoginResponse docordoLoginResponse = DocordoAPI.DocordoService.GetInstance().Login(authenticationCredentialsAPI.instanceUrl, authenticationCredentialsAPI.username, authenticationCredentialsAPI.password);
                }

                // LOAD THE PRINCIPAL DETAILS FROM docordo
                //userInfoResult = sforceService.getUserInfo();//

                // Looks like the credentials are OK, so we create an authenticated user object for this user
                authenticatedUser.userId = string.Empty;
                authenticatedUser.username = string.Empty;
                authenticatedUser.token = string.Empty;
                authenticatedUser.tenantName = string.Empty;
                authenticatedUser.directoryId = string.Empty;
                authenticatedUser.directoryName = string.Empty;
                authenticatedUser.status = ManyWhoConstants.AUTHENTICATED_USER_STATUS_AUTHENTICATED;
                authenticatedUser.statusMessage = null;
            }
            catch (Exception exception)
            {
                // If there's an error, we simply deny the user access
                authenticatedUser.status = ManyWhoConstants.AUTHENTICATED_USER_STATUS_ACCESS_DENIED;
                authenticatedUser.statusMessage = exception.Message;
            }

            return authenticatedUser;
        }

        /// <summary>
        /// This method is used to create a new activity stream in salesforce based on the provided configuration.
        /// </summary>
        public String CreateStream(IAuthenticatedWho authenticatedWho, SocialServiceRequestAPI socialServiceRequestAPI)
        {            
            String streamId = null;
                       
            return streamId;
        }

        /// <summary>
        /// This method is used to get the user info for the logged in user in salesforce.
        /// </summary>
        public WhoAPI GetCurrentUserInfo(IAuthenticatedWho authenticatedWho, String streamId, SocialServiceRequestAPI serviceRequest)
        {
            WhoAPI whoAPI = null;
            return whoAPI;
        }

        /// <summary>
        /// This method is used to get the user info for the provided user id in docordo.
        /// </summary>
        public WhoAPI GetUserInfo(IAuthenticatedWho authenticatedWho, String streamId, String id, SocialServiceRequestAPI serviceRequest)
        {
            WhoAPI whoAPI = null;

            return whoAPI;
        }

        /// <summary>
        /// This method is used to get the list of stream followers in salesforce.
        /// </summary>
        public List<WhoAPI> GetStreamFollowers(IAuthenticatedWho authenticatedWho, String streamId, SocialServiceRequestAPI socialServiceRequest)
        {            
            List<WhoAPI> whos = null;

            return whos;
        }

        /// <summary>
        /// This method is used to get the list of stream messages in salesforce.
        /// </summary>
        public async Task<MessageListAPI> GetStreamMessages(IAuthenticatedWho authenticatedWho, String streamId, SocialServiceRequestAPI socialServiceRequest)
        {
            MessageListAPI messageList = null;            

            return messageList;
        }

        /// <summary>
        /// This method allows the user to share the flow app in salesforce with their friends.
        /// </summary>
        public async Task<MessageAPI> ShareMessage(IAuthenticatedWho authenticatedWho, String streamId, HttpContent httpContent)
        {
            MessageAPI message = null;

            return message;
        }

        /// <summary>
        /// This method allows the user to post a new message to the stream in chatter.
        /// </summary>
        public async Task<MessageAPI> PostNewMessage(IAuthenticatedWho authenticatedWho, String streamId, HttpContent httpContent)
        {
            MessageAPI message = null;

            return message;
        }

        /// <summary>
        /// This method allows the user to delete messages from the stream in chatter.
        /// </summary>
        public async Task<String> DeleteMessage(IAuthenticatedWho authenticatedWho, String streamId, String messageId, SocialServiceRequestAPI socialServiceRequest)
        {
            String response = null;

            return response;
        }

        /// <summary>
        /// This method allows the user to like messages in the stream in chatter.
        /// </summary>
        public async Task<String> LikeMessage(IAuthenticatedWho authenticatedWho, String streamId, String messageId, Boolean like, SocialServiceRequestAPI socialServiceRequest)
        {
            String response = null;

            return response;
        }

        /// <summary>
        /// This method allows the user to follow the stream in chatter.
        /// </summary>
        public async Task<String> FollowStream(IAuthenticatedWho authenticatedWho, String streamId, Boolean follow, SocialServiceRequestAPI socialServiceRequest)
        {
            String response = null;

            return response;
        }

        /// <summary>
        /// This method allows the user to search for users by name in chatter.
        /// </summary>
        public async Task<List<MentionedUserAPI>> SearchUsersByName(IAuthenticatedWho authenticatedWho, String streamId, String name, SocialServiceRequestAPI socialServiceRequest)
        {            
            List<MentionedUserAPI> mentionedUsers = null;         

            return mentionedUsers;
        }
    }
}