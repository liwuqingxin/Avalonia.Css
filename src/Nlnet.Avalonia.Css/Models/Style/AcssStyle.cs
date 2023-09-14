using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface IAcssStyle : IAcssSection, IDisposable
{
    public bool IsThemeChild { get; }

    public bool IsLogicalChild { get; }

    public Type? ThemeTargetType { get; }

    public IEnumerable<IAcssSetter>? Setters { get; set; }

    public IEnumerable<IAcssStyle>? Styles { get; set; }

    public IEnumerable<IAcssResourceDictionary>? Resources { get; set; }

    public IEnumerable<IAcssAnimation>? Animations { get; set; }

    public Selector? GetSelector();

    public Type? GetTargetType();

    public ChildStyle ToAvaloniaStyle();

    void AddDisposable(IDisposable disposable);
}

internal class AcssStyle : AcssSection, IAcssStyle
{
    private readonly IAcssBuilder _builder;
    private Selector? _selector;
    private CompositeDisposable? _compositeDisposable;

    public bool IsThemeChild { get; set; }

    public bool IsLogicalChild { get; set; }

    public Type? ThemeTargetType { get; set; }

    public IEnumerable<IAcssSetter>? Setters { get; set; }

    public IEnumerable<IAcssStyle>? Styles { get; set; }

    public IEnumerable<IAcssResourceDictionary>? Resources { get; set; }

    public IEnumerable<IAcssAnimation>? Animations { get; set; }

    public AcssStyle(IAcssBuilder builder, string selector) : base(builder, selector)
    {
        _builder = builder;
    }

    public override void InitialSection(IAcssParser parser, ReadOnlySpan<char> content)
    {
        _selector = CreateSelector();

        parser.ParseSettersAndChildren(content, out var settersSpan, out var childrenSpan);

        var pairs = parser.ParsePairs(settersSpan);
        var acssSetters = new List<IAcssSetter>();
        foreach (var pair in pairs)
        {
            if (pair.Item1.StartsWith(BehaviorConstraints.AddToken) || pair.Item1.StartsWith(BehaviorConstraints.RemoveToken))
            {
                acssSetters.Add(new AcssSetter(pair.Item1, pair.Item2));
                continue;
            }
            var index = acssSetters.FindIndex(s => s.Property == pair.Item1);
            if (index != -1)
            {
                acssSetters.RemoveAt(index);
                acssSetters.Insert(index, new AcssSetter(pair.Item1, pair.Item2));
                this.WriteError($"Duplicated setter for property '{pair.Item1}' is detected. Use the later one that value is '{pair.Item2}'.");
            }
            else
            {
                acssSetters.Add(new AcssSetter(pair.Item1, pair.Item2));
            }
        }
        Setters = acssSetters;
        var list = parser.ParseSections(this, childrenSpan).ToList();
        if (list.Count > 0)
        {
            Children   = list;
            Styles     = list.OfType<IAcssStyle>();
            Resources  = list.OfType<IAcssResourceDictionary>();
            Animations = list.OfType<IAcssAnimation>();
        }
    }

    Type? IAcssStyle.GetTargetType()
    {
        if (ThemeTargetType != null)
        {
            return ThemeTargetType;
        }

        if (_selector?.GetTargetType() is { } targetType)
        {
            return targetType;
        }

        return Parent is not IAcssStyle parentAcssStyle ? null : parentAcssStyle.GetTargetType();
    }

    private Selector? CreateSelector()
    {
        var isChild = (Parent != null && IsLogicalChild == false) || IsThemeChild;

        // Selector
        var selector   = isChild ? Selectors.Nesting(null) : null;
        var syntaxList = SelectorGrammar.Parse(Selector).ToList();
        var selectors  = new List<Selector>();

        if(IsThemeChild)
        {
            ThemeTargetType = syntaxList.First().ToSelector(_builder, this, null)?.GetTargetType();
            syntaxList = syntaxList.Skip(1).ToList();
        }

        foreach (var syntax in syntaxList)
        {
            if (syntax is CommaSyntax)
            {
                if (selector != null)
                {
                    selectors.Add(selector);
                }
                selector = isChild ? Selectors.Nesting(null) : null;
            }
            else
            {
                selector = syntax.ToSelector(_builder, this, selector);
            }
        }
        if (selector != null)
        {
            selectors.Add(selector);
        }
        
        return selectors.Count > 1 ? Selectors.Or(selectors) : selector;
    }

