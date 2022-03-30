using System.Threading.Tasks;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;

namespace Lombiq.ContentEditors.Tests.UI.Extensions;

public static class BasicAsyncEditorTestingUITestContextExtensions
{
    public static Task EnableContentEditorsSamplesFeatureAsync(this UITestContext context) =>
        context.EnableFeatureDirectlyAsync("Lombiq.ContentEditors.Samples");

    public static async Task<UITestContext> TestDemoAsyncEditorLoadOnAdminAsync(this UITestContext context)
    {
        await context.GoToRelativeUrlAsync("/Admin/ContentItemAsyncEditor/EmployeeAsyncEditorProvider/Employee");

        context.Exists(By.XPath("//label[text()='Name']"));
        context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(text(), 'Personal Details')]"));
        context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(text(), 'Employment Details')]"));

        return context;
    }
}