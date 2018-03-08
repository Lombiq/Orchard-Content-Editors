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


        protected override string Prefix => _lazyPartName.Value;


        public PropertyFocusedContentPartDriver()
        {
            _lazyPartName = new Lazy<string>(() => typeof(TPart).Name);
        }


        protected virtual ContentShapeResult PropertyEditor(
            dynamic shapeHelper,
            string shapeType,
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

        protected virtual ContentShapeResult PropertyEditor(
            dynamic shapeHelper,
            TPart part,
            Expression<Func<TPart, object>> propertyExpression,
            Action<PropertyEditorViewModel> builder) =>
            PropertyEditor(
                shapeHelper,
                GetPartEditorShapeType(), 
                part, 
                propertyExpression, 
                builder);

        protected virtual CombinedResult PropertyEditor(
            dynamic shapeHelper,
            string shapeType,
            TPart part,
            params PropertyEditorShapeBuilder[] propertyEditorShapeBuilders) =>
            Combined(propertyEditorShapeBuilders
                .Select(builder => (DriverResult)PropertyEditor((object)shapeHelper, shapeType, part, builder.PropertyExpression, builder.Builder))
                .ToArray());

        protected virtual CombinedResult PropertyEditor(
            dynamic shapeHelper,
            TPart part,
            params PropertyEditorShapeBuilder[] propertyEditorShapeBuilders)
        {
            var results = propertyEditorShapeBuilders
                .Select(builder => (DriverResult)PropertyEditor((object)shapeHelper, part, builder.PropertyExpression, builder.Builder))
                .ToArray();

            return Combined(results);
        }

        protected virtual PropertyEditorShapeBuilder For(
            Expression<Func<TPart, object>> propertyExpression,
            Action<PropertyEditorViewModel> builder) =>
            new PropertyEditorShapeBuilder
            {
                PropertyExpression = propertyExpression,
                Builder = builder
            };
        
        protected virtual string GetPartShapeType()
        {
            var partName = _lazyPartName.Value;
            if (partName.EndsWith("Part")) partName = partName.Substring(0, partName.Length - 4);

            return $"Parts_{partName}";
        }

        protected virtual string GetPartEditorShapeType() =>
            $"{GetPartShapeType()}_Edit";


        private string GetPropertyName(Expression<Func<TPart, object>> propertyExpression) =>
            ((MemberExpression)propertyExpression.Body).Member.Name;


        public class PropertyEditorShapeBuilder
        {
            public Expression<Func<TPart, object>> PropertyExpression { get; set; }
            public Action<PropertyEditorViewModel> Builder { get; set; }
        }

    }
}