    public Selector? GetSelector()
    {
        return _selector;
    }

    public ChildStyle ToAvaloniaStyle()
    {
        DiagnosisHelper.WriteLine($"---- Parsing style '{this}'.");

        var style = NewStyle();
        var targetType = ((IAcssStyle)this).GetTargetType();
        if (targetType == null)
        {
            this.WriteError($"The target type is null as raw selector string is '{Selector}'. Empty avalonia style is created.");
            return style;
        }

        // Resources
        if (Resources != null)
        {
            foreach (var acssResourceList in Resources)
            {
                var dic = acssResourceList.ToAvaloniaResourceDictionary(_builder);
                if (dic == null)
                {
                    continue;
                }
                if (acssResourceList.IsThemeResource())
                {
                    style.Resources.ThemeDictionaries.Add(acssResourceList.GetThemeVariant(), dic);
                }
                else
                {
                    style.Resources.MergedDictionaries.Add(dic);
                }
            }
        }

        // Setters
        if (Setters != null)
        {
            foreach (var setter in Setters.Select(s => s.ToAvaloniaSetter(_builder, targetType)).OfType<Setter>())
            {
                style.Add(setter);
            }
        }

        // Children Styles
        if (Styles != null)
        {
            foreach (var acssStyle in Styles)
            {
                if (acssStyle.IsLogicalChild)
                {
                    var existSetter = style.Setters.OfType<Setter>().FirstOrDefault(s => s.Property == ExStyler.AddingStyleProperty);
                    if (existSetter?.Value is IList<IAcssStyle> list)
                    {
                        list.Add(acssStyle);
                    }
                    else
                    {
                        list = new List<IAcssStyle>();
                        list.Add(acssStyle);
                        var setter = new Setter()
                        {
                            Property = ExStyler.AddingStyleProperty,
                            Value    = list,
                        };
                        style.Add(setter);
                    }
                }
                else
                {
                    var childStyle = acssStyle.ToAvaloniaStyle();
                    style.Add(childStyle);
                }
            }
        }

        // Style Animations
        if (Animations != null)
        {
            foreach (var acssAnimation in Animations)
            {
                var animation = acssAnimation.ToAvaloniaAnimation();
                if (animation != null)
                {
                    style.Animations.Add(animation);
                }
            }
        }

        return style;
    }

    public void AddDisposable(IDisposable disposable)
    {
        lock (this)
        {
            _compositeDisposable ??= new CompositeDisposable();
            _compositeDisposable.Add(disposable);
        }
    }

    private ChildStyle NewStyle()
    {
        return IsLogicalChild
            ? new LogicChildStyle(this)
            {
                Selector = _selector
            }
            : new ChildStyle(this)
            {
                Selector = _selector
            };
    }

    public override string ToString()
    {
        var styleKind = IsThemeChild ? "^" : "";
        if (IsLogicalChild)
        {
            styleKind = ">";
        }
        else if (Parent is IAcssStyle parentAcssStyle)
        {
            styleKind = $" - ";
        }

        return $"{styleKind}{Selector}";
    }

    public string ToDetailString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"'{Selector}'");
        if (Setters != null)
        {
            foreach (var acssSetter in Setters)
            {
                builder.AppendLine($"    {acssSetter.ToString()}");
            }
        }
        return builder.ToString();
    }

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
        _compositeDisposable = null;

        if (Styles != null)
        {
            foreach (var acssStyle in Styles)
            {
                acssStyle.Dispose();
            }
        }
    }
}
