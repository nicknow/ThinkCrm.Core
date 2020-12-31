using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;
using ThinkCrm.Core.PluginCore.Helper;

namespace ThinkCrm.Core.PluginCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequiredImageAttribute : Attribute, IPluginValidator
    {
        private readonly ImageType _imageType;
        private readonly string _imageName;
        private readonly string _logicalName;
        private readonly bool _throwException;
        private readonly bool _skipOnCreate;
        private readonly string[] _requiredAttributes;

        public RequiredImageAttribute(ImageType imageType, string imageName, string logicalName = null,
            bool throwException = true, bool skipOnCreate = true, params string[] requiredAttributes)
        {
            if (logicalName == null) throw new ArgumentNullException(nameof(logicalName));
            if (string.IsNullOrEmpty(imageName)) throw new ArgumentNullException(nameof(imageName));
            if (!Enum.IsDefined(typeof(ImageType), imageType))
                throw new InvalidEnumArgumentException(nameof(imageType), (int) imageType, typeof(ImageType));
            _imageType = imageType;
            _imageName = imageName;
            _logicalName = logicalName;
            _throwException = throwException;
            _skipOnCreate = skipOnCreate;
            _requiredAttributes = requiredAttributes;
        }

        public RequiredImageAttribute(ImageType imageType, string imageName, string logicalName = null,
           bool throwException = true, params string[] requiredAttributes) : this(imageType, imageName, logicalName, throwException, true, requiredAttributes)
        { }

        public bool Validate(IPluginExecutionContext context, out bool throwException, out string errorMessage)
        {
            if (context.GetMessage() == MessageType.Create && _skipOnCreate)
            {
                throwException = false;
                errorMessage = string.Empty;
                return true;
            };

            var imageCollection = _imageType == ImageType.PreImage ? context.PreEntityImages : context.PostEntityImages;

            if (!imageCollection.Contains(_imageName))
            {
                throwException = _throwException;
                errorMessage = $"Required {GetImageTypeName()} with a key of {_imageName} is missing.";
                return false;
            }

            var imageEntity = imageCollection[_imageName];

            if (!string.IsNullOrEmpty(_logicalName) && !imageEntity.LogicalName.Equals(_logicalName))
            {
                throwException = _throwException;
                errorMessage =
                    $"Required P{GetImageTypeName()} named {_imageName} is has logical name mismatch. Expected: {_logicalName} / Actual: {imageEntity.LogicalName}";
                return false;
            }

            if (_requiredAttributes.Any(x => !imageEntity.Contains(x)))
            {
                var missingList = _requiredAttributes.Where(x => !imageEntity.Contains(x)).ToList();

                throwException = _throwException;
                errorMessage =
                    $"Required P{GetImageTypeName()} named {_imageName} is missing required attributes: {string.Join(", ", missingList)}.";
                return false;
            }

            throwException = false;
            errorMessage = string.Empty;
            return true;

        }

        private string GetImageTypeName() => _imageType == ImageType.PreImage ? "PreImage" : "Post Image";
    }

    public enum ImageType
    {
        PreImage,
        PostImage
    }
}
