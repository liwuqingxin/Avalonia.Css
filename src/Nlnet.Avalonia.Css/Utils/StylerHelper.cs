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
                    ReapplyStyling(element, isTheme, true, true, true, targetTypes);
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
                        ForceApplyStyling(window, isTheme, true, true, true, targetTypes);
                    }
                    break;
                }
                case ISingleViewApplicationLifetime { MainView: not null } singleView:
                    ForceApplyStyling(singleView.MainView, isTheme, true, true, true, targetTypes);
                    break;
            }
        }

        public static void ReapplyStyling(
            this StyledElement styledElement,
            bool isTheme,
            bool parentControlTheme, 
            bool controlTheme, 
            bool styling, 
            IReadOnlyCollection<Type>? targetTypes)
        {
            ForceApplyStyling(styledElement, isTheme, parentControlTheme, controlTheme, styling, targetTypes);
        }

        private static void ForceApplyStyling(
            StyledElement styledElement, 
            bool isTheme,
            bool parentControlTheme, 
            bool controlTheme, 
            bool styling,
            IReadOnlyCollection<Type>? targetTypes)
        {
            if (isTheme)
            {
                if (parentControlTheme && (targetTypes == null || targetTypes.Contains(styledElement.TemplatedParent?.GetType())))
                {
                    styledElement.OnTemplatedParentControlThemeChanged();
                }
                
                if (controlTheme && (targetTypes == null || targetTypes.Contains(styledElement.GetType())))
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
                if (styling)
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
                    ForceApplyStyling(child, isTheme, parentControlTheme, controlTheme, styling, targetTypes);
                }
            }

            // For detached elements.
            switch (styledElement)
            {
                case Popup { Child: StyledElement element1 }:
                    ForceApplyStyling(element1, isTheme, parentControlTheme, controlTheme, styling, targetTypes);
                    break;
                case ContextMenu contextMenu:
                {
                    foreach (var element2 in contextMenu.Items.OfType<StyledElement>())
                    {
                        ForceApplyStyling(element2, isTheme, parentControlTheme, controlTheme, styling, targetTypes);
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
