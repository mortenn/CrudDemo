# What the crud?
This is a technology demo of using source generators to automatically stand up a basic CRUD API for Entity Framework

## How to crud?
Just create a standard EF(Core) entity to the Models project, build, add a migration, update database and run

## How does it crud?
There are two source generators, reading attributes on model objects to know what to do.

The DbContextGenerator will create a DbContext in whatever project it is included in,
taking the name of the DbContext from the AutoCrud attribute on the model class.

The APIGenerator will create an extension method in whatever project it is included in,
containing all the .Map-calls on endpoints, which you simply use in your startup.
The BaseURL to the API is defined in the AutoCrud attribute on the model class.
The source generators contain scriban templates.
For the best experience changing these, use VS Code with the [Scriban extension](https://marketplace.visualstudio.com/items?itemName=xoofx.scriban) installed to edit the .sbncs file

For a deep dive into using source generators, refer to [the blog post introducing the feature](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/)

