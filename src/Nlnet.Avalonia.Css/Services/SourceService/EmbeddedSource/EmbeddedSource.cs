//using System;

//namespace Nlnet.Avalonia.Css
//{
//    public class EmbeddedSource : SourceBase
//    {
//        private readonly Uri _uri;
//        private readonly Uri? _preferUri;

//        public EmbeddedSource(Uri uri)
//        {
//            _uri = uri;
//        }

//        public EmbeddedSource(Uri uri, Uri preferUri)
//        {
//            _uri = uri;
//            _preferUri = preferUri;
//        }

//        private Uri GetEffectiveUri()
//        {
//            return _preferUri == null ? _uri : _preferUri;
//        }

//        public override string GetKeyPath()
//        {
//            var uri = GetEffectiveUri();
//            return uri.
//        }

//        public override string? GetSource()
//        {
            
//        }

//        public override bool IsValid()
//        {
            
//        }

//        public override ISource CreateFromPath(string path)
//        {
            
//        }

//        public override void Attach(IAcssContext context)
//        {
//            throw new System.NotImplementedException();
//        }

//        public override void Detach(IAcssContext context)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}
