using Lombiq.Tests.UI.Extensions;
using OpenQA.Selenium;

namespace Lombiq.Tests.UI.Services
{
    public static class BasicAsyncEditorTestingUITestContextExtensions
    {
        public static UITestContext TestDemoAsyncEditorLoadOnAdmin(this UITestContext context)
        {
            context.GoToRelativeUrl("/Admin/ContentItemAsyncEditor/DemoAsyncEditorProvider/DemoCustomer");

            context.Exists(By.XPath("//label[text()='First Name']"));
            context.Exists(By.XPath("//label[text()='Last Name']"));
            context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(text(), 'Personal Details')]"));
            context.Exists(By.XPath("//*[contains(@class, 'asyncEditor__groupLink') and contains(text(), 'Additional Notes')]"));

            return context;
        }
    }
}
