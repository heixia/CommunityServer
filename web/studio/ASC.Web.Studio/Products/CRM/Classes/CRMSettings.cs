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


#region Import

using System;
using System.Runtime.Serialization;
using ASC.Core.Common.Settings;
using ASC.CRM.Core;


#endregion

namespace ASC.Web.CRM.Classes
{
    [Serializable]
    [DataContract]
    public class SMTPServerSetting
    {

        public SMTPServerSetting()
        {
            Host = String.Empty;
            Port = 0;
            EnableSSL = false;
            RequiredHostAuthentication = false;
            HostLogin = String.Empty;
            HostPassword = String.Empty;
            SenderDisplayName = String.Empty;
            SenderEmailAddress = String.Empty;
        }

        [DataMember]
        public String Host { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public bool EnableSSL { get; set; }

        [DataMember]
        public bool RequiredHostAuthentication { get; set; }

        [DataMember]
        public String HostLogin { get; set; }

        [DataMember]
        public String HostPassword { get; set; }

        [DataMember]
        public String SenderDisplayName { get; set; }

        [DataMember]
        public String SenderEmailAddress { get; set; }

    }

    [Serializable]
    [DataContract]
    public class InvoiceSetting
    {
        public InvoiceSetting()
        {
            Autogenerated = false;
            Prefix = String.Empty;
            Number = String.Empty;
            Terms = String.Empty;
        }

        public static InvoiceSetting DefaultSettings
        {
            get
            {
                return new InvoiceSetting
                    {
                        Autogenerated = true,
                        Prefix = "INV-",
                        Number = "0000001",
                        Terms = String.Empty,
                        CompanyName = String.Empty,
                        CompanyLogoID = 0,
                        CompanyAddress = String.Empty
                    };
            }
        }

        [DataMember]
        public bool Autogenerated { get; set; }

        [DataMember]
        public String Prefix { get; set; }

        [DataMember]
        public String Number { get; set; }

        [DataMember]
        public String Terms { get; set; }

        [DataMember]
        public String CompanyName { get; set; }

        [DataMember]
        public Int32 CompanyLogoID { get; set; }

        [DataMember]
        public String CompanyAddress { get; set; }
    }


    [Serializable]
    [DataContract]
    public class CRMSettings : ISettings
    {
        [DataMember(Name = "DefaultCurrency")]
        private string defaultCurrency;

        [DataMember]
        public SMTPServerSetting SMTPServerSetting { get; set; }

        [DataMember]
        public InvoiceSetting InvoiceSetting { get; set; }

        [DataMember]
        public Guid WebFormKey { get; set; }

        public Guid ID
        {
            get { return new Guid("fdf39b9a-ec96-4eb7-aeab-63f2c608eada"); }
        }

        public CurrencyInfo DefaultCurrency
        {
            get { return CurrencyProvider.Get(defaultCurrency); }
            set { defaultCurrency = value.Abbreviation; }
        }

        [DataMember(Name = "ChangeContactStatusGroupAuto")]
        public string ChangeContactStatusGroupAutoWrapper { get; set; }

        [IgnoreDataMember]
        public Boolean? ChangeContactStatusGroupAuto
        {
            get { return string.IsNullOrEmpty(ChangeContactStatusGroupAutoWrapper) ? null : (bool?)bool.Parse(ChangeContactStatusGroupAutoWrapper); }
            set { ChangeContactStatusGroupAutoWrapper = value.HasValue ? value.Value.ToString().ToLowerInvariant() : null; }
        }

        [DataMember(Name = "AddTagToContactGroupAuto")]
        public string AddTagToContactGroupAutoWrapper { get; set; }

        [IgnoreDataMember]
        public Boolean? AddTagToContactGroupAuto
        {
            get { return string.IsNullOrEmpty(AddTagToContactGroupAutoWrapper) ? null : (bool?)bool.Parse(AddTagToContactGroupAutoWrapper); }
            set { AddTagToContactGroupAutoWrapper = value.HasValue ? value.Value.ToString().ToLowerInvariant() : null; }
        }

        [DataMember(Name = "WriteMailToHistoryAuto")]
        public Boolean WriteMailToHistoryAuto { get; set; }

        [DataMember(Name = "IsConfiguredPortal")]
        public bool IsConfiguredPortal { get; set; }

        public ISettings GetDefault()
        {
            var languageName = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            var findedCurrency = CurrencyProvider.GetAll().Find(item => String.Compare(item.CultureName, languageName, true) == 0);

            return new CRMSettings
                        {
                            defaultCurrency = findedCurrency != null ? findedCurrency.Abbreviation : "USD",
                            IsConfiguredPortal = false,
                            ChangeContactStatusGroupAuto = null,
                            AddTagToContactGroupAuto = null,
                            WriteMailToHistoryAuto = false,
                            WebFormKey = Guid.Empty,
                            SMTPServerSetting = new SMTPServerSetting(),
                            InvoiceSetting = InvoiceSetting.DefaultSettings
                        };
        }
    }
}