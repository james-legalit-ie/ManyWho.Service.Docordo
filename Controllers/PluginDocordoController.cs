using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using ManyWho.Flow.SDK.Controller;
using ManyWho.Flow.SDK.Utils;
using ManyWho.Flow.SDK.Draw.Flow;
using ManyWho.Flow.SDK.Draw.Content;
using ManyWho.Flow.SDK.Draw.Elements;
using ManyWho.Flow.SDK.Draw.Elements.UI;
using ManyWho.Flow.SDK.Draw.Elements.Map;
using ManyWho.Flow.SDK.Draw.Elements.Type;
using ManyWho.Flow.SDK.Draw.Elements.Shared;
using ManyWho.Flow.SDK.Run;
using ManyWho.Flow.SDK.Run.State;
using ManyWho.Flow.SDK.Run.Elements.UI;
using ManyWho.Flow.SDK.Run.Elements.Map;
using ManyWho.Flow.SDK.Run.Elements.Type;
using ManyWho.Flow.SDK.Run.Elements.Config;
using ManyWho.Flow.SDK.Describe;
using ManyWho.Flow.SDK.Security;
using ManyWho.Flow.SDK.Social;
using ManyWho.Service.Docordo;
using ManyWho.Service.Docordo.Utils;
using ManyWho.Service.Docordo.Models.Canvas;

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

namespace ManyWho.Flow.Web.Controllers
{
    public class PluginDocordoController : GenericController
    {
        public const String SETTING_SERVER_BASE_PATH = "Docordo.ServerBasePath";

