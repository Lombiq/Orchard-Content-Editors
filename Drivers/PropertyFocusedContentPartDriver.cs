using Lombiq.ContentEditors.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            TPart part,
            Expression<Func<TPart, object>> propertyExpression,
            Action<PropertyEditorViewModel> builder)
        {
            var propertyMemberInfo = GetPropertyMemberInfo(propertyExpression);
            var propertyName = propertyMemberInfo.Name;

            return ContentShape(shapeType, () =>
            {
                var editorBuilder = new PropertyEditorViewModel
                {
                    PropertyName = propertyName,
                    EditorType = "Text",
                    Value = propertyExpression.Compile()(part),
                    Prefix = Prefix,
                    Required = Attribute.IsDefined(propertyMemberInfo, typeof(RequiredAttribute))
                };

                builder(editorBuilder);

                if (string.IsNullOrEmpty(editorBuilder.TemplateName))
                {
                    editorBuilder.TemplateName = editorBuilder.HasOwnTemplate ?
                        $"Properties/{_lazyPartName.Value}/{propertyName}" :
                        $"Properties/{editorBuilder.EditorType}";
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
            Func<CombinedPropertyEditorShapeBuilder, CombinedPropertyEditorShapeBuilder> builderFactory) =>
            Combined(builderFactory(new CombinedPropertyEditorShapeBuilder()).PropertyEditorShapeBuilders
                .Select(builder => 
                    (DriverResult)PropertyEditor(
                        (object)shapeHelper, 
                        shapeType,
                        part, 
                        builder.PropertyExpression, 
                        builder.Builder))
                .ToArray());

        protected virtual CombinedResult PropertyEditor(
            dynamic shapeHelper,
            TPart part,
            Func<CombinedPropertyEditorShapeBuilder, CombinedPropertyEditorShapeBuilder> builderFactory) =>
            PropertyEditor(shapeHelper, GetDefaultPartEditorShapeType(), part, builderFactory);



        protected virtual string GetDefaultPartDisplayShapeType() =>
            _lazyDefaultDisplayShapeType.Value;

        protected virtual string GetDefaultPartEditorShapeType() =>
            _lazyDefaultEditorShapeType.Value;


        private static MemberInfo GetPropertyMemberInfo(Expression<Func<TPart, object>> propertyExpression) =>
            ((MemberExpression)propertyExpression.Body).Member;
        

        private static string GenerateDefaultPartDisplayShapeType(string partName)
        {
            if (partName.EndsWith("Part")) partName = partName.Substring(0, partName.Length - 4);

            return $"Parts_{partName}";
        }

        private static string GenerateDefaultPartEditorShapeType(string partName) =>
            $"{GenerateDefaultPartDisplayShapeType(partName)}_Edit";



        public class PropertyEditorShapeBuilder
        {
            public Expression<Func<TPart, object>> PropertyExpression { get; set; }
            public Action<PropertyEditorViewModel> Builder { get; set; }
        }

        public class CombinedPropertyEditorShapeBuilder
        {
            public IList<PropertyEditorShapeBuilder> PropertyEditorShapeBuilders { get; set; }


            public CombinedPropertyEditorShapeBuilder()
            {
                PropertyEditorShapeBuilders = new List<PropertyEditorShapeBuilder>();
            }


            public CombinedPropertyEditorShapeBuilder With(
                Expression<Func<TPart, object>> propertyExpression,
                Action<PropertyEditorViewModel> builder)
            {
                PropertyEditorShapeBuilders.Add(new PropertyEditorShapeBuilder
                {
                    PropertyExpression = propertyExpression,
                    Builder = builder
                });

                return this;
            }
        }
    }
}