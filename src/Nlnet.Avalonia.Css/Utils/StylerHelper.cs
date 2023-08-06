using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css
{
    internal static class StylerHelper
    {
        internal static void ReapplyStyling(IResourceHost? resourceHost)
        {
            // TODO Memory leak here.
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
                                ForceApplyStyling(window, true, true, true);
                            }
                            break;
                        }
                        case ISingleViewApplicationLifetime { MainView: not null } singleView:
                            ForceApplyStyling(singleView.MainView, true, true, true);
                            break;
                    }
                    break;
                }
                case StyledElement element:
                {
                    ForceApplyStyling(element, true, true, true);
                    break;
                }
            }
        }

        internal static void ReapplyStyling(StyledElement styledElement, bool parentControlTheme, bool controlTheme, bool styling)
        {
            ForceApplyStyling(styledElement, parentControlTheme, controlTheme, styling);
        }

        private static void ForceApplyStyling(StyledElement styledElement, bool parentControlTheme, bool controlTheme, bool styling)
        {
            if (parentControlTheme)
            {
                styledElement.OnTemplatedParentControlThemeChanged();
            }
            if (controlTheme)
            {
                styledElement.OnControlThemeChanged();
            }
            if (styling)
            {
                styledElement.InvalidStyles();
            }
            styledElement.ApplyStyling();

            if (styledElement is not Visual visual)
            {
                return;
            }

            foreach (var child in visual.GetVisualChildren().OfType<StyledElement>())
            {
                ForceApplyStyling(child, parentControlTheme, controlTheme, styling);
            }

            if (styledElement is Popup { Child: StyledElement element1 })
            {
                ForceApplyStyling(element1, parentControlTheme, controlTheme, styling);
            }

            if (styledElement is ContextMenu contextMenu)
            {
                foreach (var element2 in contextMenu.Items.OfType<StyledElement>())
                {
                    ForceApplyStyling(element2, parentControlTheme, controlTheme, styling);
                }
            }
        }
    }
}
