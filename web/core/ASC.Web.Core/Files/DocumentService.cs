﻿/*
 *
 * (c) Copyright Ascensio System Limited 2010-2016
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using ASC.Core;
using JWT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Formatting = Newtonsoft.Json.Formatting;

namespace ASC.Web.Core.Files
{
    /// <summary>
    /// Class service connector
    /// </summary>
    public static class DocumentService
    {
        /// <summary>
        /// Timeout to request conversion
        /// </summary>
        public static int Timeout = 120000;

        /// <summary>
        /// Number of tries request conversion
        /// </summary>
        public static int MaxTry = 3;

        /// <summary>
        /// Translation key to a supported form.
        /// </summary>
        /// <param name="expectedKey">Expected key</param>
        /// <returns>Supported key</returns>
        public static string GenerateRevisionId(string expectedKey)
        {
            expectedKey = expectedKey ?? "";
            const int maxLength = 20;
            if (expectedKey.Length > maxLength) expectedKey = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(expectedKey)));
            var key = Regex.Replace(expectedKey, "[^0-9a-zA-Z_]", "_");
            return key.Substring(key.Length - Math.Min(key.Length, maxLength));
        }

        /// <summary>
        /// The method is to convert the file to the required format
        /// </summary>
        /// <param name="documentConverterUrl">Url to the service of conversion</param>
        /// <param name="documentUri">Uri for the document to convert</param>
        /// <param name="fromExtension">Document extension</param>
        /// <param name="toExtension">Extension to which to convert</param>
        /// <param name="documentRevisionId">Key for caching on service</param>
        /// <param name="isAsync">Perform conversions asynchronously</param>
        /// <param name="signatureSecret">Secret key to generate the token</param>
        /// <param name="convertedDocumentUri">Uri to the converted document</param>
        /// <returns>The percentage of completion of conversion</returns>
        /// <example>
        /// string convertedDocumentUri;
        /// GetConvertedUri("http://helpcenter.teamlab.com/content/GettingStarted.pdf", ".pdf", ".docx", "469971047", false, out convertedDocumentUri);
        /// </example>
        /// <exception>
        /// </exception>
        public static int GetConvertedUri(
            string documentConverterUrl,
            string documentUri,
            string fromExtension,
            string toExtension,
            string documentRevisionId,
            bool isAsync,
            string signatureSecret,
            out string convertedDocumentUri)
        {
            fromExtension = string.IsNullOrEmpty(fromExtension) ? Path.GetExtension(documentUri) : fromExtension;
            if (string.IsNullOrEmpty(fromExtension)) throw new ArgumentNullException("fromExtension", "Document's extension for conversion is not known");
            if (string.IsNullOrEmpty(toExtension)) throw new ArgumentNullException("toExtension", "Extension for conversion is not known");

            var title = Path.GetFileName(documentUri ?? "");
            title = string.IsNullOrEmpty(title) || title.Contains("?") ? Guid.NewGuid().ToString() : title;

            documentRevisionId = string.IsNullOrEmpty(documentRevisionId)
                                     ? documentUri
                                     : documentRevisionId;
            documentRevisionId = GenerateRevisionId(documentRevisionId);

            var request = (HttpWebRequest)WebRequest.Create(documentConverterUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = Timeout;

            var body = new ConvertionBody
                {
                    Async = isAsync,
                    FileType = fromExtension.Trim('.'),
                    Key = documentRevisionId,
                    OutputType = toExtension.Trim('.'),
                    Title = title,
                    Url = documentUri,
                };

            if (!string.IsNullOrEmpty(signatureSecret))
            {
                var payload = new Dictionary<string, object>
                    {
                        { "payload", body }
                    };
                JsonWebToken.JsonSerializer = new JwtSerializer();
                var token = JsonWebToken.Encode(payload, signatureSecret, JwtHashAlgorithm.HS256);
                request.Headers.Add(FileUtility.SignatureHeader, "Bearer " + token);
            }

            var bodyString = JsonConvert.SerializeObject(body);

            var bytes = Encoding.UTF8.GetBytes(bodyString ?? "");
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            // hack. http://ubuntuforums.org/showthread.php?t=1841740
            if (WorkContext.IsMono)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
            }

            XDocument xDocumentResponse;
            WebResponse response = null;
            try
            {
                var countTry = 0;
                Stream stream = null;
                while (countTry < MaxTry)
                {
                    try
                    {
                        countTry++;
                        response = request.GetResponse();
                        stream = response.GetResponseStream();
                        break;
                    }
                    catch (WebException ex)
                    {
                        if (ex.Status != WebExceptionStatus.Timeout)
                        {
                            throw new HttpException((int)HttpStatusCode.BadRequest, ex.Message, ex);
                        }
                    }
                }
                if (countTry == MaxTry)
                {
                    throw new WebException("Timeout", WebExceptionStatus.Timeout);
                }

                if (stream == null) throw new WebException("Could not get an answer");
                xDocumentResponse = XDocument.Load(new XmlTextReader(stream));
                stream.Dispose();
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }

            return GetResponseUri(xDocumentResponse, out convertedDocumentUri);
        }

        /// <summary>
        /// Placing the document in the storage service
        /// </summary>
        /// <param name="documentStorageUrl">Url to the storage service</param>
        /// <param name="fileStream">Stream of document</param>
        /// <param name="contentType">Mime type</param>
        /// <param name="documentRevisionId">Key for caching on service, whose used in editor</param>
        /// <param name="signatureSecret">Secret key to generate the token</param>
        /// <returns>Uri to document in the storage</returns>
        public static string GetExternalUri(
            string documentStorageUrl,
            Stream fileStream,
            string contentType,
            string documentRevisionId,
            string signatureSecret)
        {
            var urlTostorage = String.Format("{0}?key={1}",
                                             documentStorageUrl,
                                             documentRevisionId);

            var request = (HttpWebRequest)WebRequest.Create(urlTostorage);
            request.Method = "POST";
            request.ContentType = contentType;
            request.Timeout = Timeout;

            if (!string.IsNullOrEmpty(signatureSecret))
            {
                var payload = new Dictionary<string, object>
                    {
                        {
                            "query", new Dictionary<string, string>
                                {
                                    {
                                        "key", documentRevisionId
                                    }
                                }
                        }
                    };
                JsonWebToken.JsonSerializer = new JwtSerializer();
                var token = JsonWebToken.Encode(payload, signatureSecret, JwtHashAlgorithm.HS256);
                request.Headers.Add(FileUtility.SignatureHeader, "Bearer " + token);
            }

            if (fileStream != null)
            {
                if (fileStream.Length > 0)
                    request.ContentLength = fileStream.Length;

                const int bufferSize = 2048;
                var buffer = new byte[bufferSize];
                int readed;
                while ((readed = fileStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    request.GetRequestStream().Write(buffer, 0, readed);
                }
            }

            // hack. http://ubuntuforums.org/showthread.php?t=1841740
            if (WorkContext.IsMono)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
            }

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                if (stream == null) throw new WebException("Could not get an answer");
                var xDocumentResponse = XDocument.Load(new XmlTextReader(stream));
                string externalUri;
                GetResponseUri(xDocumentResponse, out externalUri);
                return externalUri;
            }
        }

        /// <summary>
        /// Request to Document Server with command
        /// </summary>
        /// <param name="documentTrackerUrl">Url to the command service</param>
        /// <param name="method">Name of method</param>
        /// <param name="documentRevisionId">Key for caching on service, whose used in editor</param>
        /// <param name="callbackUrl">Url to the callback handler</param>
        /// <param name="users">users id for drop</param>
        /// <param name="meta">file meta data for update</param>
        /// <param name="signatureSecret">Secret key to generate the token</param>
        /// <param name="version">server version</param>
        /// <returns>Response</returns>
        public static CommandResultTypes CommandRequest(
            string documentTrackerUrl,
            CommandMethod method,
            string documentRevisionId,
            string callbackUrl,
            string[] users,
            MetaData meta,
            string signatureSecret,
            out string version)
        {
            var request = (HttpWebRequest)WebRequest.Create(documentTrackerUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = Timeout;

            var body = new CommandBody
                {
                    Command = method,
                    Key = documentRevisionId,
                };

            if (!string.IsNullOrEmpty(callbackUrl)) body.Callback = callbackUrl;
            if (users != null && users.Length > 0) body.Users = users;
            if (meta != null) body.Meta = meta;

            if (!string.IsNullOrEmpty(signatureSecret))
            {
                var payload = new Dictionary<string, object>
                    {
                        { "payload", body }
                    };
                JsonWebToken.JsonSerializer = new JwtSerializer();
                var token = JsonWebToken.Encode(payload, signatureSecret, JwtHashAlgorithm.HS256);
                request.Headers.Add(FileUtility.SignatureHeader, "Bearer " + token);
            }

            var bodyString = JsonConvert.SerializeObject(body);

            var bytes = Encoding.UTF8.GetBytes(bodyString ?? "");
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            // hack. http://ubuntuforums.org/showthread.php?t=1841740
            if (WorkContext.IsMono)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
            }

            string data;
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                if (stream == null)
                {
                    throw new Exception("Response is null");
                }

                using (var reader = new StreamReader(stream))
                {
                    data = reader.ReadToEnd();
                }
            }

            var jResponse = JObject.Parse(data);

            try
            {
                version = jResponse.Value<string>("version");
            }
            catch (Exception)
            {
                version = "0";
            }

            return (CommandResultTypes)jResponse.Value<int>("error");
        }

        public enum CommandMethod
        {
            Info,
            Drop,
            Saved, //not used
            Version,
            ForceSave, //not used
            Meta,
        }

        public enum CommandResultTypes
        {
            NoError = 0,
            DocumentIdError = 1,
            ParseError = 2,
            UnknownError = 3,
            NotModify = 4,
            UnknownCommand = 5,
            Token = 6,
            TokenExpire = 7,
        }

        [Serializable]
        [DataContract(Name = "Command", Namespace = "")]
        [DebuggerDisplay("{Command} ({Key})")]
        private class CommandBody
        {
            public CommandMethod Command { get; set; }

            [DataMember(Name = "c", IsRequired = true)]
            public string C
            {
                get { return Command.ToString().ToLower(CultureInfo.InvariantCulture); }
            }

            [DataMember(Name = "callback", IsRequired = false, EmitDefaultValue = false)]
            public string Callback { get; set; }

            [DataMember(Name = "key", IsRequired = true)]
            public string Key { get; set; }

            [DataMember(Name = "meta", IsRequired = false, EmitDefaultValue = false)]
            public MetaData Meta { get; set; }

            [DataMember(Name = "users", IsRequired = false, EmitDefaultValue = false)]
            public string[] Users { get; set; }

            //not used
            [DataMember(Name = "userdata", IsRequired = false, EmitDefaultValue = false)]
            public string UserData { get; set; }
        }

        [Serializable]
        [DataContract(Name = "meta", Namespace = "")]
        [DebuggerDisplay("{Title}")]
        public class MetaData
        {
            [DataMember(Name = "title")]
            public string Title;
        }

        [Serializable]
        [DataContract(Name = "Converion", Namespace = "")]
        [DebuggerDisplay("{Title} from {FileType} to {OutputType} ({Key})")]
        private class ConvertionBody
        {
            [DataMember(Name = "async")]
            public bool Async { get; set; }

            [DataMember(Name = "filetype", IsRequired = true)]
            public string FileType { get; set; }

            [DataMember(Name = "key", IsRequired = true)]
            public string Key { get; set; }

            [DataMember(Name = "outputtype", IsRequired = true)]
            public string OutputType { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "url", IsRequired = true)]
            public string Url { get; set; }
        }

        public class DocumentServiceException : Exception
        {
            public DocumentServiceException(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// Processing document received from the editing service
        /// </summary>
        /// <param name="xDocumentResponse">The resulting xml from editing service</param>
        /// <param name="responseUri">Uri to the converted document</param>
        /// <returns>The percentage of completion of conversion</returns>
        private static int GetResponseUri(XDocument xDocumentResponse, out string responseUri)
        {
            var responceFromConvertService = xDocumentResponse.Root;
            if (responceFromConvertService == null) throw new WebException("Invalid answer format");

            var errorElement = responceFromConvertService.Element("Error");
            if (errorElement != null) ProcessResponceError(Convert.ToInt32(errorElement.Value));

            var endConvert = responceFromConvertService.Element("EndConvert");
            if (endConvert == null) throw new WebException("Invalid answer format");
            var isEndConvert = Convert.ToBoolean(endConvert.Value);

            var resultPercent = 0;
            responseUri = string.Empty;
            if (isEndConvert)
            {
                var fileUrl = responceFromConvertService.Element("FileUrl");
                if (fileUrl == null) throw new WebException("Invalid answer format");

                responseUri = fileUrl.Value;
                resultPercent = 100;
            }
            else
            {
                var percent = responceFromConvertService.Element("Percent");
                if (percent != null)
                    resultPercent = Convert.ToInt32(percent.Value);
                resultPercent = resultPercent >= 100 ? 99 : resultPercent;
            }

            return resultPercent;
        }

        /// <summary>
        /// Generate an error code table
        /// </summary>
        /// <param name="errorCode">Error code</param>
        private static void ProcessResponceError(int errorCode)
        {
            string errorMessage;
            switch (errorCode)
            {
                case -22: // public const int c_nErrorUserCountExceed = -22;
                    errorMessage = "user count exceed";
                    break;
                case -21: // public const int c_nErrorKeyExpire = -21;
                    errorMessage = "signature expire";
                    break;
                case -20: // public const int c_nErrorVKeyEncrypt = -20;
                    errorMessage = "encrypt signature";
                    break;
                case -8: // public const int c_nErrorFileVKey = -8;
                    errorMessage = "document signature";
                    break;
                case -6: // public const int c_nErrorDatabase = -6;
                    errorMessage = "database";
                    break;
                case -4: // public const int c_nErrorDownloadError = -4;
                    errorMessage = "download";
                    break;
                case -3: // public const int c_nErrorConvertationError = -3;
                    errorMessage = "convertation";
                    break;
                case -2: // public const int c_nErrorConvertationTimeout = -2;
                    errorMessage = "convertation timeout";
                    break;
                case -1: // public const int c_nErrorUnknown = -1;
                    errorMessage = "unknown error";
                    break;
                default: // public const int c_nErrorNo = 0;
                    errorMessage = "errorCode = " + errorCode;
                    break;
            }

            throw new DocumentServiceException(errorMessage);
        }


        public class JwtSerializer : IJsonSerializer
        {
            private class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
            {
                protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
                {
                    var contract = base.CreateDictionaryContract(objectType);

                    contract.DictionaryKeyResolver = propertyName => propertyName;

                    return contract;
                }
            }

            public string Serialize(object obj)
            {
                var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCaseExceptDictionaryKeysResolver(),
                        NullValueHandling = NullValueHandling.Ignore,
                    };

                return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
            }

            public T Deserialize<T>(string json)
            {
                var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCaseExceptDictionaryKeysResolver(),
                        NullValueHandling = NullValueHandling.Ignore,
                    };

                return JsonConvert.DeserializeObject<T>(json, settings);
            }
        }
    }
}