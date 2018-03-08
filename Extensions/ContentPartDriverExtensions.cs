//using Orchard.ContentManagement;
//using Orchard.ContentManagement.Drivers;
//using Orchard.Validation;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Lombiq.ContentEditors.Extensions
//{
//    public static class ContentPartDriverExtensions
//    {
//        public static ContentShapeResult PropertyEditor<TContent, TValue>(
//            this ContentPartDriver<TContent> driver,
//            dynamic shapeHelper,
//            TValue propertyValue,
//            string propertyName,
//            string editorType = "Text") where TContent : ContentPart, new()
//        {
//            return driver.ContentShape(GetPropertyEditorShapeTypeBase(driver, propertyName), () =>
//            {
//                return shapeHelper.EditorTemplate(
//                    TemplateName: "Parts/Project.Engagement",
//                    Model: part,
//                    Prefix: driver.Prefix);
//            });
//        }
        
//        public static string GetShapeTypeBase<TContent>(this ContentPartDriver<TContent> driver) where TContent : ContentPart, new()
//        {
//            var partNameWithoutSuffix = typeof(TContent).Name;
//            if (partNameWithoutSuffix.EndsWith("Part"))
//            {
//                partNameWithoutSuffix = partNameWithoutSuffix.Substring(0, partNameWithoutSuffix.Length - 4);
//            }

//            return $"Parts_{partNameWithoutSuffix}";
//        }

//        public static string GetEditorShapeTypeBase<TContent>(this ContentPartDriver<TContent> driver) where TContent : ContentPart, new() =>
//            $"{GetShapeTypeBase(driver)}_Edit";

//        public static string GetPropertyEditorShapeTypeBase<TContent>(this ContentPartDriver<TContent> driver, string propertyName) where TContent : ContentPart, new() =>
//            $"{GetShapeTypeBase(driver)}_{propertyName}_Edit";
//    }
//}