﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.18444
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace OBShopWeb.EntranceService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Result", Namespace="http://schemas.datacontract.org/2004/07/OrangeBear.Entrance")]
    [System.SerializableAttribute()]
    public partial struct Result : System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AccountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<OBShopWeb.EntranceService.Authority> AuthoritysField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FullnameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsAccountLoginField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private OBShopWeb.EntranceService.ResultType ResultStatusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ZoneField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Account {
            get {
                return this.AccountField;
            }
            set {
                if ((object.ReferenceEquals(this.AccountField, value) != true)) {
                    this.AccountField = value;
                    this.RaisePropertyChanged("Account");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<OBShopWeb.EntranceService.Authority> Authoritys {
            get {
                return this.AuthoritysField;
            }
            set {
                if ((object.ReferenceEquals(this.AuthoritysField, value) != true)) {
                    this.AuthoritysField = value;
                    this.RaisePropertyChanged("Authoritys");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Fullname {
            get {
                return this.FullnameField;
            }
            set {
                if ((object.ReferenceEquals(this.FullnameField, value) != true)) {
                    this.FullnameField = value;
                    this.RaisePropertyChanged("Fullname");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsAccountLogin {
            get {
                return this.IsAccountLoginField;
            }
            set {
                if ((this.IsAccountLoginField.Equals(value) != true)) {
                    this.IsAccountLoginField = value;
                    this.RaisePropertyChanged("IsAccountLogin");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public OBShopWeb.EntranceService.ResultType ResultStatus {
            get {
                return this.ResultStatusField;
            }
            set {
                if ((this.ResultStatusField.Equals(value) != true)) {
                    this.ResultStatusField = value;
                    this.RaisePropertyChanged("ResultStatus");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Zone {
            get {
                return this.ZoneField;
            }
            set {
                if ((object.ReferenceEquals(this.ZoneField, value) != true)) {
                    this.ZoneField = value;
                    this.RaisePropertyChanged("Zone");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Authority", Namespace="http://schemas.datacontract.org/2004/07/OrangeBear.Security")]
    [System.SerializableAttribute()]
    public partial class Authority : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ActiveField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IndexField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MemoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TitleField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TypeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Active {
            get {
                return this.ActiveField;
            }
            set {
                if ((this.ActiveField.Equals(value) != true)) {
                    this.ActiveField = value;
                    this.RaisePropertyChanged("Active");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Index {
            get {
                return this.IndexField;
            }
            set {
                if ((object.ReferenceEquals(this.IndexField, value) != true)) {
                    this.IndexField = value;
                    this.RaisePropertyChanged("Index");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Memo {
            get {
                return this.MemoField;
            }
            set {
                if ((object.ReferenceEquals(this.MemoField, value) != true)) {
                    this.MemoField = value;
                    this.RaisePropertyChanged("Memo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title {
            get {
                return this.TitleField;
            }
            set {
                if ((object.ReferenceEquals(this.TitleField, value) != true)) {
                    this.TitleField = value;
                    this.RaisePropertyChanged("Title");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Type {
            get {
                return this.TypeField;
            }
            set {
                if ((this.TypeField.Equals(value) != true)) {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ResultType", Namespace="http://schemas.datacontract.org/2004/07/OrangeBear.Entrance")]
    public enum ResultType : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Failure = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        AuthError = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="EntranceService.IEntrance")]
    public interface IEntrance {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEntrance/LogIn", ReplyAction="http://tempuri.org/IEntrance/LogInResponse")]
        OBShopWeb.EntranceService.Result LogIn(string account, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEntrance/EmployeeCode", ReplyAction="http://tempuri.org/IEntrance/EmployeeCodeResponse")]
        OBShopWeb.EntranceService.Result EmployeeCode(string barcode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEntrance/ErrorMsg", ReplyAction="http://tempuri.org/IEntrance/ErrorMsgResponse")]
        string ErrorMsg();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IEntranceChannel : OBShopWeb.EntranceService.IEntrance, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class EntranceClient : System.ServiceModel.ClientBase<OBShopWeb.EntranceService.IEntrance>, OBShopWeb.EntranceService.IEntrance {
        
        public EntranceClient() {
        }
        
        public EntranceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public EntranceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public EntranceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public EntranceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public OBShopWeb.EntranceService.Result LogIn(string account, string password) {
            return base.Channel.LogIn(account, password);
        }
        
        public OBShopWeb.EntranceService.Result EmployeeCode(string barcode) {
            return base.Channel.EmployeeCode(barcode);
        }
        
        public string ErrorMsg() {
            return base.Channel.ErrorMsg();
        }
    }
}
