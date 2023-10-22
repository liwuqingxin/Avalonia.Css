using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Nlnet.Avalonia.Controls;
using Nlnet.Avalonia.Css.Controls;
using Nlnet.Avalonia.Senior.Controls;
using System;
using System.ComponentModel;

namespace Nlnet.Avalonia.Css.Fluent
{
    /// <summary>
    /// If you would like to create <see cref="AcssFluentTheme"/> in code-behind, you should call
    /// <see cref="ISupportInitialize"/>.<see cref="ISupportInitialize.BeginInit"/> and
    /// <see cref="ISupportInitialize"/>.<see cref="ISupportInitialize.EndInit"/>, and set properties
    /// of <see cref="AcssFluentTheme"/> you want between that two functions.
    /// </summary>
    public partial class AcssFluentTheme : Styles, ISupportInitialize
    {
        private readonly bool _suspendInitializeNotification;
        private IAcssFile? _accentColorFile;

        /// <summary>
        /// Gets or sets the value that indicates if use the recommended prefer source instead.
        /// If set it to true, the local path, which is same to the AbsolutePath of the Uri, will be used to load the source, if the file exists.
        /// </summary>
        public bool UseRecommendedPreferSource { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates if auto export all sources to local.
        /// </summary>
        public bool AutoExportSourceToLocal { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates the prefer local path. This has higher priority than <see cref="UseRecommendedPreferSource"/>.
        /// </summary>
        public string? PreferLocalPath { get; set; }

        static AcssFluentTheme()
        {
            TemplatedControlExtension.Init();
        }

        public AcssFluentTheme()
        {
            _suspendInitializeNotification = true;
            AvaloniaXamlLoader.Load(this);
            _suspendInitializeNotification = false;
        }

        private void Load()
        {
            AcssContext.UseDefaultContext();

            var resProvidersManager = AcssContext.Default.GetService<IResourceProvidersManager>();
            var typeResolverManager = AcssContext.Default.GetService<ITypeResolverManager>();
            var loader = AcssContext.Default.GetService<IAcssLoader>();

            // This is not added to application's styles till now. Register this to resource manager to enable resource access to this.
            resProvidersManager.RegisterResourceProvider(this);

            // Nlnet.Avalonia.Css.Controls
            typeResolverManager.LoadResolver(new GenericTypeResolver<NotifyChangeContentPresenter>());

            // Avalonia.Controls.DataGrid
            typeResolverManager.LoadResolver(new GenericTypeResolver<DataGrid>());

            // Nlnet.Avalonia.Senior
            typeResolverManager.LoadResolver(new GenericTypeResolver<NtScrollViewer>());

            // Nlnet.Avalonia.MessageBox
            typeResolverManager.LoadResolver(new GenericTypeResolver<MessageBox>());

            // Load acss sources.
            {
                //const string debugRelative = "../../src/Nlnet.Avalonia.Css.Fluent/";

                //_accentColorFile = loader.Load(this, new FileSource(
                //    "Acss/Nlnet.Avalonia.Css.Fluent/Resources/AccentColor.acss", 
                //    $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Resources/AccentColor.acss"));

                //loader.LoadCollection(this, new FileSourceCollection(
                //    "Acss/Nlnet.Avalonia.Css.Fluent/Resources",
                //    $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Resources"));
                //loader.LoadCollection(this, new FileSourceCollection(
                //    "Acss/Nlnet.Avalonia.Css.Fluent/",
                //    $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/"));
                //loader.LoadCollection(this, new FileSourceCollection(
                //    "Acss/Nlnet.Avalonia.Css.Fluent/Senior",
                //    $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/Senior"));
                //loader.LoadCollection(this, new FileSourceCollection(
                //    "Acss/Nlnet.Avalonia.Css.Fluent/MessageBox",
                //    $"{debugRelative}Acss/Nlnet.Avalonia.Css.Fluent/MessageBox"));
            }

            // Load acss sources.
            {
                _accentColorFile = loader.Load(this, new EmbeddedSource(new Uri("avares://Nlnet.Avalonia.Css.Fluent/Acss/Nlnet.Avalonia.Css.Fluent/Resources/AccentColor.acss"), PreferLocalPath, UseRecommendedPreferSource, AutoExportSourceToLocal));

                loader.LoadCollection(this, new EmbeddedSourceCollection(new Uri("avares://Nlnet.Avalonia.Css.Fluent/Acss/Nlnet.Avalonia.Css.Fluent/Resources"), PreferLocalPath, UseRecommendedPreferSource, AutoExportSourceToLocal));
                loader.LoadCollection(this, new EmbeddedSourceCollection(new Uri("avares://Nlnet.Avalonia.Css.Fluent/Acss/Nlnet.Avalonia.Css.Fluent"), PreferLocalPath, UseRecommendedPreferSource, AutoExportSourceToLocal));
                loader.LoadCollection(this, new EmbeddedSourceCollection(new Uri("avares://Nlnet.Avalonia.Css.Fluent/Acss/Nlnet.Avalonia.Css.Fluent/Senior"), PreferLocalPath, UseRecommendedPreferSource, AutoExportSourceToLocal));
                loader.LoadCollection(this, new EmbeddedSourceCollection(new Uri("avares://Nlnet.Avalonia.Css.Fluent/Acss/Nlnet.Avalonia.Css.Fluent/MessageBox"), PreferLocalPath, UseRecommendedPreferSource, AutoExportSourceToLocal));
            }
        }

        public void UpdateThemeColor(bool reapplyStyle)
        {
            _accentColorFile?.Reload(reapplyStyle);
        }

        void ISupportInitialize.BeginInit()
        {
            
        }

        void ISupportInitialize.EndInit()
        {
            if (_suspendInitializeNotification)
            {
                return;
            }

            Load();
        }
    }
}
