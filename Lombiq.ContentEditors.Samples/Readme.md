# Lombiq Content Editors for Orchard Core - Samples

## About

Example Orchard Core module that teaches you how to use the Lombiq Content Editors for Orchard Core module.

For general details about and usage instructions see the [root Readme](../Readme.md).

This sample project demonstrates:

- How to create an async editor provider for a content type. If you are using the OSOCE application you can try it out on the front-end by clicking on the "Content Editor Samples" and "Employee (admin)" menu items. It will redirect you to the Admin UI where you can click through an async editor for the Employee content type that contains two editor pages. You can also access the demo editor on this URL: _/Admin/ContentTypes/Employee/ContentItemAsyncEditor_.
- How to create an async editor that can be used on the front-end instead of the Admin UI. You can try it out by clicking on the "Content Editor Samples" and "Support Ticket (front-end)" menu items. It will redirect you to an editor where you can click through an async editor for the Support Ticket content type that contains two editor pages. Instead of using the Admin UI, it uses a custom front-end editor that is initialized by a custom controller (`FrontEndDemoContentItemAsyncEditorController`). You can also access the demo editor on this URL: _/FrontEndDemoContentItemAsyncEditor_.

Note, that both of these example editors are accessible with the respective EditContent permission only. For the sake of simplicity, just log in with the site owner account to try them out.

For detailed instructions please check out the training sections below.

## Training sections

You can start with any of these sections, they demonstrate different approaches that best fit different use-cases.

- [Content Item async editor provider](Services/EmployeeAsyncEditorProvider.cs) - Example async editor provider for Employee content items.
- [Front-end async editor](Controllers/FrontEndDemoContentItemAsyncEditorController.cs) - Example async editor displayed on the front-end instead of the Admin UI.
