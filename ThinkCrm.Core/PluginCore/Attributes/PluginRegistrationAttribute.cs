//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xrm.Sdk;
//using ThinkPlusPlus.PluginCore.Helper;
//using ThinkPlusPlus.PluginCore.Interfaces;
//using ThinkPlusPlus.PluginCore.Logging;

//namespace ThinkPlusPlus.PluginCore.Attributes
//{

//    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
//    public class PluginRegistrationAttribute : Attribute, IPluginValidator
//    {
//        private readonly MessageType _messageType;
//        private readonly string _primaryEntityLogicalName;
//        private readonly string _secondaryEntityLogicalName;
//        private readonly string[] _filteringAttributes;
//        private readonly string _name;
//        private readonly string _userContext;
//        private readonly int _executionOrder;
//        private readonly PipelineStage _pipelineStage;
//        private readonly ExecutionMode _executionMode;
//        private readonly SupportedDeployment _supportedDeployment;
//        private readonly bool _deleteAsyncOpOnSuccess;
//        private readonly string _preImageName;
//        private readonly string[] _preImageAttributes;
//        private readonly string _postImageName;
//        private readonly string[] _postImageAttributes;
//        private readonly string _keyName;

//        /// <summary>
//        /// Defines a Plugin Step on which this <see cref="CrmPlugin"/> should be registered and executed.
//        /// </summary>
//        /// <param name="keyName"></param>
//        /// <param name="messageType"></param>
//        /// <param name="primaryEntityLogicalName"></param>
//        /// <param name="secondaryEntityLogicalName"></param>
//        /// <param name="filteringAttributes"></param>
//        /// <param name="name"></param>
//        /// <param name="userContext"></param>
//        /// <param name="executionOrder"></param>
//        /// <param name="pipelineStage"></param>
//        /// <param name="executionMode"></param>
//        /// <param name="supportedDeployment"></param>
//        /// <param name="deleteAsyncOpOnSuccess"></param>
//        /// <param name="preImageName"></param>
//        /// <param name="preImageAttributes"></param>
//        /// <param name="postImageName"></param>
//        /// <param name="postImageAttributes"></param>
//        public PluginRegistrationAttribute(string keyName, MessageType messageType, string primaryEntityLogicalName = null,
//            string secondaryEntityLogicalName = null, string[] filteringAttributes = null, string name = null,
//            string userContext = null, int executionOrder = 1,
//            PipelineStage pipelineStage = PipelineStage.Preoperation,
//            ExecutionMode executionMode = ExecutionMode.Synchronous,
//            SupportedDeployment supportedDeployment = SupportedDeployment.ServerOnly,
//            bool deleteAsyncOpOnSuccess = true, string preImageName = null, string[] preImageAttributes = null,
//            string postImageName = null, string[] postImageAttributes = null)
//        {            
//            _messageType = messageType;
//            _primaryEntityLogicalName = primaryEntityLogicalName;
//            _secondaryEntityLogicalName = secondaryEntityLogicalName;
//            _filteringAttributes = filteringAttributes;
//            _name = name;
//            _userContext = userContext;
//            _executionOrder = executionOrder;
//            _pipelineStage = pipelineStage;
//            _executionMode = executionMode;
//            _supportedDeployment = supportedDeployment;
//            _deleteAsyncOpOnSuccess = deleteAsyncOpOnSuccess;
//            _preImageName = preImageName;
//            _preImageAttributes = preImageAttributes;
//            _postImageName = postImageName;
//            _postImageAttributes = postImageAttributes;
//            _keyName = keyName;
//        }

//        private bool MatchesExecutingPlugin(ILogging logging, MessageType messageType, string primaryEntityName, string secondaryEntityName,
//            PipelineStage pipelineStage, ExecutionMode executionMode
//            , bool isExecutingOffline, string preImageName, Entity preImage, string postImageName,
//            Entity postImage)
//        {
//            var internalLogging = new LocalLogger(logging, "PluginRegistrationAttribute");

//            if (_messageType != messageType) return internalLogging.WriteAndReturn(false, "MessageType Not Matched: {0}/{1}", _messageType, messageType);
            
//            if (!string.IsNullOrEmpty(_primaryEntityLogicalName) &&
//                !string.Equals(_primaryEntityLogicalName, primaryEntityName, StringComparison.InvariantCultureIgnoreCase))
//                return internalLogging.WriteAndReturn(false, "PrimaryEntityLogicalName Not Matched: {0}/{1}",
//                    _primaryEntityLogicalName, primaryEntityName);

