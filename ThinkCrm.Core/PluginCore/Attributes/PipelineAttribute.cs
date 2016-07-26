using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;
using ThinkCrm.Core.PluginCore.Helper;

namespace ThinkCrm.Core.PluginCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PipelineAttribute : Attribute, IPluginValidator
    {
        private readonly MessageType _messageType;
        private readonly PipelineStage _pipelineStage;
        private readonly IsolationMode _isolationMode;
        private readonly bool _throwException;

        private readonly bool _checkExecutionMode = false;
        private readonly bool _checkIsolationMode = false;
        private readonly bool _checkPipelineStage = false;
        private readonly bool _checkMessage = false;

        private readonly ExecutionMode _executionMode;

        public PipelineAttribute(ExecutionMode executionMode, bool throwException = true)
        {
            if (!Enum.IsDefined(typeof(ExecutionMode), executionMode))
                throw new InvalidEnumArgumentException(nameof(executionMode), (int) executionMode, typeof(ExecutionMode));
            _executionMode = executionMode;
            _throwException = throwException;

            _checkExecutionMode = true;
        }

        public PipelineAttribute(IsolationMode isolationMode, bool throwException = true)
        {
            if (!Enum.IsDefined(typeof(IsolationMode), isolationMode))
                throw new InvalidEnumArgumentException(nameof(isolationMode), (int) isolationMode, typeof(IsolationMode));
            _isolationMode = isolationMode;
            _throwException = throwException;

            _checkIsolationMode = true;
        }

        public PipelineAttribute(PipelineStage pipelineStage, bool throwException = true)
        {
            if (!Enum.IsDefined(typeof(PipelineStage), pipelineStage))
                throw new InvalidEnumArgumentException(nameof(pipelineStage), (int) pipelineStage, typeof(PipelineStage));
            _pipelineStage = pipelineStage;
            _throwException = throwException;

            _checkPipelineStage = true;
        }

        public PipelineAttribute(MessageType messageType, bool throwException = true)
        {
            if (!Enum.IsDefined(typeof(MessageType), messageType))
                throw new InvalidEnumArgumentException(nameof(messageType), (int) messageType, typeof(MessageType));
            _messageType = messageType;
            _throwException = throwException;

            _checkMessage = true;

        }

        public bool Validate(IPluginExecutionContext context, out bool throwException, out string errorMessage)
        {
            return HandleCheck(GetExpectedValue(), GetActualValue(context), out throwException, out errorMessage);
        }

        private Enum GetExpectedValue()
        {
            if (_checkExecutionMode) return _executionMode;
            if (_checkIsolationMode) return _isolationMode;
            if (_checkMessage) return _messageType;
            if (_checkPipelineStage) return _pipelineStage;

            // This should only happen if there is a coding error.
            throw new Exception("Unexpected result during Pipeline validation. Could not identify the desired check type.");
        }

        private Enum GetActualValue(IPluginExecutionContext context)
        {
            if (_checkExecutionMode) return context.GetMode();
            if (_checkIsolationMode) return context.GetIsolationMode();
            if (_checkMessage) return context.GetMessage();
            if (_checkPipelineStage) return context.GetPipelineStage();

            // This should only happen if there is a coding error.
            throw new Exception("Unexpected result during Pipeline validation. Could not identify the desired check type.");
        }

        private bool HandleCheck(Enum expectedEnum, Enum actualEnum, out bool throwException, out string errorMessage)
        {
            if (expectedEnum == null) throw new ArgumentNullException(nameof(expectedEnum));
            if (actualEnum == null) throw new ArgumentNullException(nameof(actualEnum));

            if (Enum.Equals(expectedEnum, actualEnum))
            {
                throwException = false;
                errorMessage = string.Empty;
                return true;
            }
            else
            {
                throwException = _throwException;
                errorMessage =
                    $"Pipeline Validation failed for {expectedEnum.GetType()}. Expected={expectedEnum} / Actual={actualEnum}.";
                return false;
            }
        }
    }
}
