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
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DocordoAPI.Model.Domain;
    using ManyWho.Flow.SDK;
    using ManyWho.Flow.SDK.Describe;
    using ManyWho.Flow.SDK.Draw.Elements.Type;
    using ManyWho.Flow.SDK.Run.Elements.Config;
    using ManyWho.Flow.SDK.Run.Elements.Type;
    using ManyWho.Flow.SDK.Security;
    using ManyWho.Flow.SDK.Social;
    using ManyWho.Flow.SDK.Utils;
    using Newtonsoft.Json;

    public class DocordoServiceSingleton
    {
        public const String SERVICE_ACTION_LOGIN = "login";

        public const String SERVICE_VALUE_DOCORDO_DOMAIN = "DocordoDomain";
        public const String SERVICE_VALUE_DOCORDO_USERNAME = "DocordoUsername";
        public const String SERVICE_VALUE_DOCORDO_PASSWORD = "DocordoPassword";
        

        public const String SERVICE_VALUE_DOCORDO_EBIKKO_SESSION_ID = "DocordoEbikkoSessionId";
        public const String SERVICE_VALUE_DOCORDO_COOKIE_JSESSIONID = "DocordoCookieJSESSIONID";
       
        

        private static DocordoServiceSingleton docordoServiceSingleton;

        public const string SERVICE_INPUT_RECORD_NUMBER = "RecordNumber";
        public const string SERVICE_INPUT_MATTER_DESCRIPTION = "MatterDescription";

        public const String SERVICE_OUTPUT_ID = "Id";

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
            System.Diagnostics.Trace.TraceInformation("@start - public DescribeServiceResponseAPI Describe(DescribeServiceRequestAPI describeServiceRequest)");

            if (describeServiceRequest == null)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "DescribeServiceRequest object cannot be null.");
            }
            
            DescribeServiceInstallResponseAPI describeServiceInstallResponse = null;
            DescribeServiceActionResponseAPI describeServiceAction = null;
            DescribeServiceResponseAPI describeServiceResponse = null;
            
            System.Diagnostics.Trace.TraceInformation("// Start building the describe service response so the caller knows what they need to provide to use this service");
            describeServiceResponse = new DescribeServiceResponseAPI();
            describeServiceResponse.culture = new CultureAPI();
            describeServiceResponse.culture.country = "US";
            describeServiceResponse.culture.language = "EN";
            describeServiceResponse.culture.variant = null;

            describeServiceResponse.providesLogic = true;
            describeServiceResponse.providesIdentity = true;

            describeServiceResponse.providesDatabase = false;            
            describeServiceResponse.providesViews = false;
            describeServiceResponse.providesSocial = false;
            
            System.Diagnostics.Trace.TraceInformation("// If the user has provided these values as part of a re-submission, we can then go about configuring the rest of the service");
            describeServiceResponse.actions = new List<DescribeServiceActionResponseAPI>();

            // We have one message available under this service for creating tasks
            describeServiceAction = new DescribeServiceActionResponseAPI();
            describeServiceAction.uriPart = SERVICE_ACTION_LOGIN;
            describeServiceAction.developerName = "Login";
            describeServiceAction.developerSummary = "This action logs a user into docordo";
            describeServiceAction.isViewMessageAction = false;
            describeServiceAction.pageResponse = null;

            // Create the inputs for the task creation
            describeServiceAction.serviceInputs = new List<DescribeValueAPI>();
            describeServiceAction.serviceInputs.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_STRING, "DocordoDomain", null, true));
            describeServiceAction.serviceInputs.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_STRING, "DocordoUsername", null, true));
            describeServiceAction.serviceInputs.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_PASSWORD, "DocordoPassword", null, true));            

            // Create the outputs for the task creation
            describeServiceAction.serviceOutputs = new List<DescribeValueAPI>();
            describeServiceAction.serviceOutputs.Add(DescribeUtils.CreateDescribeValue(ManyWhoConstants.CONTENT_TYPE_BOOLEAN, "NodeId", null, false));            

            // Add the task action to the response
            describeServiceResponse.actions.Add(describeServiceAction);

            describeServiceInstallResponse = new DescribeServiceInstallResponseAPI();
            describeServiceInstallResponse.typeElements = new List<TypeElementRequestAPI>();
            
            // Assign the installation object to our main describe response
            describeServiceResponse.install = describeServiceInstallResponse;

            return describeServiceResponse;
        }

        /// <summary>
        /// This method returns the list of tables available for the org being queried.
        /// </summary>
        public List<TypeElementBindingAPI> DescribeTables(ObjectDataRequestAPI objectDataRequestAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public List<TypeElementBindingAPI> DescribeTables(ObjectDataRequestAPI objectDataRequestAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(objectDataRequestAPI));
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method returns the list of fields for the specified object and org being queried.
        /// </summary>
        public List<TypeElementPropertyBindingAPI> DescribeFields(ObjectDataRequestAPI objectDataRequestAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public List<TypeElementPropertyBindingAPI> DescribeFields(ObjectDataRequestAPI objectDataRequestAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(objectDataRequestAPI));            
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to invoke particular messages on the service.
        /// </summary>
        public async Task<ServiceResponseAPI> Invoke(IAuthenticatedWho authenticatedWho, String action, ServiceRequestAPI serviceRequest)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public async Task<ServiceResponseAPI> Invoke(IAuthenticatedWho authenticatedWho, String action, ServiceRequestAPI serviceRequest)");
            Trace.TraceInformation(JsonConvert.SerializeObject(authenticatedWho));
            Trace.TraceInformation(JsonConvert.SerializeObject(action));
            Trace.TraceInformation(JsonConvert.SerializeObject(serviceRequest));

            ServiceResponseAPI serviceResponse = null;

            if (string.IsNullOrEmpty(action))
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "Action cannot be null or blank.");
            }

            if (serviceRequest == null)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "ServiceRequest cannot be null.");
            }

            if (action.Equals(SERVICE_ACTION_LOGIN, StringComparison.InvariantCultureIgnoreCase) == true)
            {
                serviceResponse = await SalesforceInvokeSingleton.GetInstance().InvokeCreateMatter(authenticatedWho, serviceRequest);
            }
            else
            {
                // We don't have an action by that name
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "Action cannot be found for name: " + action);
            }

            return serviceResponse;            
        }

        /// <summary>
        /// This method is used to save data back to docordo.com.
        /// </summary>
        public ObjectDataResponseAPI Save(IAuthenticatedWho authenticatedWho, ObjectDataRequestAPI objectDataRequestAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public ObjectDataResponseAPI Save(IAuthenticatedWho authenticatedWho, ObjectDataRequestAPI objectDataRequestAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(authenticatedWho));
            Trace.TraceInformation(JsonConvert.SerializeObject(objectDataRequestAPI));
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to load data from salesforce.com.
        /// </summary>
        public ObjectDataResponseAPI Load(ObjectDataRequestAPI objectDataRequestAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public ObjectDataResponseAPI Load(ObjectDataRequestAPI objectDataRequestAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(objectDataRequestAPI));            
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to check to see if the user is in the provided authorization context.
        /// </summary>
        public ObjectDataResponseAPI GetUserInAuthorizationContext(IAuthenticatedWho authenticatedWho, ObjectDataRequestAPI objectDataRequestAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public ObjectDataResponseAPI GetUserInAuthorizationContext(IAuthenticatedWho authenticatedWho, ObjectDataRequestAPI objectDataRequestAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(authenticatedWho));
            Trace.TraceInformation(JsonConvert.SerializeObject(objectDataRequestAPI));

            System.Diagnostics.Trace.TraceInformation("A");
            ObjectAPI userObject = DescribeUtils.CreateUserObject(authenticatedWho);

            System.Diagnostics.Trace.TraceInformation("B");
            userObject.properties.Add(DescribeUtils.CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_STATUS,
                authenticatedWho.UserId == ManyWhoConstants.AUTHENTICATED_USER_PUBLIC_USER_ID ?
                    ManyWhoConstants.AUTHORIZATION_STATUS_NOT_AUTHORIZED : ManyWhoConstants.AUTHORIZATION_STATUS_AUTHORIZED));

            System.Diagnostics.Trace.TraceInformation("C");
            ObjectDataResponseAPI objectDataResponseAPI = new ObjectDataResponseAPI();
            objectDataResponseAPI.objectData = new List<ObjectAPI>();
            objectDataResponseAPI.objectData.Add(userObject);

            System.Diagnostics.Trace.TraceInformation("D");
            return objectDataResponseAPI;
        }

        /// <summary>
        /// This method is used to load the attributes that are available for user authentication queries.
        /// </summary>
        public ObjectDataResponseAPI LoadUserAttributes(ObjectDataRequestAPI objectDataRequestAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public ObjectDataResponseAPI LoadUserAttributes(ObjectDataRequestAPI objectDataRequestAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(objectDataRequestAPI));
            return new ObjectDataResponseAPI();
        }

        /// <summary>
        /// This method is used to load the attributes that are available for group authentication queries.
        /// </summary>
        public ObjectDataResponseAPI LoadGroupAttributes(ObjectDataRequestAPI objectDataRequestAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public ObjectDataResponseAPI LoadGroupAttributes(ObjectDataRequestAPI objectDataRequestAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(objectDataRequestAPI));
            return new ObjectDataResponseAPI();
        }

        /// <summary>
        /// This method is used to load the list of users that are available to select from.
        /// </summary>
        public ObjectDataResponseAPI LoadUsers(ObjectDataRequestAPI objectDataRequestAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public ObjectDataResponseAPI LoadUsers(ObjectDataRequestAPI objectDataRequestAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(objectDataRequestAPI));
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to load the list of groups that are available to select from.
        /// </summary>
        public ObjectDataResponseAPI LoadGroups(ObjectDataRequestAPI objectDataRequestAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public ObjectDataResponseAPI LoadGroups(ObjectDataRequestAPI objectDataRequestAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(objectDataRequestAPI));
            throw new NotImplementedException();
        }

        public ObjectDataResponseAPI Delete(ObjectDataRequestAPI objectDataRequestAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public ObjectDataResponseAPI Delete(ObjectDataRequestAPI objectDataRequestAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(objectDataRequestAPI));
            // Ixnay on the implementay
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to log the user into docordo based on the provided credentials.
        /// </summary>
        public AuthenticatedWhoResultAPI Login(AuthenticationCredentialsAPI authenticationCredentialsAPI)
        {
            System.Diagnostics.Trace.TraceInformation("@start - public AuthenticatedWhoResultAPI Login(AuthenticationCredentialsAPI authenticationCredentialsAPI)");
            Trace.TraceInformation(JsonConvert.SerializeObject(authenticationCredentialsAPI));
                        
            if (authenticationCredentialsAPI == null)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "AuthenticationCredentials object cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(authenticationCredentialsAPI.sessionToken) || string.IsNullOrWhiteSpace(authenticationCredentialsAPI.token))
            {
                if (string.IsNullOrWhiteSpace(authenticationCredentialsAPI.loginUrl))
                {
                    throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "AuthenticationCredentials.loginUrl cannot be null or blank.");
                }

                if (string.IsNullOrWhiteSpace(authenticationCredentialsAPI.username))
                {
                    throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "AuthenticationCredentials.Username cannot be null or blank.");
                }

                if (string.IsNullOrWhiteSpace(authenticationCredentialsAPI.password))
                {
                    throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "AuthenticationCredentials.Password cannot be null or blank.");
                }
            }            

            // Assuming everything went ok so far
            AuthenticatedWhoResultAPI authenticatedUser = new AuthenticatedWhoResultAPI();            

            try
            {
                DocordoAPI.DocordoService.GetInstance().Logout(authenticationCredentialsAPI.loginUrl, authenticationCredentialsAPI.sessionToken, authenticationCredentialsAPI.token);
                DocordoLoginResponse docordoLoginResponse = DocordoAPI.DocordoService.GetInstance().Login(authenticationCredentialsAPI.loginUrl, authenticationCredentialsAPI.username, authenticationCredentialsAPI.password);
                

                // LOAD THE PRINCIPAL DETAILS FROM docordo
                //userInfoResult = sforceService.getUserInfo();//

                // Looks like the credentials are OK, so we create an authenticated user object for this user                
                authenticatedUser.identityProvider = authenticationCredentialsAPI.loginUrl;
                authenticatedUser.userId = docordoLoginResponse.EbikkoSessionId;
                authenticatedUser.token = docordoLoginResponse.CookieJSESSIONID;
                authenticatedUser.username = docordoLoginResponse.PrincipalDetails[0].Username;

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


        #region SOCIAL

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
        public async Task<List<MentionedWhoAPI>> SearchUsersByName(IAuthenticatedWho authenticatedWho, String streamId, String name, SocialServiceRequestAPI socialServiceRequest)
        {
            List<MentionedWhoAPI> mentionedUsers = null;

            return mentionedUsers;
        }
        #endregion
    }
}