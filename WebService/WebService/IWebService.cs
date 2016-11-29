using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWebService" in both code and config file together.
    [ServiceContract]
    public interface IWebService
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Json/Default")]
        string Default(Stream Data);

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Json/GetFile/{token}")]
        string GetFile(Stream Data, string token);

        [OperationContract]
         [WebInvoke(Method = "POST",
             ResponseFormat = WebMessageFormat.Json,
             RequestFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.WrappedRequest, // als ge een error krijgt bij compillen herschrijf WrappedRequest (unknown bug)
             UriTemplate = "Json/SaveFile/{id}/{max}/{current}/{token}")]
         string SaveFile(Stream Data, string id, string max, string current,string token);

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Json/Checkdiv/{token}")]
        string CheckDivisionOfData(Stream Data, string token);

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "Json/GetFileNames/{token}")]
        string GetFileNames(Stream Data, string token);


        //12.	functie server die ontvangt en json string uitgeeft (functie aanwezig maar aanpassingen nodig)
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Json/DeleteFile/{token}")]
        string DeleteFile(Stream Data, string token);


        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Json/ValidateUser/{token}")]
        string ValidateUser(Stream Data,string token);

        [OperationContract]//sprint 4 story 7 gebruikers afhalen
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Json/GetUsers/{token}")]
        string GetUsers(Stream Data, string token);//moet deze data meegeven?

        [OperationContract]//sprint 4 story 7 bestanden delen
        [WebInvoke(Method = "POST",
           ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           UriTemplate = "Json/SetUsers/{id}/{token}")]
        string SetUsers(Stream Data, string id, string token);


    }
}
