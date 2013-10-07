using System;
using System.Collections.Generic;
using System.Diagnostics;
using DocordoAPI;
using DocordoAPI.Model.Domain;
using ManyWho.Flow.SDK;
using ManyWho.Flow.SDK.Draw.Elements.Type;
using Newtonsoft.Json;

namespace ManyWho.Service.Docordo
{
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
                        Trace.TraceInformation(docordoNodeTypeDetail.Name);
                        Trace.TraceInformation(docordoNodeTypeDetail.NodeTypeId);

                        TypeElementRequestAPI typeElement = new TypeElementRequestAPI();
                        typeElement.developerName = docordoNodeTypeDetail.Name;
                        typeElement.developerSummary = docordoNodeTypeDetail.Name;
                        typeElement.serviceElementId = docordoNodeTypeDetail.NodeTypeId;
                        typeElement.bindings = new List<TypeElementBindingAPI>();

                        TypeElementBindingAPI typeElementBinding = new TypeElementBindingAPI();
                        typeElementBinding.tableName = docordoNodeTypeDetail.Name;
                        typeElementBinding.developerName = authenticationUrl + " " + docordoNodeTypeDetail.Name + " Binding";
                        typeElementBinding.developerSummary = "The binding to save " + docordoNodeTypeDetail.Name + " objects into " + authenticationUrl;
                        typeElementBinding.fieldBindings = new List<TypeElementFieldBindingAPI>();

                        typeElement.bindings.Add(typeElementBinding);
                        typeElement.typeElementEntries = new List<TypeElementEntryAPI>();

                        if (docordoNodeTypeDetail.Properties != null)
                        {
                            foreach (DocordoProperty docordoProperty in docordoNodeTypeDetail.Properties)
                            {
                                Trace.TraceInformation("DocordoProperty docordoProperty in docordoNodeTypeDetail.Properties");
                                Trace.TraceInformation(JsonConvert.SerializeObject(docordoProperty));                                

                                TypeElementFieldBindingAPI typeElementFieldBinding = new TypeElementFieldBindingAPI();
                                typeElementFieldBinding.fieldName = docordoProperty.Name;
                                typeElementFieldBinding.contentType = docordoProperty.Type.ToString();
                                typeElementFieldBinding.typeElementEntryDeveloperName = docordoProperty.Name;
                                typeElementFieldBinding.typeElementEntryId = docordoProperty.PropertyId;

                                typeElementBinding.fieldBindings.Add(typeElementFieldBinding);

                                TypeElementEntryAPI typeElementEntry = new TypeElementEntryAPI();
                                typeElementEntry.contentType = this.TranslateToManyWhoContentType(docordoProperty.Type);
                                typeElementEntry.developerName = docordoProperty.Name;
                                typeElementEntry.typeElementDeveloperName = docordoProperty.Name;
                                typeElementEntry.typeElementId = docordoProperty.PropertyId;

                                typeElement.typeElementEntries.Add(typeElementEntry);
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

                                TypeElementFieldBindingAPI typeElementFieldBinding = new TypeElementFieldBindingAPI();
                                typeElementFieldBinding.fieldName = docordoAttribute.Name;
                                typeElementFieldBinding.contentType = docordoAttribute.Type;
                                typeElementFieldBinding.typeElementEntryDeveloperName = docordoAttribute.Name;
                                typeElementFieldBinding.typeElementEntryId = docordoAttribute.PropertyId;

                                typeElementBinding.fieldBindings.Add(typeElementFieldBinding);

                                TypeElementEntryAPI typeElementEntry = new TypeElementEntryAPI();
                                typeElementEntry.contentType = docordoAttribute.Type;
                                typeElementEntry.developerName = docordoAttribute.Name;
                                typeElementEntry.typeElementDeveloperName = docordoAttribute.Name;
                                typeElementEntry.typeElementId = docordoAttribute.PropertyId;

                                typeElement.typeElementEntries.Add(typeElementEntry);
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
                else {
                    Trace.TraceInformation("docordoNodeTypeDetailResponse.Results == null");
                    Trace.TraceInformation(JsonConvert.SerializeObject(describeGlobalResult));
                }
            }

            return typeElements;
        }

        public DocordoLoginResponse Login(String authenticationUrl, String username, String password)
        {

            return DocordoAPI.DocordoService.GetInstance().Login(authenticationUrl, username, password);
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
                    contentType = ManyWhoConstants.CONTENT_TYPE_NUMBER;
                    break;
                case 400:
                    contentType = ManyWhoConstants.CONTENT_TYPE_BOOLEAN;
                    break;
                case 500:
                    contentType = ManyWhoConstants.CONTENT_TYPE_DATETIME;
                    break;
                default:
                    contentType = ManyWhoConstants.CONTENT_TYPE_OBJECT;
                    break;
            }

            return contentType;
        }
    }
}