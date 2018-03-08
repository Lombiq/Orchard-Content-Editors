using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Orchard.Localization;
using Orchard.Utility;
using Orchard.Utility.Extensions;
using System.Web;
using Lombiq.ContentEditors.ViewModels;

namespace Orchard.Mvc.Html
{
    public static class ContentEditorsHtmlHelperExtensions
    {
        public static IHtmlString TextBoxEditorFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            dynamic shapeHelper,
            Expression<Func<TModel, TValue>> expression,
            string labelText,
            bool required = false,
            string hint = "",
            Action<EditorViewModel> viewModelBuilder = null)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var viewModel = new EditorViewModel
            {
                Name = GetPartialFieldNameFor(expression),
                Value = ModelMetadata.FromLambdaExpression(expression, html.ViewData).Model as string,
                Label = labelText,
                Required = required
            };

            viewModelBuilder?.Invoke(viewModel);

            return shapeHelper.Lombiq_ContentEditors_TextBoxEditor(ViewModel: viewModel);
        }


        /// <summary>
        /// Returns the partial field name so the actual editor shape helper will extend it with the required prefix.
        /// </summary>
        private static string GetPartialFieldNameFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression) =>
            ExpressionHelper.GetExpressionText(expression);
    }
}