//            if (!string.IsNullOrEmpty(_secondaryEntityLogicalName) &&
//                !string.Equals(_secondaryEntityLogicalName, secondaryEntityName, StringComparison.InvariantCultureIgnoreCase))
//                return internalLogging.WriteAndReturn(false, "SecondaryEntityLogicalName Not Matched: {0}/{1}",
//                    _secondaryEntityLogicalName, secondaryEntityName);

//            if (_pipelineStage != pipelineStage)
//                return internalLogging.WriteAndReturn(false, "Pipeline Stagge Not Matched: {0}/{1}", _pipelineStage, pipelineStage);

//            if (_executionMode != executionMode)
//                return internalLogging.WriteAndReturn(false, "ExectionMode Not Matched: {0}/{1}", _executionMode, executionMode);

//            if (_supportedDeployment == SupportedDeployment.MicrosoftDynamicsCrmClientForOutlookOnly && !isExecutingOffline)
//                return internalLogging.WriteAndReturn(false,
//                    "Supported Deployment of Offline Only and Plugin is NOT executing Offline: Deployment={0}/ExecutingOffLine={1}",
//                    _supportedDeployment, isExecutingOffline.ToString());

//            if (_supportedDeployment == SupportedDeployment.ServerOnly && isExecutingOffline)
//                return internalLogging.WriteAndReturn(false,
//                    "Supported Deployment of Online Only and Plugin is executing Offline: Deployment={0}/ExecutingOffLine={1}",
//                    _supportedDeployment, isExecutingOffline.ToString());

//            if (!string.IsNullOrEmpty(_preImageName) && _preImageName != preImageName)
//                return internalLogging.WriteAndReturn(false, "Required Pre-Image Not Found as first Image: {0}/{1}",
//                    _preImageName, preImageName);
//            else if (!string.IsNullOrEmpty(_preImageName))
//            {
//                if (_preImageAttributes != null && !ImageIncludes(_preImageAttributes, preImage, internalLogging))
//                    return internalLogging.WriteAndReturn(false, "Failed to validate on Pre-Image Attributes");
//            }


//            if (!string.IsNullOrEmpty(_postImageName) && _postImageName != postImageName)
//                return internalLogging.WriteAndReturn(false, "Required Post-Image Not Found as first Image: {0}/{1}",
//                    _postImageName, postImageName);
//            else if (!string.IsNullOrEmpty((_postImageName)))
//            {
//                if (_postImageAttributes != null && !ImageIncludes(_postImageAttributes, postImage, internalLogging))
//                    return internalLogging.WriteAndReturn(false, "Failed to validate on Post-Image Attributes");
//            }

//            return true;

//        }

//        private static bool ImageIncludes(IEnumerable<string> imageAttributes, Entity imageEntity, ILogging logging)
//        {
//            var internalLogger = new LocalLogger(logging, "PluginRegistrationAttribute");

//            return imageAttributes.All(
//                x => !imageEntity.Contains(x) ? internalLogger.WriteAndReturn(false, "Image does not contain required field: {0}", x) : logging.WriteAndReturn(true, "Image does contain required field: {0}", x))
//                ? internalLogger.WriteAndReturn(true, "Image contained all required fields")
//                : internalLogger.WriteAndReturn(false, "Image does not contain all required fields");
//        }

//        public string KeyName { get { return _keyName; } }

//        public bool Validate(IPluginExecutionContext context, out bool throwException, out string errorMessage)
//        {
//            var sb = new StringBuilder();
//            ILogging sbLogger = new Logger("", true, new LogToStringBuilder(sb));

//            if (this.MatchesExecutingPlugin(sbLogger, context.GetMessage(), context.PrimaryEntityName,
//                context.SecondaryEntityName, context.GetPipelineStage(), context.GetMode(), context.IsExecutingOffline,
//                context.PreEntityImages.Any() ? context.PreEntityImages.First().Key : string.Empty,
//                context.PreEntityImages.Any() ? context.PreEntityImages.First().Value : null,
//                context.PostEntityImages.Any() ? context.PostEntityImages.First().Key : string.Empty,
//                context.PostEntityImages.Any() ? context.PostEntityImages.First().Value : null))
//            {
//                throwException = false;
//                errorMessage = string.Empty;
//                return true;
//            }
//            else
//            {
//                throwException = true;
//                errorMessage = sb.ToString();
//                return false;
//            }


//        }
//    }
//}
