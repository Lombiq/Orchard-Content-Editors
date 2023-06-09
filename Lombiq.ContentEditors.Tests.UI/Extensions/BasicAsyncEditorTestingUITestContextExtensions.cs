using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Tests.UI.Extensions;

public static class BasicAsyncEditorTestingUITestContextExtensions
{
    public static Task EnableContentEditorsSamplesFeatureAsync(this UITestContext context) =>
        context.EnableFeatureDirectlyAsync("Lombiq.ContentEditors.Samples");

    public static async Task<UITestContext> TestDemoAsyncEditorLoadOnAdminAsync(this UITestContext context)
    {
        await context.GoToRelativeUrlAsync("/Admin/ContentItemAsyncEditor/EmployeeAsyncEditorProvider/Employee");

        context.Exists(By.XPath("//label[. = 'Name']"));
        context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(., 'Personal Details')]"));
        context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(., 'Employment Details')]"));

        await context.FillInWithRetriesAsync(By.Id("AsyncEditorEmployeePart_Name_Text"), "John Doe");
        await context.ClickReliablyOnAsync(By.ClassName("asyncEditor__saveAndNextAction"));
        await context.FillInWithRetriesAsync(By.Id("AsyncEditorEmployeePart_Position_Text"), "CEO");
        await context.FillInWithRetriesAsync(By.Id("AsyncEditorEmployeePart_Office_Text"), "Budapest");
        await context.ClickReliablyOnAsync(By.ClassName("asyncEditor__submitAction"));
        context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__message') and contains(., 'Editor has been successfully submitted.')]"));

        return context;
    }
}
