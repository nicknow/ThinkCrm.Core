using System;
using FakeItEasy;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.PluginCore.Attributes;
using ThinkCrm.Core.PluginCore.Helper;
using Xunit;

namespace ThinkCrm.Core.Tests.PipelineAttributeTests
{
    public class PipelineMessageAttributeTest
    {
        [Fact]
        public void Test_It_Works_MessageName_When_It_Matches()
        {
            var ctx = A.Fake<IPluginExecutionContext>();

            A.CallTo(() => ctx.MessageName).Returns("Create");

            RunTest(ctx,MessageType.Create,false,true);
        }

        [Fact]
        public void Test_It_Works_MessageName_When_It_DoesNotMatch()
        {
            var ctx = A.Fake<IPluginExecutionContext>();

            A.CallTo(() => ctx.MessageName).Returns("Update");

            RunTest(ctx, MessageType.Create, true, false, "Update");

        }


        [Fact]
        public void Test_That_ThrowException_Is_False_When_It_Shouldnt_Throw_On_A_NonMatch_MessageName()
        {
            var ctx = A.Fake<IPluginExecutionContext>();

            A.CallTo(() => ctx.MessageName).Returns("Update");

            RunTest(ctx,MessageType.Create,false,false,"Create");

        }

        [Fact]
        public void Test_It_Works_Mode_When_It_Matches()
        {
            var ctx = A.Fake<IPluginExecutionContext>();

            A.CallTo(() => ctx.Mode).Returns(1);

            RunTest(ctx, ExecutionMode.Asynchronous, false, true);
        }

        [Fact]
        public void Test_It_Works_Mode_When_It_DoesNotMatch()
        {
            var ctx = A.Fake<IPluginExecutionContext>();

            A.CallTo(() => ctx.Mode).Returns(1);

            RunTest(ctx, ExecutionMode.Synchronous, true, false, "Synchronous");

        }


        private void RunTest(IPluginExecutionContext context, Enum expectedResult, bool throwException, bool assertValidation,
            string errorMessageContains = null)
        {
            var attributeToTest = new PipelineAttribute((dynamic)expectedResult, throwException);

            bool throwExecptionResult;
            string errorMessageResult;
            var result = attributeToTest.Validate(context, out throwExecptionResult, out errorMessageResult);

            Assert.Equal(assertValidation, result);
            Assert.Equal(!assertValidation && throwException, throwExecptionResult);

            if (!string.IsNullOrEmpty(errorMessageContains)) Assert.True(errorMessageResult.Contains(errorMessageContains));
        }
    }
}
