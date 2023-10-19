using System;
using System.IO;
using System.Text;
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

        public EmbeddedSource(Uri uri, ISource preferSource)
        {
            _uri = uri;
            PreferSource = preferSource;
        }

        public EmbeddedSource(Uri uri, bool useRecommendedPreferSource)
        {
            _uri = uri;
            if (useRecommendedPreferSource)
            {
                UseRecommendedPreferSource();
            }
        }



        public override Uri GetKey()
        {
            return _uri;
        }

        public override string? GetSource()
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
