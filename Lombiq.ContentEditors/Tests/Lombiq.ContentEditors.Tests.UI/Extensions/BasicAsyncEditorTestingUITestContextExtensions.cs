using Lombiq.Tests.UI.Extensions;
using OpenQA.Selenium;
using System.Threading.Tasks;

namespace Lombiq.Tests.UI.Services
{
    public static class BasicAsyncEditorTestingUITestContextExtensions
    {
        public static async Task<UITestContext> TestDemoAsyncEditorLoadOnAdminAsync(this UITestContext context)
        {
            await context.GoToRelativeUrlAsync("/Admin/ContentItemAsyncEditor/DemoAsyncEditorProvider/DemoCustomer");

            context.Exists(By.XPath("//label[text()='First Name']"));
            context.Exists(By.XPath("//label[text()='Last Name']"));
            context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(text(), 'Personal Details')]"));
            context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(text(), 'Additional Notes')]"));

            return context;
        }
    }
}