        [ActionName("Canvas")]
        public HttpResponseMessage Canvas(String tenantId, String flowId, String playerUrl)
        {
            String redirectUrl = null;
            String signedRequest = null;
            CanvasRequest canvasRequest = null;
            HttpResponseMessage response = null;

            // Get the signed request from the form post
            signedRequest = System.Web.HttpContext.Current.Request.Form["signed_request"];

            // Grab the canvas request object from the post
            // The secret needs to be stored somewhere - actually, it doesn't - we don't need the secret at all
            canvasRequest = CanvasUtils.VerifyAndDecode(null, signedRequest, "6156156167154975556");

            if (string.IsNullOrWhiteSpace(flowId))
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "A flow identifier is required.  Please pass in a parameter for \"flow-id\".");
            }

            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "A tenant identifier is required.  Please pass in a parameter for \"tenant-id\".");
            }

            if (string.IsNullOrWhiteSpace(playerUrl))
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "A player is required.  Please pass in a parameter for \"player-url\".");
            }

            // Construct the redirect url so the player knows what to do
            redirectUrl = "";
            redirectUrl += SettingUtils.GetStringSetting(SETTING_SERVER_BASE_PATH) + "/" + tenantId + "/play/" + playerUrl;
            redirectUrl += "?session-token=" + canvasRequest.client.oauthToken;
            redirectUrl += "&session-url=" + HttpUtility.HtmlEncode(canvasRequest.client.instanceUrl + canvasRequest.context.links.partnerUrl);

            // Create the run url stuff using utils
            redirectUrl = RunUtils.CompleteRunUrl(redirectUrl, Guid.Parse(flowId));

            // Tell the caller to redirect back to the desired location
            response = Request.CreateResponse(HttpStatusCode.RedirectMethod, redirectUrl);
            response.Headers.Add("Location", redirectUrl);

            return response;
        }

        [HttpPost]
        [ActionName("Describe")]
        public DescribeServiceResponseAPI Describe(DescribeServiceRequestAPI describeServiceRequest)
        {
            try
            {
                return DocordoServiceSingleton.GetInstance().Describe(describeServiceRequest);
            }
            catch (Exception exception)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, exception);
            }
        }

        [HttpPost]
        [ActionName("DescribeTables")]
        public List<TypeElementBindingAPI> DescribeTables(ObjectDataRequestAPI objectDataRequestAPI)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "DescribeTables - Not implemented.");
        }

        [HttpPost]
        [ActionName("DescribeFields")]
        public List<TypeElementFieldBindingAPI> DescribeFields(ObjectDataRequestAPI objectDataRequestAPI)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "DescribeFields - Not implemented.");
        }

        [HttpPost]
        [ActionName("Invoke")]
        public Task<ServiceResponseAPI> Invoke(String actionName, ServiceRequestAPI serviceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "Invoke - Not implemented.");
        }

        [HttpPost]
        [ActionName("View")]
        public UIServiceResponseAPI View(String action, UIServiceRequestAPI uiServiceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "View - Not implemented.");
        }

        [HttpPut]
        [ActionName("Save")]
        public ObjectDataResponseAPI Save(ObjectDataRequestAPI objectDataRequestAPI)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "Save - Not implemented.");
        }

        [HttpPost]
        [ActionName("Load")]
        public ObjectDataResponseAPI Load(ObjectDataRequestAPI objectDataRequestAPI)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "Load - Not implemented.");
        }

        [HttpPost]
        [ActionName("Delete")]
        public ObjectDataResponseAPI Delete(ObjectDataRequestAPI objectDataRequestAPI)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "Delete - Not implemented.");
        }

        [HttpPost]
        [ActionName("GetUserInAuthorizationContext")]
        public ObjectDataResponseAPI GetUserInAuthorizationContext(ObjectDataRequestAPI objectDataRequestAPI)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "GetUserInAuthorizationContext - Not implemented.");
        }

        [HttpPost]
        [ActionName("LoadUsers")]
        public ObjectDataResponseAPI LoadUsers(ObjectDataRequestAPI objectDataRequestAPI)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "LoadUsers - Not implemented.");
        }

        [HttpPost]
        [ActionName("LoadUserAttributes")]
        public ObjectDataResponseAPI LoadUserAttributes(ObjectDataRequestAPI objectDataRequestAPI)
        {
            try
            {
                return DocordoServiceSingleton.GetInstance().LoadUserAttributes(objectDataRequestAPI);
            }
            catch (Exception exception)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, exception);
            }            
        }

        [HttpPost]
        [ActionName("LoadGroups")]
        public ObjectDataResponseAPI LoadGroups(ObjectDataRequestAPI objectDataRequestAPI)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "LoadGroups - Not implemented.");
        }

        [HttpPost]
        [ActionName("LoadGroupAttributes")]
        public ObjectDataResponseAPI LoadGroupAttributes(ObjectDataRequestAPI objectDataRequestAPI)
        {
            try
            {
                return DocordoServiceSingleton.GetInstance().LoadGroupAttributes(objectDataRequestAPI);
            }
            catch (Exception exception)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, exception);
            }
        }

        [HttpPost]
        [ActionName("Login")]
        public AuthenticatedWhoResultAPI Login(AuthenticationCredentialsAPI authenticationCredentialsAPI)
        {
            try
            {
                return DocordoServiceSingleton.GetInstance().Login(authenticationCredentialsAPI);
            }
            catch (Exception exception)
            {
                throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, exception);
            }
        }

        [HttpPost]
        [ActionName("CreateStream")]
        public String CreateStream(SocialServiceRequestAPI socialServiceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "CreateStream - Not implemented.");
        }

        [HttpPost]
        [ActionName("GetCurrentUserInfo")]
        public WhoAPI GetCurrentUserInfo(String streamId, SocialServiceRequestAPI socialServiceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "GetCurrentUserInfo - Not implemented.");
        }

        [HttpPost]
        [ActionName("GetUserInfo")]
        public WhoAPI GetUserInfo(String streamId, String userId, SocialServiceRequestAPI socialServiceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "GetUserInfo - Not implemented.");
        }

        [HttpPost]
        [ActionName("GetStreamFollowers")]
        public List<WhoAPI> GetStreamFollowers(String streamId, SocialServiceRequestAPI socialServiceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "GetStreamFollowers - Not implemented.");
        }

        [HttpPost]
        [ActionName("GetStreamMessages")]
        public Task<MessageListAPI> GetStreamMessages(String streamId, SocialServiceRequestAPI socialServiceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "GetStreamMessages - Not implemented.");
        }

        [HttpPost]
        [ActionName("ShareMessage")]
        public Task<MessageAPI> ShareMessage(String streamId)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "ShareMessage - Not implemented.");
        }

        [HttpPost]
        [ActionName("PostNewMessage")]
        public Task<MessageAPI> PostNewMessage(String streamId)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "PostNewMessage - Not implemented.");
        }

        [HttpPost]
        [ActionName("DeleteMessage")]
        public Task<String> DeleteMessage(String streamId, String messageId, SocialServiceRequestAPI socialServiceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "DeleteMessage - Not implemented.");
        }

        [HttpPost]
        [ActionName("LikeMessage")]
        public Task<String> LikeMessage(String streamId, String messageId, String like, SocialServiceRequestAPI socialServiceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "LikeMessage - Not implemented.");
        }

        [HttpPost]
        [ActionName("FollowStream")]
        public Task<String> FollowStream(String streamId, String follow, SocialServiceRequestAPI socialServiceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "FollowStream - Not implemented.");
        }

        [HttpPost]
        [ActionName("SearchUsersByName")]
        public Task<List<MentionedUserAPI>> SearchUsersByName(String streamId, String name, SocialServiceRequestAPI socialServiceRequest)
        {
            throw ErrorUtils.GetWebException(HttpStatusCode.BadRequest, "SearchUsersByName - Not implemented.");
        }
    }
}
