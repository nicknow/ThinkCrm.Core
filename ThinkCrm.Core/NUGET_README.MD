# ThinkCrm Core - Simplified Plugin Development

> ** Warning **
THIS IS NOT PRODUCTION READY. DO NOT USE IN A PRODUCTION SYSTEM

ThinkCrm.Core is a DLL to simplify the creation and execution of Microsoft Dataverse (Dynamics 365) Plugins. At its core it is designed to eliminate plumbing code and automate validation execution. This simplifies development reducing the opportunity for error and helps to support code-based testing.

This exists as conceptual and for references purposes at this point. I wouldn't approach this the same today. I don't use it in production solutions. But it does serve as a model of implementing a plugin plumbing/validation wrapper and distributing it as a NuGet package. I started working on this original concept in August 2012 and it has evolved over the years but also much of the evolution has occurred on non-public projects. Bottom line: don't just drop this into your 2022+ solution and think you are doing a good thing.