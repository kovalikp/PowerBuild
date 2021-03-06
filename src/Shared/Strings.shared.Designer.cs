﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PowerBuild.Shared {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings_shared {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings_shared() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PowerBuild.Shared.Strings.shared", typeof(Strings_shared).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB4188: Build was canceled..
        /// </summary>
        internal static string BuildAborted {
            get {
                return ResourceManager.GetString("BuildAborted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5022: The MSBuild task host does not support running tasks that perform IBuildEngine callbacks. If you wish to perform these operations, please run your task in the core MSBuild process instead.  A task will automatically execute in the task host if the UsingTask has been attributed with a &quot;Runtime&quot; or &quot;Architecture&quot; value, or the task invocation has been attributed with an &quot;MSBuildRuntime&quot; or &quot;MSBuildArchitecture&quot; value, that does not match the current runtime or architecture of MSBuild..
        /// </summary>
        internal static string BuildEngineCallbacksInTaskHostUnsupported {
            get {
                return ResourceManager.GetString("BuildEngineCallbacksInTaskHostUnsupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Build started..
        /// </summary>
        internal static string BuildStarted {
            get {
                return ResourceManager.GetString("BuildStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB4008: A conflicting assembly for the task assembly &quot;{0}&quot; has been found at &quot;{1}&quot;..
        /// </summary>
        internal static string ConflictingTaskAssembly {
            get {
                return ResourceManager.GetString("ConflictingTaskAssembly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find directory path: {0}.
        /// </summary>
        internal static string DirectoryNotFound {
            get {
                return ResourceManager.GetString("DirectoryNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Event type &quot;{0}&quot; was expected to be serializable using the .NET serializer. The event was not serializable and has been ignored..
        /// </summary>
        internal static string ExpectedEventToBeSerializable {
            get {
                return ResourceManager.GetString("ExpectedEventToBeSerializable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} ({1},{2}).
        /// </summary>
        internal static string FileLocation {
            get {
                return ResourceManager.GetString("FileLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to When attempting to generate a reference assembly path from the path &quot;{0}&quot; and the framework moniker &quot;{1}&quot; there was an error. {2}.
        /// </summary>
        internal static string FrameworkLocationHelper_CouldNotGenerateReferenceAssemblyDirectory {
            get {
                return ResourceManager.GetString("FrameworkLocationHelper.CouldNotGenerateReferenceAssemblyDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .NET Framework version &quot;{0}&quot; is not supported. Please specify a value from the enumeration Microsoft.Build.Utilities.TargetDotNetFrameworkVersion..
        /// </summary>
        internal static string FrameworkLocationHelper_UnsupportedFrameworkVersion {
            get {
                return ResourceManager.GetString("FrameworkLocationHelper.UnsupportedFrameworkVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .NET Framework version &quot;{0}&quot; is not supported when explicitly targeting the Windows SDK, which is only supported on .NET 4.5 and later.  Please specify a value from the enumeration Microsoft.Build.Utilities.TargetDotNetFrameworkVersion that is Version45 or above..
        /// </summary>
        internal static string FrameworkLocationHelper_UnsupportedFrameworkVersionForWindowsSdk {
            get {
                return ResourceManager.GetString("FrameworkLocationHelper.UnsupportedFrameworkVersionForWindowsSdk", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Visual Studio version &quot;{0}&quot; is not supported.  Please specify a value from the enumeration Microsoft.Build.Utilities.VisualStudioVersion..
        /// </summary>
        internal static string FrameworkLocationHelper_UnsupportedVisualStudioVersion {
            get {
                return ResourceManager.GetString("FrameworkLocationHelper.UnsupportedVisualStudioVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB4025: The project file could not be loaded. {0}.
        /// </summary>
        internal static string InvalidProjectFile {
            get {
                return ResourceManager.GetString("InvalidProjectFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB4103: &quot;{0}&quot; is not a valid logger verbosity level..
        /// </summary>
        internal static string InvalidVerbosity {
            get {
                return ResourceManager.GetString("InvalidVerbosity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSBuild is expecting a valid &quot;{0}&quot; object..
        /// </summary>
        internal static string MissingProject {
            get {
                return ResourceManager.GetString("MissingProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Making the following modifications to the environment received from the parent node before applying it to the task host:.
        /// </summary>
        internal static string ModifyingTaskHostEnvironmentHeader {
            get {
                return ResourceManager.GetString("ModifyingTaskHostEnvironmentHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to   Setting &apos;{0}&apos; to &apos;{1}&apos; rather than the parent environment&apos;s value, &apos;{2}&apos;..
        /// </summary>
        internal static string ModifyingTaskHostEnvironmentVariable {
            get {
                return ResourceManager.GetString("ModifyingTaskHostEnvironmentVariable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5016: The name &quot;{0}&quot; contains an invalid character &quot;{1}&quot;..
        /// </summary>
        internal static string NameInvalid {
            get {
                return ResourceManager.GetString("NameInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This collection is read-only..
        /// </summary>
        internal static string OM_NotSupportedReadOnlyCollection {
            get {
                return ResourceManager.GetString("OM_NotSupportedReadOnlyCollection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &quot;{0}&quot; is a reserved item metadata, and cannot be modified or deleted..
        /// </summary>
        internal static string Shared_CannotChangeItemSpecModifiers {
            get {
                return ResourceManager.GetString("Shared.CannotChangeItemSpecModifiers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The string &quot;{0}&quot; cannot be converted to a boolean (true/false) value..
        /// </summary>
        internal static string Shared_CannotConvertStringToBool {
            get {
                return ResourceManager.GetString("Shared.CannotConvertStringToBool", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5024: Could not determine a valid location to MSBuild. Try running this process from the Developer Command Prompt for Visual Studio..
        /// </summary>
        internal static string Shared_CanNotFindValidMSBuildLocation {
            get {
                return ResourceManager.GetString("Shared.CanNotFindValidMSBuildLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5003: Failed to create a temporary file. Temporary files folder is full or its path is incorrect. {0}.
        /// </summary>
        internal static string Shared_FailedCreatingTempFile {
            get {
                return ResourceManager.GetString("Shared.FailedCreatingTempFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5018: Failed to delete the temporary file &quot;{0}&quot;. {1}.
        /// </summary>
        internal static string Shared_FailedDeletingTempFile {
            get {
                return ResourceManager.GetString("Shared.FailedDeletingTempFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The item metadata &quot;%({0})&quot; cannot be applied to the path &quot;{1}&quot;. {2}.
        /// </summary>
        internal static string Shared_InvalidFilespecForTransform {
            get {
                return ResourceManager.GetString("Shared.InvalidFilespecForTransform", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5019: The project file is malformed: &quot;{0}&quot;. {1}.
        /// </summary>
        internal static string Shared_InvalidProjectFile {
            get {
                return ResourceManager.GetString("Shared.InvalidProjectFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5002: The task executable has not completed within the specified limit of {0} milliseconds, terminating..
        /// </summary>
        internal static string Shared_KillingProcess {
            get {
                return ResourceManager.GetString("Shared.KillingProcess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5021: &quot;{0}&quot; and its child processes are being terminated in order to cancel the build..
        /// </summary>
        internal static string Shared_KillingProcessByCancellation {
            get {
                return ResourceManager.GetString("Shared.KillingProcessByCancellation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &quot;{0}&quot; cannot be null..
        /// </summary>
        internal static string Shared_ParameterCannotBeNull {
            get {
                return ResourceManager.GetString("Shared.ParameterCannotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &quot;{0}&quot; cannot have zero length..
        /// </summary>
        internal static string Shared_ParameterCannotHaveZeroLength {
            get {
                return ResourceManager.GetString("Shared.ParameterCannotHaveZeroLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameters &quot;{0}&quot; and &quot;{1}&quot; must have the same number of elements..
        /// </summary>
        internal static string Shared_ParametersMustHaveTheSameLength {
            get {
                return ResourceManager.GetString("Shared.ParametersMustHaveTheSameLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5020: Could not load the project file: &quot;{0}&quot;. {1}.
        /// </summary>
        internal static string Shared_ProjectFileCouldNotBeLoaded {
            get {
                return ResourceManager.GetString("Shared.ProjectFileCouldNotBeLoaded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The resource string &quot;{0}&quot; for the &quot;{1}&quot; task cannot be found. Confirm that the resource name &quot;{0}&quot; is correctly spelled, and the resource exists in the task&apos;s assembly..
        /// </summary>
        internal static string Shared_TaskResourceNotFound {
            get {
                return ResourceManager.GetString("Shared.TaskResourceNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &quot;{0}&quot; task has not registered its resources. In order to use the &quot;TaskLoggingHelper.FormatResourceString()&quot; method this task needs to register its resources either during construction, or via the &quot;TaskResources&quot; property..
        /// </summary>
        internal static string Shared_TaskResourcesNotRegistered {
            get {
                return ResourceManager.GetString("Shared.TaskResourcesNotRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5004: The solution file has two projects named &quot;{0}&quot;..
        /// </summary>
        internal static string SolutionParseDuplicateProject {
            get {
                return ResourceManager.GetString("SolutionParseDuplicateProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5005: Error parsing project section for project &quot;{0}&quot;. The project file name &quot;{1}&quot; contains invalid characters..
        /// </summary>
        internal static string SolutionParseInvalidProjectFileNameCharacters {
            get {
                return ResourceManager.GetString("SolutionParseInvalidProjectFileNameCharacters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5006: Error parsing project section for project &quot;{0}&quot;. The project file name is empty..
        /// </summary>
        internal static string SolutionParseInvalidProjectFileNameEmpty {
            get {
                return ResourceManager.GetString("SolutionParseInvalidProjectFileNameEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5007: Error parsing the project configuration section in solution file. The entry &quot;{0}&quot; is invalid..
        /// </summary>
        internal static string SolutionParseInvalidProjectSolutionConfigurationEntry {
            get {
                return ResourceManager.GetString("SolutionParseInvalidProjectSolutionConfigurationEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5008: Error parsing the solution configuration section in solution file. The entry &quot;{0}&quot; is invalid..
        /// </summary>
        internal static string SolutionParseInvalidSolutionConfigurationEntry {
            get {
                return ResourceManager.GetString("SolutionParseInvalidSolutionConfigurationEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5009: Error parsing the nested project section in solution file..
        /// </summary>
        internal static string SolutionParseNestedProjectError {
            get {
                return ResourceManager.GetString("SolutionParseNestedProjectError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5023: Error parsing the nested project section in solution file. A project with the GUID &quot;{0}&quot; is listed as being nested under project &quot;{1}&quot;, but does not exist in the solution..
        /// </summary>
        internal static string SolutionParseNestedProjectUndefinedError {
            get {
                return ResourceManager.GetString("SolutionParseNestedProjectUndefinedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5010: No file format header found..
        /// </summary>
        internal static string SolutionParseNoHeaderError {
            get {
                return ResourceManager.GetString("SolutionParseNoHeaderError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5011: Parent project GUID not found in &quot;{0}&quot; project dependency section..
        /// </summary>
        internal static string SolutionParseProjectDepGuidError {
            get {
                return ResourceManager.GetString("SolutionParseProjectDepGuidError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5012: Unexpected end-of-file reached inside &quot;{0}&quot; project section..
        /// </summary>
        internal static string SolutionParseProjectEofError {
            get {
                return ResourceManager.GetString("SolutionParseProjectEofError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5013: Error parsing a project section..
        /// </summary>
        internal static string SolutionParseProjectError {
            get {
                return ResourceManager.GetString("SolutionParseProjectError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5014: File format version is not recognized.  MSBuild can only read solution files between versions {0}.0 and {1}.0, inclusive..
        /// </summary>
        internal static string SolutionParseVersionMismatchError {
            get {
                return ResourceManager.GetString("SolutionParseVersionMismatchError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB5015: The properties could not be read from the WebsiteProperties section of the &quot;{0}&quot; project..
        /// </summary>
        internal static string SolutionParseWebProjectPropertiesError {
            get {
                return ResourceManager.GetString("SolutionParseWebProjectPropertiesError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Schema validation.
        /// </summary>
        internal static string SubCategoryForSchemaValidationErrors {
            get {
                return ResourceManager.GetString("SubCategoryForSchemaValidationErrors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Solution file.
        /// </summary>
        internal static string SubCategoryForSolutionParsingErrors {
            get {
                return ResourceManager.GetString("SubCategoryForSolutionParsingErrors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB4077: The &quot;{0}&quot; task has been marked with the attribute LoadInSeparateAppDomain, but does not derive from MarshalByRefObject. Check that the task derives from MarshalByRefObject or AppDomainIsolatedTask..
        /// </summary>
        internal static string TaskNotMarshalByRef {
            get {
                return ResourceManager.GetString("TaskNotMarshalByRef", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You do not have access to: {0}.
        /// </summary>
        internal static string UnauthorizedAccess {
            get {
                return ResourceManager.GetString("UnauthorizedAccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unrecognized solution version &quot;{0}&quot;, attempting to continue..
        /// </summary>
        internal static string UnrecognizedSolutionComment {
            get {
                return ResourceManager.GetString("UnrecognizedSolutionComment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MSB4132: The tools version &quot;{0}&quot; is unrecognized. Available tools versions are {1}..
        /// </summary>
        internal static string UnrecognizedToolsVersion {
            get {
                return ResourceManager.GetString("UnrecognizedToolsVersion", resourceCulture);
            }
        }
    }
}
