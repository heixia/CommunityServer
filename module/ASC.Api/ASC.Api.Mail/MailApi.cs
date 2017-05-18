/*
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
using System.Configuration;
using System.Globalization;
using ASC.Api.Attributes;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using ASC.Common.Threading;
using ASC.Core;
using ASC.Mail.Aggregator;
using ASC.Mail.Aggregator.Common.Logging;
using ASC.Mail.Aggregator.ComplexOperations.Base;
using ASC.Web.Studio.Core;

namespace ASC.Api.Mail
{
    public partial class MailApi : IApiEntryPoint
    {
        private readonly ApiContext _context;

        private MailBoxManager _mailBoxManager;
        private ILogger _log;

        public const int DEFAULT_PAGE_SIZE = 25;

        ///<summary>
        /// Api name entry
        ///</summary>
        public string Name
        {
            get { return "mail"; }
        }

        private MailBoxManager MailBoxManager
        {
            get { return _mailBoxManager ?? (_mailBoxManager = new MailBoxManager(Logger)); }
        }

        private ILogger Logger
        {
            get { return _log ?? (_log = LoggerFactory.GetLogger(LoggerFactory.LoggerType.Log4Net, "ASC.Api")); }
        }

        private int TenantId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        private string Username
        {
            get { return SecurityContext.CurrentAccount.ID.ToString(); }
        }

        private CultureInfo CurrentCulture
        {
            get
            {
                var u = CoreContext.UserManager.GetUsers(new Guid(Username));

                var culture = !string.IsNullOrEmpty(u.CultureName) ? u.GetCulture() : CoreContext.TenantManager.GetCurrentTenant().GetCulture();

                return culture;
            }
        }

        private bool IsSignalRAvailable
        {
            get { return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["web.hub"]); }
        }

        private string MailDaemonEmail
        {
            get { return ConfigurationManager.AppSettings["mail.daemon-email"] ?? "mail-daemon@onlyoffice.com"; }
        }

        /// <summary>
        /// Limit result per Contact System
        /// </summary>
        private int MailAutocompleteMaxCountPerSystem
        {
            get
            {
                var count = 20;
                if(ConfigurationManager.AppSettings["mail.autocomplete-max-count"] == null) 
                   return count;

                int.TryParse(ConfigurationManager.AppSettings["mail.autocomplete-max-count"], out count);
                return count;
            }
        }

        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        private int MailAutocompleteTimeout
        {
            get
            {
                var count = 3000;
                if (ConfigurationManager.AppSettings["mail.autocomplete-timeout"] == null)
                    return count;

                int.TryParse(ConfigurationManager.AppSettings["mail.autocomplete-timeout"], out count);
                return count;
            }
        }

        /// <summary>
        /// Need any mail's body http links change to proxy handler
        /// </summary>
        private bool NeedProxyHttp
        {
            get { return SetupInfo.IsVisibleSettings("ProxyHttpContent"); }
        }

        /// <summary>
        /// Permit errors of SSL certificates
        /// </summary>
        private bool SslCertificatesErrorPermit
        {
            get
            {
                return ConfigurationManager.AppSettings["mail.certificate-permit"] != null &&
                       Convert.ToBoolean(ConfigurationManager.AppSettings["mail.certificate-permit"]);
            }
        }

        /// <summary>
        /// Protocol log path (if not empty then enabled)
        /// </summary>
        private string ProtocolLogPath
        {
            get
            {
                return ConfigurationManager.AppSettings["mail.protocol-log-path"] ?? "";
            }
        }

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="context"></param>
        public MailApi(ApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all Mail runnung operations (only complex)
        /// </summary>
        /// <short>
        /// Get all Mail running complex operations
        /// </short>
        /// <returns>list of MailOperationResult</returns>
        [Read("operations")]
        public List<MailOperationStatus> GetMailOperations()
        {
            var list = MailBoxManager.GetMailOperations(TranslateMailOperationStatus);
            return list;
        }

        /// <summary>
        /// Returns Mail complex operation status
        /// </summary>
        /// <short>
        /// Get Mail complex operation status
        /// </short>
        /// <param name="operationId">Id of operation</param>
        /// <returns>MailOperationResult</returns>
        [Read("operations/{operationId}")]
        public MailOperationStatus GetMailOperation(string operationId)
        {
            return MailBoxManager.GetMailOperationStatus(operationId, TranslateMailOperationStatus);
        }

        /// <summary>
        /// Method for translation mail operation statuses
        /// </summary>
        /// <param name="op">instance of DistributedTask</param>
        /// <returns>translated status text</returns>
        private static string TranslateMailOperationStatus(DistributedTask op)
        {
            var type = op.GetProperty<MailOperationType>(MailOperation.OPERATION_TYPE);
            var status = op.GetProperty<string>(MailOperation.STATUS);
            //TODO: Move strings to Resource file
            switch (type)
            {
                case MailOperationType.RemoveMailbox:
                {
                    var progress = op.GetProperty<MailOperationRemoveMailboxProgress>(MailOperation.PROGRESS);
                    switch (progress)
                    {
                        case MailOperationRemoveMailboxProgress.Init:
                            return "Setup tenant and user";
                        case MailOperationRemoveMailboxProgress.RemoveFromDb:
                            return "Remove mailbox from Db";
                        case MailOperationRemoveMailboxProgress.FreeQuota:
                            return "Decrease newly freed quota space";
                        case MailOperationRemoveMailboxProgress.RecalculateFolder:
                            return "Recalculate folders counters";
                        case MailOperationRemoveMailboxProgress.ClearCache:
                            return "Clear accounts cache";
                        case MailOperationRemoveMailboxProgress.Finished:
                            return "Finished";
                        default:
                            return status;
                    }
                }
                case MailOperationType.RecalculateFolders:
                {
                    var progress = op.GetProperty<MailOperationRecalculateMailboxProgress>(MailOperation.PROGRESS);
                    switch (progress)
                    {
                        case MailOperationRecalculateMailboxProgress.Init:
                            return "Setup tenant and user";
                        case MailOperationRecalculateMailboxProgress.CountUnreadMessages:
                            return "Calculate unread messages";
                        case MailOperationRecalculateMailboxProgress.CountTotalMessages:
                            return "Calculate total messages";
                        case MailOperationRecalculateMailboxProgress.CountUreadConversation:
                            return "Calculate unread conversations";
                        case MailOperationRecalculateMailboxProgress.CountTotalConversation:
                            return "Calculate total conversations";
                        case MailOperationRecalculateMailboxProgress.UpdateFoldersCounters:
                            return "Update folders counters";
                        case MailOperationRecalculateMailboxProgress.Finished:
                            return "Finished";
                        default:
                            return status;
                    }
                }
                default:
                    return status;
            }
        }
    }
}
