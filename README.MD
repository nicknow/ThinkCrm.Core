# ThinkCrm Core - Simplified Plugin Development

# Introduction

ThinkCrm.Core is a DLL to simplify the creation and execution of Dynamics CRM Plugins. At its core it is designed to eliminate plumbing code and automate validation execution. This simplifies development reducing the opportunity for error and helps to support code-based testing.

This project builds on a blog article (http://nicknow.net/dynamics-crm-2011-abstracting-plugin-setup) that I wrote several years ago. This project greatly expands on the ideas in that article and provide a more robust and configurable set of capabilities.

I have a demonstration of how I use this solution in a fully testable state ~~w/ ILMerge~~ and dependency injection (for testability): https://github.com/nicknow/BloggingCrm-Dynamics-Crm-Plugin-Unit-Testing-Example/

# Update on ILMerge and Nuget

I've moved away from using ILMerge for Plugins (and custom activities) on the Microsoft Dataverse platform. I had previously written a [blog article](https://nicknow.net/dynamics-crm-ilmerge-dll-plugin/) in February 2015 on how to use ILMerge for plugin development (it remains one of my more popular posts.) But in August of 2015 a [Microsoft post](https://cloudblogs.microsoft.com/dynamics365/no-audience/2010/11/09/how-to-reference-assemblies-from-plug-ins/) was updated to say that ILMerge was unsupported.

I'm not sure I agree with Microsoft that this is *unsupported* so much as just something we don't promote. There isn't really a good reason it would **not** be supported. But that's opinion and we deal with actual code that needs to be run in a supported manner. To that end, I've rearranged this code base to support a Shared Project approach, so the code becomes *part* of the actual project and all compiles to the same DLL.

Thus, there is now a *ThinkCrm.Shared.Core* project that can be added as a reference to your Plugin project in Visual Studio. Once added as a reference it will act as a reference but when building the code will be compiled into the same DLL as your Plugin project, this ultimately allows you to deploy a single plugin - with no ILMerge - to the Dataverse service.

Within the *ThinkCrm.Core* solution there are two additional projects. The *ThinkCrm.Core* project compiles the DLLs from the shared project. This could be used if you still wanted to use ILMerge or want to use them in non-plugin code (for example, to take advantage of the ICrmService/CrmService wrapper around IOrganizationService.) There is also a *ThinkCrm.Core.Tests* project to provide a structure for implementing coded tests.

![add_shared_project](https://i.imgur.com/fZxQ2Zu.png)



# Status

ThinkCrm.Core currently is an in-development project. It needs a significant amounts of testing and generation of test cases to be considered production releasable as a v1.0. I welcome your pull requests to help move this project to that stage.

This is much more a proof-of-concept than a ready-to-go enterprise stable product. If you intend to use this for production applications I recommend being prepared to write tests and implement fixes (please think of submitting them) and please create issues so we can track and fix bugs as they are found.

# Quick Get Started

```cs
    [TargetEntity(true, CrmObject.Account.EntityLogicalName)]
    [PreImageValidator(ImageName, CrmObject.Account.EntityLogicalName, true, CrmObject.Account.Fields.think_frozen)]
    public class FrozenFlagChanged : CorePlugin
    {
        private const string ImageName = "PreImage";

        /// <summary>
        /// This constructor will skip configuration of the ObjectProviderService.
        /// Call this from a derived class being used as a testing harness 
        /// </summary>
        /// <param name="switcher">value does not matter</param>
        protected FrozenFlagChanged(bool switcher)
        {

        }

        public FrozenFlagChanged()
        {
            ObjectProviderService.RegisterType<IFrozenAccountManagement>(new FrozenAccountManagement());
        }

        protected override bool ExecutePostOpSync(IPluginSetup p)
        {
            if (!ObjectProviderService.Contains<IFrozenAccountManagement>()) throw new NullReferenceException(nameof(IFrozenAccountManagement));

            var l = new LocalLogger(p.Logging, ClassName);            

            var target = p.Helper.GetTargetEntity<CrmObject.Account>();

            if (!target.Contains(CrmObject.Account.Fields.think_frozen))
                return l.WriteAndReturn(false, $"Attribute not in target: {CrmObject.Account.Fields.think_frozen}.");

            var preImage = p.Context.PreEntityImages[ImageName].ToEntity<CrmObject.Account>();            

            return l.WriteAndReturn(false, "Execution Completed.");
        }

    }
````

~~# ILMerge~~

~~In order to incorporate ThinkCrm.Core into your plugins you must use ILMerge. I recommend using a NuGet package, MSBuild.ILMerge.Task, to merge the DLLs. I have a blog article (http://nicknow.net/dynamics-crm-ilmerge-dll-plugin/) that describes how to implement this for CRM plugins.~~

We don't use ILMerge any longer, see [Update on ILMerge and Nuget](#update-on-ilmerge-and-nuget).

# License

This solution and source code is licensed under the MIT License (see LICENSE.MD)

