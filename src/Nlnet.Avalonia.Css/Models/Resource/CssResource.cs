using System;
using System.Text.RegularExpressions;

namespace Nlnet.Avalonia.Css
{
    public abstract class CssResource
    {
        private static readonly Regex VarRegex = new("var\\((.*?)\\)", RegexOptions.IgnoreCase);

        public string? Key { get; set; }

        public string? ValueString { get; set; }

        public object? Value { get; set; }

        public bool IsValid => Key != null;

        public bool IsDeferred { get; set; } = false;

        public void AcceptCore(string key, string valueString)
        {
            Key         = key;
            ValueString = valueString;
            Value       = Accept(valueString);
        }

        protected abstract object? Accept(string valueString);
        
        public virtual object? GetDeferredValue(IServiceProvider? provider)
        {
            return null;
        }

        protected static bool IsVar(string? valueString, out string? varKey)
        {
            if (valueString == null)
            {
                varKey = null;
                return false;
            }
            var match = VarRegex.Match(valueString);
            if (match.Success)
            {
                varKey = match.Groups[1].Value;
                return true;
            }

            varKey = null;
            return false;
        }
    }

    public abstract class CssResource<T> : CssResource, IResourceFactory where T : CssResource, new()
    {
        public CssResource Create()
        {
            return new T();
        }
    }
}
