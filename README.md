Samples
=======

All of these samples (except SimpleLti) are based on the Visual Studio C# ASP.NET Web Application template.

Consumer
--------
Full blown LTI Tool Consumer with users, classes, and assignments. Supports Launch, Outcomes, and Content-Item Message.

ConsumerCertification
---------------------
Purpose built LTI Tool Consumer that passes all the 1.0, 1.1, 1.1.1, 1.2, and Outcomes-1 certification tests. See the
Lti1Controller for the launch code, ProfileController for the ToolConsumerProfile, and OutcomesController for Outcomes 1.0.

Publisher
---------
Full blown LTI Tool Provider with users and consumers. Supports Launch, Outcomes, and Content-Item Message.

SimpleLti
---------
A single app that performs as both a TC and a TP. Has both MVC and WebForms pages.

Getting Started
===============

1. Download the LtiSamples repository (this repository).
2. Download the [LtiLibrary](https://github.com/andyfmiller/LtiLibrary) repository.
3. Open LtiSamples solution in Visual Studio (I use the free Visual Studio 2015 Community Edition).
4. Make sure all the LtiLibrary projects are loading. If not, then remove them from the solution and add them from wherever you put the repository.
5. Build the solution and fix any broken references (most likely because you don't have a required library).
6. Set the Startup Project to SimpleLti and run the solution.

