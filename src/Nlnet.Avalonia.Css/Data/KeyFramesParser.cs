using System;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css;

// TODO 和TransitionsParser统一；

public class KeyFramesParser
{
    private static readonly Regex KeyFrameRegex = new("^KeyFrame\\s*\\((.*?)\\)\\s*\\:\\s*$", RegexOptions.IgnoreCase);

    public static KeyFrames? Parse(string keyFramesString)
    {
        var parser = ServiceLocator.GetService<ICssParser>();
        if (parser == null)
        {
            throw new Exception($"Can not find the {nameof(ICssParser)} service.");
        }

        var keyFrames = new KeyFrames();
        var objects   = parser.ParseObjects(keyFramesString);

        foreach (var (selector, propertySettingsString) in objects)
        {
            var match = KeyFrameRegex.Match(selector);
            if (match.Success)
            {
                var initString = match.Groups[1].Value;
            }
        }



        //var keyFrameList = keyFramesString.Trim('[', ']', ' ').Split(';', StringSplitOptions.RemoveEmptyEntries);
        //foreach (var keyFrame in keyFrameList)
        //{
        //    if (InterpreterHelper.IsVar(transition, out var key) && Application.Current != null)
        //    {
        //        if (Application.Current.TryFindResource(key!, out var resource) && resource is ITransition t)
        //        {
        //            keyFrames.Add(t);
        //        }
        //    }
        //    else
        //    {
        //        var t = InterpreterHelper.ParseTransition(transition);
        //        if (t != null)
        //        {
        //            keyFrames.Add(t);
        //        }
        //    }
        //}

        return keyFrames;
    }
}
