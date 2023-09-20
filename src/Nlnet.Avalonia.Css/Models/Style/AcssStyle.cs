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
    
    bool MatchKey(string one);
    
    void ForceMergeBase();
}

internal class AcssStyle : AcssSection, IAcssStyle
{
    private readonly IAcssBuilder _builder;
    private readonly AcssTokens _tokens;
    private string? _selectorString;
    private Selector? _selector;
    private CompositeDisposable? _compositeDisposable;
    private List<IAcssSetter> _localSetters = new();
    private List<IAcssSection> _localChildren = new();
    private IList<string>? _bases;

    public bool IsThemeChild { get; set; }

    public bool IsLogicalChild { get; set; }

    public Type? ThemeTargetType { get; private set; }

    public IEnumerable<IAcssSetter>? Setters { get; set; }

    public IEnumerable<IAcssStyle>? Styles { get; set; }

    public IEnumerable<IAcssResourceDictionary>? Resources { get; set; }

    public IEnumerable<IAcssAnimation>? Animations { get; set; }

    public AcssStyle(IAcssBuilder builder, AcssTokens tokens, string header) : base(builder, header)
    {
        _builder = builder;
        _tokens = tokens;
    }

    public override void InitialSection(IAcssParser parser, ReadOnlySpan<char> content)
    {
        // Selector
        var interpreter = _builder.Interpreter;
        _selectorString = interpreter.ParseSelectorAndBases(Header, out _bases);
        _selector = CreateSelector(_selectorString);
        
        parser.ParseSettersAndChildren(content, out var settersSpan, out var childrenSpan);
        var pairs = parser.ParsePairs(settersSpan);

        // Setters
        foreach (var pair in pairs)
        {
            var setter = new AcssSetter(pair.Item1, pair.Item2);
            if (pair.Item1.StartsWith(BehaviorConstraints.AddToken) || pair.Item1.StartsWith(BehaviorConstraints.RemoveToken))
            {
                _localSetters.Add(setter);
                continue;
            }
            
            ReplaceOrAddSetter(_localSetters, setter);
        }
        
        // Children
        _localChildren.AddRange(parser.ParseSections(_tokens, this, childrenSpan));
    }

    public override IAcssSection Clone()
    {
        var acssStyle = new AcssStyle(_builder, _tokens, Header);

        acssStyle._selectorString = _selectorString;
        acssStyle._selector = _selector;
        acssStyle._localSetters = _localSetters;
        acssStyle._localChildren = _localChildren;
        acssStyle._bases = _bases;
        acssStyle.IsThemeChild = IsThemeChild;
        acssStyle.IsLogicalChild = IsLogicalChild;
        acssStyle.ThemeTargetType = ThemeTargetType;
        acssStyle.Children = Children;
        acssStyle.Parent = Parent;
        acssStyle.Setters = Setters;
        acssStyle.Styles = Styles;
        acssStyle.Resources = Resources;
        acssStyle.Animations = Animations;
        
        return acssStyle;
    }

    private void ReplaceOrAddSetter(List<IAcssSetter> list, IAcssSetter setter)
    {
        var index = list.FindIndex(s => s.Property == setter.Property);
        if (index != -1)
        {
            list.RemoveAt(index);
            list.Insert(index, setter);
            this.WriteError($"Duplicated setter for property '{setter.Property}' is detected. Use the later one that value is '{setter.RawValue}'.");
        }
        else
        {
            list.Add(setter);
        }
    }

    private void ApplyBases(IEnumerable<string> bases, List<IAcssSetter> setters, List<IAcssSection> children)
    {
        foreach (var one in bases)
        {
            var style = _tokens.TryGetBaseStyle(one);
            if (style == null)
            {
                continue;
            }
            
            if (style.Setters != null)
            {
                foreach (var acssSetter in style.Setters)
                {
                    ReplaceOrAddSetter(setters, acssSetter);
                }
            }

            if (style.Children != null)
            {
                children.AddRange(style.Children.Select(c => c.Clone()));
            }
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

    private Selector? CreateSelector(string selectorString)
    {
        var isChild = (Parent != null && IsLogicalChild == false) || IsThemeChild;
        var selector   = isChild ? Selectors.Nesting(null) : null;
        var syntaxList = SelectorGrammar.Parse(selectorString).ToList();
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
            this.WriteError($"The target type is null as raw header string is '{Header}'. Empty avalonia style is created.");
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

    public bool MatchKey(string one)
    {
        return this._selectorString == one;
    }

    public void ForceMergeBase()
    {
        var setters = new List<IAcssSetter>();
        var children = new List<IAcssSection>();

        if (_bases != null)
        {
            ApplyBases(_bases, setters, children);
        }
        
        foreach (var acssStyle in children)
        {
            acssStyle.Parent = this;
        }

        if(_localSetters.Count > 0)
        {
            foreach (var setter in _localSetters)
            {
                ReplaceOrAddSetter(setters, setter);
            }
        }
        if (_localChildren.Count > 0)
        {
            children.AddRange(_localChildren);
        }
        
        Setters    = setters;
        Children   = children;
        Styles     = children.OfType<IAcssStyle>();
        Resources  = children.OfType<IAcssResourceDictionary>();
        Animations = children.OfType<IAcssAnimation>();

        foreach (var acssStyle in Styles)
        {
            acssStyle.ForceMergeBase();
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

        return $"{styleKind}{Header}";
    }

    public string ToDetailString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"'{Header}'");
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
