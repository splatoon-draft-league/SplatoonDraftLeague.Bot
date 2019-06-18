﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SquidDraftLeague.Language.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SquidDraftLeague.Language.Resources.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You currently are banned from participating in SDL. Time remaining for your ban: .
        /// </summary>
        public static string BanMessage {
            get {
                return ResourceManager.GetString("BanMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find user with this name. Make sure you use the format `&quot;Username#Discrimator&quot;` e.g. `&quot;DeltaJordan#5497&quot;` and wrap it in quotes to capture any spaces..
        /// </summary>
        public static string InvalidReportNameResolve {
            get {
                return ResourceManager.GetString("InvalidReportNameResolve", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please use this format: `&quot;Username#Discrimator&quot;` e.g. `&quot;DeltaJordan#5497&quot;` for reporting a user. Make sure to wrap it in quotes if it has spaces in the name..
        /// </summary>
        public static string InvalidReportNameSplit {
            get {
                return ResourceManager.GetString("InvalidReportNameSplit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reporting requires an explaination after the user. Example command: `%report &quot;DeltaJordan#5497&quot; Called me a crayon eater.`.
        /// </summary>
        public static string InvalidReportNoReason {
            get {
                return ResourceManager.GetString("InvalidReportNoReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You cannot join a lobby if you are already in one!.
        /// </summary>
        public static string JoinLobbyInLobby {
            get {
                return ResourceManager.GetString("JoinLobbyInLobby", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are already in a set, there is no need to join a lobby..
        /// </summary>
        public static string JoinLobbyInSet {
            get {
                return ResourceManager.GetString("JoinLobbyInSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are already the max amount of lobbies! Please wait until another lobby either enters your power level threshold or there are less queuing lobbies..
        /// </summary>
        public static string LobbiesFull {
            get {
                return ResourceManager.GetString("LobbiesFull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are not in a lobby or this is not a set channel!.
        /// </summary>
        public static string NotInLobby {
            get {
                return ResourceManager.GetString("NotInLobby", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thank you for your interest in SDL! All responses to my questions will be pulled from the next message sent by you. To begin, please state your timezone..
        /// </summary>
        public static string RegistrationBegin {
            get {
                return ResourceManager.GetString("RegistrationBegin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your registration has been sent to staff and will be processed. If we require any more info we will get in contact with you. Otherwise, this is all you have to do for now other than wait for our opening date, June 23rd. Thank you for your registration!.
        /// </summary>
        public static string RegistrationComplete {
            get {
                return ResourceManager.GetString("RegistrationComplete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Next up, what would you like your nickname for standings to be? E.g. your in-game name in Splatoon..
        /// </summary>
        public static string RegistrationNickname {
            get {
                return ResourceManager.GetString("RegistrationNickname", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please wait while we process your request. If you don&apos;t get a reply from this bot for more than 10 minutes, please get in contact with SDL staff..
        /// </summary>
        public static string RegistrationProcessing {
            get {
                return ResourceManager.GetString("RegistrationProcessing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Finally, please **directly upload** a screenshot of your solo queue powers from the last month. **DO NOT** link it as this may cause issues. Be sure to upload the correct image as well as **you may not retry this response**. You can, however, still retry the last response if needed. If you do not wish to upload a screenshot reply &quot;No&quot;. Please note that this most likely cause you to be placed in the lowest seeding class, mainly depending on the team experience you listed in the last question..
        /// </summary>
        public static string RegistrationScreenshot {
            get {
                return ResourceManager.GetString("RegistrationScreenshot", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Now we will need you to list any competitive teams you have been on within the last 3 months. If there are none, just reply &quot;None&quot; without the quotes..
        /// </summary>
        public static string RegistrationTeams {
            get {
                return ResourceManager.GetString("RegistrationTeams", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It&apos;s been 10 minutes since you last responded. You will have to try again later..
        /// </summary>
        public static string RegistrationTimeout {
            get {
                return ResourceManager.GetString("RegistrationTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command is to be used to report discrepancies during a set. If you want to report a player, use `%report name` in DMs with this bot..
        /// </summary>
        public static string ReportErrorResponse {
            get {
                return ResourceManager.GetString("ReportErrorResponse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Moderators have been notified of this report and will get back to you if more info is needed..
        /// </summary>
        public static string ReportResponse {
            get {
                return ResourceManager.GetString("ReportResponse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sit tight in {0}! There are too many sets in progress right now. Once a set finishes you will be notified..
        /// </summary>
        public static string SetsFull {
            get {
                return ResourceManager.GetString("SetsFull", resourceCulture);
            }
        }
    }
}
