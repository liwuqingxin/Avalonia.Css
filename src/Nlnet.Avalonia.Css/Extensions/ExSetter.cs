//using System;
//using System.ComponentModel;
//using System.Linq;
//using System.Reflection;
//using Avalonia;
//using Avalonia.Animation;
//using Avalonia.Controls;
//using Avalonia.Controls.Primitives;
//using Avalonia.Data;
//using Avalonia.Media;
//using Avalonia.Metadata;
//using Avalonia.Styling;
//using DynamicData;

//namespace Nlnet.Avalonia.Css
//{
//    internal enum TargetFindingMode
//    {
//        /// <summary>
//        /// The template parent for the hit object.
//        /// </summary>
//        TemplateParent,
//        /// <summary>
//        /// The value of the property of the hit object.
//        /// </summary>
//        Property,
//        /// <summary>
//        /// Ancestor of the hit object.
//        /// </summary>
//        Ancestor,
//        /// <summary>
//        /// Next sibling of the hit object.
//        /// </summary>
//        NextSibling,
//        /// <summary>
//        /// Last sibling of the hit object.
//        /// </summary>
//        LastSibling,
//    }

//    /// <summary>
//    /// Specific setter that can handle some complex situations.
//    /// </summary>
//    internal class ExSetter : global::Avalonia.Styling.Setter
//    {
//        private object? _value;

//        /// <summary>
//        /// The mode for finding target of this setter. It defines some solutions for kinds of situations.
//        /// </summary>
//        public TargetFindingMode Mode { get; set; } = TargetFindingMode.TemplateParent;

//        /// <summary>
//        /// The target property of this setter. It defines the property of the style selector target, whose value is the real setter target.
//        /// Only take effect when <see cref="Mode"/> is <see cref="TargetFindingMode.Property"/>.
//        /// </summary>
//        public AvaloniaProperty? HitTargetProperty { get; set; }

//        /// <summary>
//        /// The type that target should be. Only take effect when <see cref="Mode"/> is <see cref="TargetFindingMode.Ancestor"/>,
//        /// <see cref="TargetFindingMode.NextSibling"/> or <see cref="TargetFindingMode.LastSibling"/>.
//        /// </summary>
//        public Type? FindType { get; set; }

//        /// <summary>
//        /// The name that target should have. Only take effect when <see cref="Mode"/> is <see cref="TargetFindingMode.Ancestor"/>,
//        /// <see cref="TargetFindingMode.NextSibling"/> or <see cref="TargetFindingMode.LastSibling"/>.
//        /// </summary>
//        [DependsOn("FindType")]
//        public string? FindName { get; set; }

//        /// <summary>
//        /// The level refers to searching the Nth object that meets the conditions. It is 1 by default.
//        /// Only take effect when <see cref="Mode"/> is <see cref="TargetFindingMode.Ancestor"/>,
//        /// <see cref="TargetFindingMode.NextSibling"/> or <see cref="TargetFindingMode.LastSibling"/>.
//        /// </summary>
//        public int FindLevel { get; set; } = 1;

//        /// <summary>
//        /// Gets or sets the property to set. It defines the real setter property.
//        /// </summary>
//        public new AvaloniaProperty? Property { get; set; }

//        /// <summary>
//        /// Gets or sets the property value.
//        /// </summary>
//        [Content]
//        [AssignBinding]
//        [DependsOn("Property")]
//        public new object? Value
//        {
//            get => this._value;
//            set
//            {
//                if (this.Property == null)
//                {
//                    throw new InvalidOperationException("Setter.Property must be set.");
//                }

//                if (value is ISetterValue setterValue)
//                {
//                    setterValue.Initialize((Setter)this);
//                }

//                if (value == null)
//                {
//                    this._value = null;
//                }
//                else if (value is string s)
//                {
//                    // TODO 参考css 修改Value解析
//                    if (Property.PropertyType == typeof(string))
//                    {
//                        this._value = value;
//                    }
//                    else if (Property.PropertyType == typeof(Thickness))
//                    {
//                        this._value = Thickness.Parse(s);
//                    }
//                    else if (Property.PropertyType == typeof(double))
//                    {
//                        this._value = double.Parse(s);
//                    }
//                    else if (Property.PropertyType == typeof(Size))
//                    {
//                        this._value = Size.Parse(s);
//                    }
//                    else if (Property.PropertyType == typeof(Matrix))
//                    {
//                        this._value = Matrix.Parse(s);
//                    }
//                    else if (Property.PropertyType == typeof(CornerRadius))
//                    {
//                        this._value = CornerRadius.Parse(s);
//                    }
//                    else if (Property.PropertyType == typeof(Transform))
//                    {
//                        this._value = Transform.Parse(s);
//                    }
//                    else if (Property.PropertyType.IsAssignableTo(typeof(Enum)))
//                    {
//                        this._value = Enum.Parse(Property.PropertyType, s);
//                    }
//                    else if (Property.PropertyType.IsAssignableTo(typeof(IBrush)))
//                    {
//                        this._value = Brush.Parse(s);
//                    }
//                    else if (Property?.PropertyType != null && Property.PropertyType != typeof(string))
//                    {
//                        var converter = TypeDescriptor.GetConverter(Property.PropertyType);
//                        if (converter.IsValid(value) == false)
//                        {
//                            throw new ArgumentException($"Can not convert type {value.GetType()} to type {Property.PropertyType}");
//                        }
//                        this._value = converter.ConvertFrom(s);
//                    }
//                }
//                else
//                {
//                    this._value = value;
//                }
//            }
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:Avalonia.Styling.Setter" /> class.
//        /// </summary>
//        public ExSetter()
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:Avalonia.Styling.Setter" /> class.
//        /// </summary>
//        /// <param name="property">The property to set.</param>
//        /// <param name="value">The property value.</param>
//        public ExSetter(AvaloniaProperty property, object value)
//            : base(property, value)
//        {
            
