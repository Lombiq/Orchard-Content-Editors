using Lombiq.ContentEditors.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lombiq.ContentEditors.Drivers
{
    public abstract class PropertyFocusedContentPartDriver<TPart> : ContentPartDriver<TPart>, IContentPartDriver where TPart : ContentPart, new()
    {
        private readonly Lazy<string> _lazyPartName;
        private readonly Lazy<string> _lazyDefaultDisplayShapeType;
        private readonly Lazy<string> _lazyDefaultEditorShapeType;


        protected override string Prefix => _lazyPartName.Value;


        public PropertyFocusedContentPartDriver()
        {
            _lazyPartName = new Lazy<string>(() => typeof(TPart).Name);
            _lazyDefaultEditorShapeType = new Lazy<string>(() =>
                GenerateDefaultPartEditorShapeType(_lazyPartName.Value));
            _lazyDefaultDisplayShapeType = new Lazy<string>(() =>
                GenerateDefaultPartDisplayShapeType(_lazyPartName.Value));
        }


        protected virtual ContentShapeResult PropertyEditor(
            dynamic shapeHelper,
            string shapeType,
            bool uniqueTemplate,
            TPart part,
            Expression<Func<TPart, object>> propertyExpression,
            Action<PropertyEditorViewModel> builder)
        {
            var propertyName = GetPropertyName(propertyExpression);

            return ContentShape(shapeType, () =>
            {
                var editorBuilder = new PropertyEditorViewModel
                {
                    PropertyName = propertyName,
                    EditorType = "Text",
                    Value = propertyExpression.Compile()(part),
                    Prefix = Prefix
                };

                editorBuilder.TemplateName = uniqueTemplate ?
                    $"Properties/{_lazyPartName.Value}/{propertyName}" : 
                    $"Properties/{editorBuilder.EditorType}";

                builder(editorBuilder);

                if (string.IsNullOrEmpty(editorBuilder.TemplateName))
                {
                    editorBuilder.TemplateName = $"Properties/{editorBuilder.EditorType}";
                }

                return shapeHelper.EditorTemplate(
                    TemplateName: editorBuilder.TemplateName,
                    Model: editorBuilder,
                    Prefix: editorBuilder.Prefix);
            }).Differentiator(propertyName);
        }

        protected virtual CombinedResult PropertyEditor(
            dynamic shapeHelper,
            string shapeType,
            TPart part,
            params PropertyEditorShapeBuilder[] propertyEditorShapeBuilders) =>
            Combined(propertyEditorShapeBuilders
                .Select(builder => 
                    (DriverResult)PropertyEditor(
                        (object)shapeHelper, 
                        shapeType, 
                        builder.UniqueTemplate, 
                        part, 
                        builder.PropertyExpression, 
                        builder.Builder))
                .ToArray());

        protected virtual CombinedResult PropertyEditor(
            dynamic shapeHelper,
            TPart part,
            params PropertyEditorShapeBuilder[] propertyEditorShapeBuilders) =>
            Combined(propertyEditorShapeBuilders
                .Select(builder =>
                    (DriverResult)PropertyEditor(
                        (object)shapeHelper,
                        GetDefaultPartEditorShapeType(),
                        builder.UniqueTemplate,
                        part,
                        builder.PropertyExpression,
                        builder.Builder))
                .ToArray());

        protected virtual PropertyEditorShapeBuilder For(
            Expression<Func<TPart, object>> propertyExpression,
            Action<PropertyEditorViewModel> builder,
            bool useUniqueTemplate = false) =>
            new PropertyEditorShapeBuilder
            {
                PropertyExpression = propertyExpression,
                Builder = builder,
                UniqueTemplate = useUniqueTemplate
            };

        protected virtual PropertyEditorShapeBuilder ForUnique(
            Expression<Func<TPart, object>> propertyExpression,
            Action<PropertyEditorViewModel> builder) =>
            For(propertyExpression, builder, true);

        protected virtual string GetDefaultPartDisplayShapeType() =>
            _lazyDefaultDisplayShapeType.Value;

        protected virtual string GetDefaultPartEditorShapeType() =>
            _lazyDefaultEditorShapeType.Value;


        private static string GetPropertyName(Expression<Func<TPart, object>> propertyExpression) =>
            ((MemberExpression)propertyExpression.Body).Member.Name;



        private static string GenerateDefaultPartDisplayShapeType(string partName)
        {
            if (partName.EndsWith("Part")) partName = partName.Substring(0, partName.Length - 4);

            return $"Parts_{partName}";
        }

        private static string GenerateDefaultPartEditorShapeType(string partName) =>
            $"{GenerateDefaultPartDisplayShapeType(partName)}_Edit";


        public class PropertyEditorShapeBuilder
        {
            public bool UniqueTemplate { get; set; }
            public Expression<Func<TPart, object>> PropertyExpression { get; set; }
            public Action<PropertyEditorViewModel> Builder { get; set; }
        }

    }
}