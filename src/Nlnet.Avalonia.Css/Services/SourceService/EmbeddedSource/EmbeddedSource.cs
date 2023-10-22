using System;
using System.IO;
using Avalonia.Platform;

namespace Nlnet.Avalonia.Css
{
    public class EmbeddedSource : SourceBase<Uri>, IPreferSourceOwner
    {
        private readonly Uri _uri;

        public EmbeddedSource(Uri uri)
        {
            _uri = uri;
        }

        public EmbeddedSource(Uri uri, string? preferLocalSource, bool useRecommendedPreferSource) 
            : this(uri)
        {
            var source = preferLocalSource == null
                ? null
                : new FileSource(Path.Combine(preferLocalSource, $".{uri.AbsolutePath}"));

            PreferSource = source;
            if (preferLocalSource == null && useRecommendedPreferSource)
            {
                UseRecommendedPreferSource();
            }
        }

        public EmbeddedSource(Uri uri, string? preferLocalSource, bool useRecommendedPreferSource, bool autoExportSourceToLocal)
            : this(uri, preferLocalSource, useRecommendedPreferSource)
        {
            if (autoExportSourceToLocal)
            {
                ExportToLocal();
            }
        }

        private void ExportToLocal()
        {
            if (PreferSource is not FileSource fileSource)
            {
                return;
            }

            var keyPath = fileSource.GetKey();
            if (File.Exists(keyPath))
            {
                return;
            }

            try
            {
                var dir = Path.GetDirectoryName(keyPath);
                if (string.IsNullOrEmpty(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(keyPath, GetSource());
            }
            catch (Exception e)
            {
                this.WriteLine(e.ToString());
            }
        }



        public override Uri GetKey()
        {
            return _uri;
        }

        public sealed override string? GetSource()
        {
            if (IsValid() == false)
            {
                return null;
            }

            if (PreferSource != null && PreferSource.IsValid())
            {
                return PreferSource.GetSource();
            }

            return _uri.GetAssetOfString();
        }

        public override bool IsValid()
        {
            return AssetLoader.Exists(_uri);
        }

        public override ISource CreateFromPath(string path, bool alignPathToThis)
        {
            if (alignPathToThis)
            {
                path = GetPathAlignToThis(path);
            }
            return new EmbeddedSource(new Uri(path, UriKind.Absolute));
        }

        public override void Attach(IAcssContext context)
        {
            if (PreferSource != null && PreferSource.IsValid())
            {
                PreferSource.Attach(context);
                PreferSource.SourceChanged += PreferSourceOnSourceChanged;
            }
        }

        public override void Detach(IAcssContext context)
        {
            if (PreferSource != null && PreferSource.IsValid())
            {
                PreferSource.Detach(context);
                PreferSource.SourceChanged -= PreferSourceOnSourceChanged;
            }
        }

        private void PreferSourceOnSourceChanged(object? sender, EventArgs e)
        {
            this.OnSourceChanged();
        }



        #region IPreferSourceOwner

        public ISource? PreferSource { get; set; }

        public void UseRecommendedPreferSource()
        {
            var keyPath = GetKey().AbsolutePath;
            PreferSource = new FileSource($".{keyPath}");
        }

        #endregion



        private string GetPathAlignToThis(string path)
        {
            if (File.Exists(path))
            {
                return path;
            }

            var currentKeyPath = GetKey().AbsoluteUri;
            var dir            = Path.GetDirectoryName(currentKeyPath);
            return string.IsNullOrEmpty(dir) ? path : Path.Combine(dir, path).Replace('\\', '/').Replace("avares:/", "avares://");
        }
    }
}
