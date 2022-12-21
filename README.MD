# ThinkCrm Core - Simplified Plugin Development

> ** Warning **
THIS IS NOT PRODUCTION READY. DO NOT USE IN A PRODUCTION SYSTEM

That is your only warning. This exists as conceptual and for references purposes at this point. I wouldn't approach this the same today. I don't use it in production solutions. But it does serve as a model of implementing a plugin plumbing/validation wrapper and distributing it as a NuGet package. I started working on this original concept in August 2012 and it has evolved over the years but also much of the evolution has occurred on non-public projects. Bottom line: don't just drop this into your 2022+ solution and think you are doing a good thing.

:warning: __YOU HAVE BEEN WARNED__

***

# Introduction

ThinkCrm.Core is a DLL to simplify the creation and execution of Microsoft Dataverse (Dynamics 365) Plugins. At its core it is designed to eliminate plumbing code and automate validation execution. This simplifies development reducing the opportunity for error and helps to support code-based testing.

This project builds on a blog article (http://nicknow.net/dynamics-crm-2011-abstracting-plugin-setup) that I wrote back in 2012. This project greatly expands on the ideas in that article and provide a more robust and configurable set of capabilities.

I have a demonstration of how I use this solution in a fully testable state ~~w/ ILMerge~~ and dependency injection (for testability): https://github.com/nicknow/BloggingCrm-Dynamics-Crm-Plugin-Unit-Testing-Example/

# Update on ILMerge and NuGet

This package went from originally intended to be used with ILMerge to being a Shared Project, because ILMerge was officially unsupported per Microsoft, and now it is back to being a NuGet package intended for use with the dependent assembly capability in Microsoft Dataverse. 

# Status

ThinkCrm.Core currently is an in-development project.

This is much more a proof-of-concept than a ready-to-go enterprise stable product. If you intend to use this for production applications I recommend being prepared to write tests and implement fixes (please think of submitting them) and please create issues so we can track and fix bugs as they are found.

It needs a significant amounts of testing and generation of test cases to be considered production releasable as a v1.0. I highly doubt this project will ever get there. I may create and release a completely a new solution that will be geared to production use (but that will depend on how much time I get and what else looks like fun.)

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

