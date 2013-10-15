using System;
using System.Collections.Generic;
using ManyWho.Flow.SDK;
using ManyWho.Flow.SDK.Describe;
using ManyWho.Flow.SDK.Run.Elements.Type;
using ManyWho.Flow.SDK.Security;

namespace ManyWho.Service.Docordo
{
    public class DescribeUtils
    {
        public static DescribeValueAPI CreateDescribeValue(String contentType, String developerName, String contentValue, Boolean required)
        {
            DescribeValueAPI describeValue = null;

            describeValue = new DescribeValueAPI();
            describeValue.contentType = contentType;
            describeValue.developerName = developerName;
            describeValue.contentValue = contentValue;
            describeValue.isRequired = required;

            return describeValue;
        }

        public static ObjectAPI CreateAttributeObject(String label, String value)
        {
            ObjectAPI attributeObject = null;
            PropertyAPI attributeProperty = null;

            attributeObject = new ObjectAPI();
            attributeObject.externalId = value;
            attributeObject.developerName = ManyWhoConstants.AUTHENTICATION_AUTHENTICATION_ATTRIBUTE_OBJECT_DEVELOPER_NAME;
            attributeObject.properties = new List<PropertyAPI>();

            attributeProperty = new PropertyAPI();
            attributeProperty.developerName = ManyWhoConstants.AUTHENTICATION_ATTRIBUTE_LABEL;
            attributeProperty.contentValue = label;

            attributeObject.properties.Add(attributeProperty);

            attributeProperty = new PropertyAPI();
            attributeProperty.developerName = ManyWhoConstants.AUTHENTICATION_ATTRIBUTE_VALUE;
            attributeProperty.contentValue = value;

            attributeObject.properties.Add(attributeProperty);

            return attributeObject;
        }

        public static ObjectAPI CreateUserObject(IAuthenticatedWho authenticatedWho)
        {
            ObjectAPI userObject = null;

            userObject = new ObjectAPI();
            userObject.developerName = ManyWhoConstants.MANYWHO_USER_DEVELOPER_NAME;
            userObject.properties = new List<PropertyAPI>();

            userObject.properties.Add(CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_DIRECTORY_ID, authenticatedWho.DirectoryId));
            userObject.properties.Add(CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_DIRECTORY_NAME, authenticatedWho.DirectoryName));

            userObject.properties.Add(CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_COUNTRY, null));
            userObject.properties.Add(CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_EMAIL, authenticatedWho.Email));
            userObject.properties.Add(CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_USERNAME, authenticatedWho.Email));
            userObject.properties.Add(CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_FIRST_NAME, null));
            userObject.properties.Add(CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_LANGUAGE, null));
            userObject.properties.Add(CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_LAST_NAME, null));
            userObject.properties.Add(CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_LOCATION, null));
            userObject.properties.Add(CreateProperty(ManyWhoConstants.MANYWHO_USER_PROPERTY_USER_ID, authenticatedWho.UserId));

            return userObject;
        }

        /// <summary>
        /// Utility method for creating new properties.
        /// </summary>
        public static PropertyAPI CreateProperty(String developerName, String contentValue)
        {
            PropertyAPI propertyAPI = null;

            propertyAPI = new PropertyAPI();
            propertyAPI.developerName = developerName;
            propertyAPI.contentValue = contentValue;

            return propertyAPI;
        }
    }
}
