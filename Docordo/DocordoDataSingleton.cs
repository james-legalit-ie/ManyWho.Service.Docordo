namespace ManyWho.Service.Docordo
{
    using System;
using System.Collections.Generic;
using System.Diagnostics;
using DocordoAPI;
using DocordoAPI.Model.Bean;
using DocordoAPI.Model.Domain;
using ManyWho.Flow.SDK;
using ManyWho.Flow.SDK.Draw.Elements.Type;
using ManyWho.Flow.SDK.Run.Elements.Type;
using Newtonsoft.Json;

    public class DocordoDataSingleton
    {
        private static DocordoDataSingleton docordoDataSingleton;

        private DocordoDataSingleton() { }

        public static DocordoDataSingleton GetInstance()
        {
            if (docordoDataSingleton == null)
            {
                docordoDataSingleton = new DocordoDataSingleton();
            }

            return docordoDataSingleton;
        }

        public List<ObjectAPI> CreateMatter(string authenticationUrl, string ebikkoSessionId, string cookieJSESSIONID, string recordNumber, string description) {
            DocordoNodeCreateResponse docordoNodeCreateResponse = DocordoAPI.DocordoService.GetInstance().CreateMatter(authenticationUrl, ebikkoSessionId, cookieJSESSIONID, new DocordoCreateMatterBean() { ContainerRecordNumber = recordNumber, Description = description });            
            throw new NotImplementedException();
        }

        public List<TypeElementRequestAPI> GetTypeElements(String authenticationUrl, String username, String password, String ebikkoSessionID, String jSessionID)
        {
            List<TypeElementRequestAPI> typeElements = new List<TypeElementRequestAPI>();

            DocordoService docordoService = DocordoService.GetInstance();

            Trace.TraceInformation("// Login to the service");
            DocordoLoginResponse docordoLoginResponse = docordoService.Login(authenticationUrl, username, password);

            System.Diagnostics.Trace.TraceInformation("// Get all the objects available in the org");
            DocordoNodeTypeListResponse describeGlobalResult = docordoService.ListNodeTypes(authenticationUrl, docordoLoginResponse.EbikkoSessionId, docordoLoginResponse.CookieJSESSIONID);

            Trace.TraceInformation("// Get the names of all of the objects so we can then do a full object query");

            foreach (DocordoNodeTypeDetail docordoNodeTypeStub in describeGlobalResult.Results)
            {
                Trace.TraceInformation("//DocordoNodeTypeDetail docordoNodeTypeStub in describeGlobalResult.Results");

                DocordoNodeTypeDetailResponse docordoNodeTypeDetailResponse = docordoService.LoadNodeTypeDetail(authenticationUrl, docordoLoginResponse.EbikkoSessionId, docordoLoginResponse.CookieJSESSIONID, docordoNodeTypeStub.NodeTypeId);

                if (docordoNodeTypeDetailResponse.Results != null)
                {
                    foreach (DocordoNodeTypeDetail docordoNodeTypeDetail in docordoNodeTypeDetailResponse.Results)
                    {
                        Trace.TraceInformation("DocordoNodeTypeDetail docordoNodeTypeDetail in docordoNodeTypeDetailResponse.Results");
                        Trace.TraceInformation(JsonConvert.SerializeObject(docordoNodeTypeDetail));

                        TypeElementRequestAPI typeElement = new TypeElementRequestAPI();
                        typeElement.developerName = docordoNodeTypeDetail.Name;
                        typeElement.developerSummary = docordoNodeTypeDetail.Name;
                        typeElement.bindings = new List<TypeElementBindingAPI>();

                        TypeElementBindingAPI typeElementBinding = new TypeElementBindingAPI();
                        typeElementBinding.databaseTableName = docordoNodeTypeDetail.Name;
                        typeElementBinding.developerName = authenticationUrl + " " + docordoNodeTypeDetail.Name + " Binding";
                        typeElementBinding.developerSummary = "The binding to save " + docordoNodeTypeDetail.Name + " objects into " + authenticationUrl;
                        typeElementBinding.propertyBindings = new List<TypeElementPropertyBindingAPI>();

                        typeElement.bindings.Add(typeElementBinding);
                        typeElement.properties = new List<TypeElementPropertyAPI>();

                        if (docordoNodeTypeDetail.Properties != null)
                        {
                            foreach (DocordoProperty docordoProperty in docordoNodeTypeDetail.Properties)
                            {
                                Trace.TraceInformation("DocordoProperty docordoProperty in docordoNodeTypeDetail.Properties");
                                Trace.TraceInformation(JsonConvert.SerializeObject(docordoProperty));

                                TypeElementPropertyBindingAPI typeElementFieldBinding = new TypeElementPropertyBindingAPI();
                                typeElementFieldBinding.databaseFieldName = docordoProperty.Name;
                                typeElementFieldBinding.databaseContentType = docordoProperty.Type.ToString();
                                typeElementFieldBinding.typeElementPropertyDeveloperName = docordoProperty.Name;

                                typeElementBinding.propertyBindings.Add(typeElementFieldBinding);

                                TypeElementPropertyAPI typeElementEntry = new TypeElementPropertyAPI();
                                typeElementEntry.contentType = this.TranslateToManyWhoContentType(docordoProperty.Type);
                                typeElementEntry.developerName = docordoProperty.Name;
                                typeElementEntry.typeElementDeveloperName = docordoProperty.Name;

                                typeElement.properties.Add(typeElementEntry);
                            }
                        }
                        else
                        {
                            Trace.TraceInformation("docordoNodeTypeDetail.Properties == null");
                            Trace.TraceInformation(JsonConvert.SerializeObject(docordoNodeTypeDetail));
                        }
                        DocordoAttributesResponse docordoAttributesResponse = docordoService.LoadAttributesByNodeType(authenticationUrl, docordoLoginResponse.EbikkoSessionId, docordoLoginResponse.CookieJSESSIONID, docordoNodeTypeDetail.NodeTypeId);
                        if (docordoAttributesResponse.Results != null)
                        {
                            foreach (DocordoAttribute docordoAttribute in docordoAttributesResponse.Results)
                            {
                                Trace.TraceInformation("DocordoAttribute docordoAttribute in docordoAttributesResponse.Results");
                                Trace.TraceInformation(JsonConvert.SerializeObject(docordoAttribute));

                                TypeElementPropertyBindingAPI typeElementFieldBinding = new TypeElementPropertyBindingAPI();
                                typeElementFieldBinding.databaseFieldName = docordoAttribute.Name;
                                typeElementFieldBinding.databaseContentType = docordoAttribute.Type;
                                typeElementFieldBinding.typeElementPropertyDeveloperName = docordoAttribute.Name;

                                typeElementBinding.propertyBindings.Add(typeElementFieldBinding);

                                TypeElementPropertyAPI typeElementEntry = new TypeElementPropertyAPI();
                                typeElementEntry.contentType = docordoAttribute.Type;
                                typeElementEntry.developerName = docordoAttribute.Name;
                                typeElementEntry.typeElementDeveloperName = docordoAttribute.Name;

                                typeElement.properties.Add(typeElementEntry);
                            }
                        }
                        else
                        {
                            Trace.TraceInformation("docordoAttributesResponse.Results == null");
                            Trace.TraceInformation(JsonConvert.SerializeObject(docordoAttributesResponse));
                        }

                        typeElements.Add(typeElement);
                    }
                }
                else
                {
                    Trace.TraceInformation("docordoNodeTypeDetailResponse.Results == null");
                    Trace.TraceInformation(JsonConvert.SerializeObject(describeGlobalResult));
                }
            }

            return typeElements;
        }

        public DocordoLoginResponse Login(String authenticationUrl, String username, String password)
        {
            DocordoLoginResponse docordoLoginResponse = null;
            try
            {
                DocordoAPI.DocordoService.GetInstance().Logout(authenticationUrl, username, password);
            }
            finally
            {
                docordoLoginResponse = DocordoAPI.DocordoService.GetInstance().Login(authenticationUrl, username, password);
            }
            return docordoLoginResponse;
        }

        private string TranslateToManyWhoContentType(int dataType)
        {
            string contentType = ManyWhoConstants.CONTENT_TYPE_OBJECT;

            switch (dataType)
            {
                case 100:
                case 800:
                    contentType = ManyWhoConstants.CONTENT_TYPE_STRING;
                    break;
                case 200:
                case 300:
                    contentType = ManyWhoConstants.CONTENT_TYPE_NUMBER;
                    break;
                case 400:
                    contentType = ManyWhoConstants.CONTENT_TYPE_BOOLEAN;
                    break;
                case 500:
                    contentType = ManyWhoConstants.CONTENT_TYPE_DATETIME;
                    break;
                case 1200:
                    contentType = ManyWhoConstants.CONTENT_TYPE_OBJECT;
                    break;
                default:
                    contentType = ManyWhoConstants.CONTENT_TYPE_STRING;
                    break;
            }

            return contentType;
        }
    }
}