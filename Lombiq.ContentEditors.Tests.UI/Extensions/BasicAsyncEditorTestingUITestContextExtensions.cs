using Atata;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using Shouldly;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Tests.UI.Extensions;

public static class BasicAsyncEditorTestingUITestContextExtensions
{
    public static Task EnableContentEditorsSamplesFeatureAsync(this UITestContext context) =>
        context.EnableFeatureDirectlyAsync("Lombiq.ContentEditors.Samples");

    public static async Task<UITestContext> TestDemoContentItemAsyncEditorAsync(this UITestContext context)
    {
        await context.GoToRelativeUrlAsync("/Admin/ContentItemAsyncEditor/EmployeeAsyncEditorProvider/Employee");

        context.Exists(By.XPath("//label[. = 'Name']"));
        context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(., 'Personal Details')]"));
        context.Exists(
            By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(., 'Employment Details')]"));

        await context.FillInWithRetriesAsync(By.Id("AsyncEditorEmployeePart_Name_Text"), "John Doe");
        await context.ClickReliablyOnAsync(By.ClassName("asyncEditor__saveAndNextAction"));
        await context.FillInWithRetriesAsync(By.Id("AsyncEditorEmployeePart_Position_Text"), "CEO");
        await context.FillInWithRetriesAsync(By.Id("AsyncEditorEmployeePart_Office_Text"), "Budapest");
        await context.ClickReliablyOnAsync(By.ClassName("asyncEditor__submitAction"));

        context.Exists(By.ClassName("asyncEditor__messages"));
        var errorMessage = context.Get(By.ClassName("asyncEditor__error").Safely());
        errorMessage?.Text?.ShouldBeNullOrWhiteSpace(errorMessage.GetAttribute("data-error-json"));
        context.Get(By.ClassName("asyncEditor__message")).Text.Trim().ShouldBe("Editor has been successfully submitted.");

        return context;
    }

    public static async Task<UITestContext> TestDemoFrontEndAsyncEditorAsync(this UITestContext context)
    {
        await context.GoToRelativeUrlAsync("/FrontEndContentItemAsyncEditor");

        context.Exists(By.XPath("//label[. = 'Name']"));
        context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(., 'Reporter')]"));
        context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(., 'Details')]"));

        await context.FillInWithRetriesAsync(By.Id("SupportTicketPart_Name"), "John Doe");
        await context.FillInWithRetriesAsync(By.Id("SupportTicketPart_Email"), "john@test.com");
        await context.ClickReliablyOnAsync(By.ClassName("asyncEditor__saveAndNextAction"));
        await context.FillInWithRetriesAsync(By.Id("SupportTicketPart_Url"), "https://test.com");
        await context.FillInWithRetriesAsync(By.Id("SupportTicketPart_Description"), "Lorem ipsum.");
        await context.ClickReliablyOnAsync(By.ClassName("asyncEditor__submitAction"));
        context.Exists(
            By.XPath(
                "//*[contains(@class, 'asyncEditor__message') and contains(., 'Editor has been successfully submitted.')]"));

        return context;
    }
}
