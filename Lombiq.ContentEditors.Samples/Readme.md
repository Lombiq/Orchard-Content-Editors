# Lombiq Content Editors for Orchard Core - Samples

## About

Example Orchard Core module that teaches you how to use the Lombiq Content Editors for Orchard Core module.

For general details about and usage instructions see the [root Readme](../Readme.md).

This sample project demonstrates:
- How to create an async editor provider for a content type. Once you've enabled the module you can access the demo editor on this URL: `/Admin/ContentItemAsyncEditor/Employee`.
- How to create an async editor that can be used on the front-end instead of the Admin UI. Once you've enabled the module you can access the demo editor on this URL: `/FrontEndDemoContentItemAsyncEditor`.

For detailed instructions please check out the training sections below.

## Training sections

You can start with any of these sections, they demonstrate different approaches that best fit different use-cases.

- [Content Item async editor provider](Services/EmployeeAsyncEditorProvider.cs) - Example async editor provider for Employee content items.
- [Front-end async editor](Controllers/FrontEndDemoContentItemAsyncEditorController.cs) - Example async editor displayed on the front-end instead of the Admin UI.
