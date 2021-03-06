﻿using ArcGIS.ServiceModel;
using ArcGIS.ServiceModel.Operation;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteDescriber.ServiceInterface
{
    [Restrict(VisibleInternalOnly = true)]
    [ClientCanSwapTemplates]
    [DefaultView("SiteDescription")]
    public class GatewayService : Service
    {
        ServiceStackSerializer _serializer = new ServiceStackSerializer();

        public async Task<ModelForServer> Any(AgServer request)
        {
            if (String.IsNullOrWhiteSpace(request.Url)) return null;

            var gateway = new SecureArcGISServerGateway(request.Url.AsRootUrl(), request.Username, request.Password, _serializer);
            var result = new ModelForServer { Url = gateway.RootUrl };
            var siteDescription = await gateway.DescribeSite();
            result.SiteDescription = siteDescription;
            return result;
        }
    }

    [Route("/describe")]
    public class AgServer
    {
        public String Url { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
    }

    public class ModelForServer
    {
        public String Url { get; set; }
        public SiteDescription SiteDescription { get; set; }
    }

    public class ServiceStackSerializer : ISerializer
    {
        public ServiceStackSerializer()
        {
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
            ServiceStack.Text.JsConfig.IncludeTypeInfo = false;
            ServiceStack.Text.JsConfig.ConvertObjectTypesIntoStringDictionary = true;
            ServiceStack.Text.JsConfig.IncludeNullValues = false;
        }

        public Dictionary<String, String> AsDictionary<T>(T objectToConvert) where T : CommonParameters
        {
            return objectToConvert == null ?
                null :
                ServiceStack.Text.JsonSerializer.DeserializeFromString<Dictionary<String, String>>(ServiceStack.Text.JsonSerializer.SerializeToString(objectToConvert));
        }

        public T AsPortalResponse<T>(String dataToConvert) where T : IPortalResponse
        {
            return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(dataToConvert);
        }
    }
}
