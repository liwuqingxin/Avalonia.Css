using System;
using System.IO;
using System.Text;
using Avalonia.Platform;

namespace Nlnet.Avalonia.Css
{
    public class EmbeddedSource : SourceBase<Uri>
    {
        private readonly Uri _uri;

        public EmbeddedSource(Uri uri)
        {
            _uri = uri;
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
            
        }

        public override void Detach(IAcssContext context)
        {
            
        }



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
