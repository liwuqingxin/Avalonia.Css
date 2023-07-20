using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css
{
    internal static class StylerHelper
    {
        public static void ReapplyStyling(IResourceHost? resourceHost)
        {
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
                        case ISingleViewApplicationLifetime { MainView: { } } singleView:
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

        public static void ForceApplyStyling(StyledElement styledElement, bool parentControlTheme, bool controlTheme, bool styling)
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
