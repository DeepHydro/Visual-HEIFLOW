﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Heiflow.Controls.WinForm.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("select * from Sites")]
        public string GagingStationSQL {
            get {
                return ((string)(this["GagingStationSQL"]));
            }
            set {
                this["GagingStationSQL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("select * from Sites")]
        public string HOBSQL {
            get {
                return ((string)(this["HOBSQL"]));
            }
            set {
                this["HOBSQL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\Application Extensions\\VHF\\Template\\Lookup_Table.csv")]
        public string LookupTableTemplateFile {
            get {
                return ((string)(this["LookupTableTemplateFile"]));
            }
            set {
                this["LookupTableTemplateFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\Application Extensions\\VHF\\Template\\Zone_Map.csv")]
        public string ZoneMapTemplateFile {
            get {
                return ((string)(this["ZoneMapTemplateFile"]));
            }
            set {
                this["ZoneMapTemplateFile"] = value;
            }
        }
    }
}
