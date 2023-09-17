using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css
{
    internal static class StylerHelper
    {
        // TODO Memory leak here.
        // See https://github.com/AvaloniaUI/Avalonia/issues/12455
        // and https://github.com/AvaloniaUI/Avalonia/issues/12457
        /// <summary>
        /// Reapply the theme or style for the <see cref="resourceHost"/> and it's descendents.
        /// </summary>
        /// <param name="resourceHost">The target that will be reapplied the theme or style.</param>
        /// <param name="isTheme">If reapply the theme. If not, the style will be reapplied.</param>
        /// <param name="targetTypes">The types of elements that should be reapplied theme or style.</param>
        public static void ReapplyStyling(this IResourceHost? resourceHost, bool isTheme, IReadOnlyCollection<Type>? targetTypes)
        {
            switch (resourceHost)
            {
                case Application application:
                {
                    ReapplyStyling(application, isTheme, targetTypes);
                    break;
                }
                case StyledElement element:
                {
                    ReapplyStyling(element, isTheme, targetTypes);
                    break;
                }
            }
        }

        public static void ReapplyStyling(this Application? app, bool isTheme, IReadOnlyCollection<Type>? targetTypes)
        {
            if (app == null)
            {
                return;
            }

            switch (app.ApplicationLifetime)
            {
                case ClassicDesktopStyleApplicationLifetime lifetime:
                {
                    foreach (var window in lifetime.Windows)
                    {
                        ForceApplyStyling(window, isTheme, targetTypes);
                    }
                    break;
                }
                case ISingleViewApplicationLifetime { MainView: not null } singleView:
                    ForceApplyStyling(singleView.MainView, isTheme, targetTypes);
                    break;
            }
        }

        public static void ReapplyStyling(this StyledElement styledElement, bool isTheme, IReadOnlyCollection<Type>? targetTypes)
        {
            ForceApplyStyling(styledElement, isTheme, targetTypes);
        }

        private static bool MatchTypeStyleKey(IReadOnlyCollection<Type>? targetTypes, StyledElement? element)
        {
            if (element == null)
            {
                return false;
            }
            if (targetTypes == null)
            {
                return true;
            }

            return targetTypes.Contains(element.StyleKey);
        }
        
        private static void ForceApplyStyling(
            StyledElement styledElement, 
            bool isTheme,
            IReadOnlyCollection<Type>? targetTypes)
        {
            if (isTheme)
            {
                if (MatchTypeStyleKey(targetTypes, styledElement.TemplatedParent as StyledElement))
                {
                    styledElement.OnTemplatedParentControlThemeChanged();
                }
                
                if (MatchTypeStyleKey(targetTypes, styledElement))
                {
                    // [ava-11.0.0] If the window use custom chrome, reapplying control theme will result in a mess.
                    if (styledElement is not Window)
                    {
                        styledElement.OnControlThemeChanged();
                    }
                }
            }
            else
            {
                if (MatchTypeStyleKey(targetTypes, styledElement))
                {
                    styledElement.InvalidStyles();
                }
            }
           
            styledElement.ApplyStyling();


            // For children.
            if (styledElement is Visual visual)
            {
                foreach (var child in visual.GetVisualChildren().OfType<StyledElement>())
                {
                    ForceApplyStyling(child, isTheme, targetTypes);
                }
            }

            // For detached elements.
            switch (styledElement)
            {
                case Popup { Child: StyledElement element1 }:
                    ForceApplyStyling(element1, isTheme, targetTypes);
                    break;
                case ContextMenu contextMenu:
                {
                    foreach (var element2 in contextMenu.Items.OfType<StyledElement>())
                    {
                        ForceApplyStyling(element2, isTheme, targetTypes);
                    }

                    break;
                }
            }
        }



        public static void DetachStylesRecursively(this IResourceHost? resourceHost, IReadOnlyList<Style> styles)
        {
            return;
            
            var list = new List<Style>();
            StylerHelper.FlattenStyles(list, styles);
            
            switch (resourceHost)
            {
                case Application application:
                {
                    switch (application.ApplicationLifetime)
                    {
                        case ClassicDesktopStyleApplicationLifetime lifetime:
                        {
                            foreach (var window in lifetime.Windows)
                            {
                                ForceDetachStyles(window, list);
                            }
                            break;
                        }
                        case ISingleViewApplicationLifetime { MainView: not null } singleView:
                            ForceDetachStyles(singleView.MainView, list);
                            break;
                    }
                    break;
                }
                case StyledElement element:
                {
                    ForceDetachStyles(element, list);
                    break;
                }
            }
        }
        
        private static void ForceDetachStyles(StyledElement styledElement, IReadOnlyList<Style> styles)
        {
            styledElement.DetachStyles(styles);

            if (styledElement is not Visual visual)
            {
                return;
            }

            foreach (var child in visual.GetVisualChildren().OfType<StyledElement>())
            {
                ForceDetachStyles(child, styles);
            }

            switch (styledElement)
            {
                case Popup { Child: StyledElement element1 }:
                    ForceDetachStyles(element1, styles);
                    break;
                case ContextMenu contextMenu:
                {
                    foreach (var element2 in contextMenu.Items.OfType<StyledElement>())
                    {
                        ForceDetachStyles(element2, styles);
                    }

                    break;
                }
            }
        }

        private static void FlattenStyles(List<Style> styles, IReadOnlyList<Style>? appliedStyles)
        {
            if (appliedStyles == null)
            {
                return;
            }
            styles.AddRange(appliedStyles);
            foreach (var style in appliedStyles)
            {
                FlattenStyles(styles, style.Children.OfType<Style>().ToList());
            }
        }
    }
}
