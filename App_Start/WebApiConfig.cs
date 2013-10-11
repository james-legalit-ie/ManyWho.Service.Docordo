using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace ManyWho.Service.Docordo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Routes.MapHttpRoute(
                name: "PluginDocordoDescribe",
                routeTemplate: "plugins/api/docordo/1/metadata",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "Describe"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoDescribeTables",
                routeTemplate: "plugins/api/docordo/1/metadata/table",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "DescribeTables"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoDescribeFields",
                routeTemplate: "plugins/api/docordo/1/metadata/field",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "DescribeFields"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoView",
                routeTemplate: "plugins/api/docordo/1/view/{actionName}",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "View",
                    actionName = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoDelete",
                routeTemplate: "plugins/api/docordo/1/data/delete",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "Delete"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoGetUserInAuthorizationContext",
                routeTemplate: "plugins/api/docordo/1/authorization",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "GetUserInAuthorizationContext"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoLoadUserAttributes",
                routeTemplate: "plugins/api/docordo/1/authorization/user/attribute",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "LoadUserAttributes"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoLoadGroupAttributes",
                routeTemplate: "plugins/api/docordo/1/authorization/group/attribute",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "LoadGroupAttributes"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoLoadUsers",
                routeTemplate: "plugins/api/docordo/1/authorization/user",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "LoadUsers"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoLoadGroups",
                routeTemplate: "plugins/api/docordo/1/authorization/group",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "LoadGroups"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoLogin",
                routeTemplate: "plugins/api/docordo/1/authentication",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "Login"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoLoad",
                constraints: new { httpMethod = new System.Web.Http.Routing.HttpMethodConstraint(HttpMethod.Post) },
                routeTemplate: "plugins/api/docordo/1/data",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "Load"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoSave",
                constraints: new { httpMethod = new System.Web.Http.Routing.HttpMethodConstraint(HttpMethod.Put) },
                routeTemplate: "plugins/api/docordo/1/data",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "Save"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoInvoke",
                routeTemplate: "plugins/api/docordo/1/{actionName}",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "Invoke",
                    actionName = RouteParameter.Optional
                }
            );

            #region SOCIAL


            config.Routes.MapHttpRoute(
                name: "PluginDocordoGetCurrentUserInfo",
                constraints: new { httpMethod = new System.Web.Http.Routing.HttpMethodConstraint(HttpMethod.Post) },
                routeTemplate: "plugins/api/docordo/1/social/stream/{streamId}/user/me",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "GetCurrentUserInfo"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoSearchUsersByName",
                routeTemplate: "plugins/api/docordo/1/social/stream/{streamId}/user/name/{name}",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "SearchUsersByName",
                    streamId = RouteParameter.Optional,
                    name = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoGetUserInfo",
                constraints: new { httpMethod = new System.Web.Http.Routing.HttpMethodConstraint(HttpMethod.Post) },
                routeTemplate: "plugins/api/docordo/1/social/stream/{streamId}/user/{userId}",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "GetUserInfo"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoGetStreamFollowers",
                routeTemplate: "plugins/api/docordo/1/social/stream/{streamId}/follower",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "GetStreamFollowers",
                    streamId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoShareMessage",
                routeTemplate: "plugins/api/docordo/1/social/stream/{streamId}/share",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "ShareMessage",
                    streamId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoPostNewMessage",
                routeTemplate: "plugins/api/docordo/1/social/stream/{streamId}/message",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "PostNewMessage",
                    streamId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoLikeMessage",
                routeTemplate: "plugins/api/docordo/1/social/stream/{streamId}/message/{messageId}/like/{like}",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "LikeMessage",
                    streamId = RouteParameter.Optional,
                    messageId = RouteParameter.Optional,
                    like = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoDeleteMessage",
                routeTemplate: "plugins/api/docordo/1/social/stream/{streamId}/message/{messageId}",
                defaults: new
                {
                    controller = "Social",
                    action = "PluginSalesforce",
                    streamId = RouteParameter.Optional,
                    messageId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoFollowStream",
                routeTemplate: "plugins/api/docordo/1/social/stream/{streamId}/follow/{follow}",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "FollowStream",
                    streamId = RouteParameter.Optional,
                    follow = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoCreateStream",
                constraints: new { httpMethod = new System.Web.Http.Routing.HttpMethodConstraint(HttpMethod.Post) },
                routeTemplate: "plugins/api/docordo/1/social/stream",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "CreateStream"
                }
            );

            config.Routes.MapHttpRoute(
                name: "PluginDocordoGetStreamMessages",
                routeTemplate: "plugins/api/docordo/1/social/stream/{streamId}",
                defaults: new
                {
                    controller = "PluginDocordo",
                    action = "GetStreamMessages",
                    streamId = RouteParameter.Optional
                }
            );


            #endregion

            // Make JSON the default format for the service
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