//        }

//        ISetterInstance ISetter.Instance(IStyleable target)
//        {
//            target = target ?? throw new ArgumentNullException(nameof(target));
//            if (this.Property == null)
//            {
//                throw new InvalidOperationException("Setter.Property must be set.");
//            }

//            switch (Mode)
//            {
//                case TargetFindingMode.TemplateParent when target.TemplatedParent == null:
//                {
//                    throw new InvalidOperationException($"Target must have a template parent when {nameof(Mode)} of setter is {nameof(TargetFindingMode.TemplateParent)}.");
//                }
//                case TargetFindingMode.TemplateParent:
//                {
//                    target = target.TemplatedParent as IStyleable ?? throw new InvalidOperationException($"Template parent of target must be {nameof(IStyleable)} when {nameof(Mode)} of setter is {nameof(TargetFindingMode.TemplateParent)}.");
//                    break;
//                }
//                case TargetFindingMode.Property when HitTargetProperty == null:
//                {
//                    throw new InvalidOperationException($"{nameof(HitTargetProperty)} must be set when {nameof(Mode)} of setter is {nameof(TargetFindingMode.Property)}.");
//                }
//                case TargetFindingMode.Property:
//                {
//                    // Try to apply control template.
//                    if (target is TemplatedControl templatedControl)
//                    {
//                        templatedControl.ApplyTemplate();
//                    }

//                    var targetPropertyValue = target.GetValue(HitTargetProperty);
//                    if (targetPropertyValue is not IStyleable styleable)
//                    {
//                        throw new InvalidOperationException($"The value of {nameof(HitTargetProperty)} must be a {nameof(IStyleable)} when {nameof(Mode)} of setter is {nameof(TargetFindingMode.Property)}.");
//                    }

//                    target = styleable;
//                    break;
//                }
//                case TargetFindingMode.Ancestor:
//                {
//                    var count  = FindLevel;
//                    var parent = (target as IControl)?.Parent;
//                    while (true)
//                    {
//                        if (parent == null)
//                        {
//                            throw new InvalidOperationException($"Can not find the {FindLevel}th ancestor with type {FindType}.");
//                        }

//                        if ((FindType == null || parent.GetType().IsAssignableTo(FindType)) && (FindName == null || parent.Name == FindName))
//                        {
//                            count--;
//                            if (count == 0)
//                            {
//                                target = parent;
//                                break;
//                            }
//                        }

//                        parent = parent.Parent;
//                    }
//                    break;
//                }
//                case TargetFindingMode.NextSibling:
//                {
//                    var count   = FindLevel;
//                    var control = (target as IControl)!;
//                    var parent  = control.Parent;
//                    if (parent == null)
//                    {
//                        throw new InvalidOperationException($"The parent of target is null and so we can not find the next sibling.");
//                    }
//                    var index = parent.VisualChildren.OfType<IControl>().IndexOf(control);
//                    while (true)
//                    {
//                        if (++index >= parent.VisualChildren.Count)
//                        {
//                            throw new InvalidOperationException($"Can not find the {FindLevel}th next sibling with type {FindType}.");
//                        }
//                        control = parent.VisualChildren[index] as IControl;
//                        if (control == null)
//                        {
//                            continue;
//                        }
//                        if ((FindType == null || control.GetType().IsAssignableTo(FindType)) && (FindName == null || control.Name == FindName))
//                        {
//                            count--;
//                            if (count == 0)
//                            {
//                                target = control;
//                                break;
//                            }
//                        }
//                    }
//                    break;
//                }
//                case TargetFindingMode.LastSibling:
//                {
//                    var count   = FindLevel;
//                    var control = (target as IControl)!;
//                    var parent  = control.Parent;
//                    if (parent == null)
//                    {
//                        throw new InvalidOperationException($"The parent of target is null and so we can not find the next sibling.");
//                    }
//                    var index = parent.VisualChildren.OfType<IControl>().IndexOf(control);
//                    while (true)
//                    {
//                        if (--index < 0)
//                        {
//                            throw new InvalidOperationException($"Can not find the {FindLevel}th next sibling with type {FindType}.");
//                        }
//                        control = parent.VisualChildren[index] as IControl;
//                        if (control == null)
//                        {
//                            continue;
//                        }
//                        if ((FindType == null || control.GetType().IsAssignableTo(FindType)) && (FindName == null || control.Name == FindName))
//                        {
//                            count--;
//                            if (count == 0)
//                            {
//                                target = control;
//                                break;
//                            }
//                        }
//                    }
//                    break;
//                }
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
            
//            var method   = typeof(AvaloniaProperty).GetMethod("CreateSetterInstance", BindingFlags.Instance | BindingFlags.NonPublic);
//            var instance = (ISetterInstance?)method?.Invoke(this.Property, new[] { target, this.Value });
//            return instance!;
//        }
//    }
//}
