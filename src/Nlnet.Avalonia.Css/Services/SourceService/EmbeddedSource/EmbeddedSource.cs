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

            try
            {
                using var stream = AssetLoader.Open(_uri);

                var bytes  = new byte[stream.Length];
                var length = stream.Read(bytes, 0, (int)stream.Length);

                // Regex can not recognize utf8-BOM.
                //var source = Encoding.UTF8.GetString(bytes);
                var source = EncodingHelper.BytesToText(bytes, Encoding.UTF8);
                
                return source;
            }
            catch (Exception e)
            {
                this.WriteError(e.ToString());
                return null;
            }
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
            return new EmbeddedSource(new Uri(path));
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

            var currentKeyPath = GetKey().AbsolutePath;
            var dir            = Path.GetDirectoryName(currentKeyPath);
            return string.IsNullOrEmpty(dir) ? path : Path.Combine(dir, path);
        }
    }
}
