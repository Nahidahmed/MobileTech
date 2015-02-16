using System.Xml.Serialization;
using System;

namespace MobileTech.Models.Entity
{
    public class Setup {
        [XmlElement]
        public long SupPrimaryId { get; set; }
        [XmlElement]
        public string KeyType { get; set; }
        [XmlElement]
        public string KeyValue { get; set; }
        [XmlElement(IsNullable = true)]
        public byte[] Data { get; set; }
    }

    [Serializable]
    public sealed class ActiveDirectorySettings
    {
        private bool _enableActiveDirectory;
        private bool _bypassLogin;
        private string _domainName;
        private string _domainUserName;
        private string _domainPassword;
        private string _ldapServer;
        private string _organizationalUnit;
        private int _pageSize;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }


        public string _SR_SCMUserGroup;
        public string SRSCMUserGroup  
        {
            get { return _SR_SCMUserGroup; }
            set { _SR_SCMUserGroup = value; }
        }
        public string _AE_SM_LMUserGroup;
        public string AESMLMUserGroup
        {
            get { return _AE_SM_LMUserGroup; }
            set { _AE_SM_LMUserGroup = value; }
        }

        public string OrganizationUnit
        {
            get { return _organizationalUnit; }
            set { _organizationalUnit = value; }
        }


        public string LDAPServer
        {
            get { return _ldapServer; }
            set { _ldapServer = value; }
        }

        public bool ActiveDirectoryEnabled
        {
            get { return _enableActiveDirectory; }
            set { _enableActiveDirectory = value; }
        }
        public bool ActiveDirectoryLoginBypass
        {
            get { return _bypassLogin; }
            set { _bypassLogin = value; }
        }
        public string Domain
        {
            get { return _domainName; }
            set { _domainName = value; }
        }
        public string DomainUserName
        {
            get { return _domainUserName; }
            set { _domainUserName = value; }
        }
        public string DomainUserPassword
        {
            get { return _domainPassword; }
            set { _domainPassword = value; }
        }

        public ActiveDirectorySettings()
        {
            this._enableActiveDirectory = false;
            this._bypassLogin = false;
            this._domainName = string.Empty;
            this._domainUserName = string.Empty;
            this._domainPassword = string.Empty;
            this._pageSize = 1000;
        }

    }

    [Serializable]
    public class GlobalSystemSettings
    {
        private ActiveDirectorySettings _activeDirectorySettings;
        public ActiveDirectorySettings ActiveDirectory
        {
            get { return _activeDirectorySettings; }
            set { _activeDirectorySettings = value; }
        }

        public GlobalSystemSettings()
        {
            _activeDirectorySettings = new ActiveDirectorySettings();
        }

        public GlobalSystemSettings(ActiveDirectorySettings activeDirectorySettings)
        {
            _activeDirectorySettings = activeDirectorySettings;
        }

    }

}