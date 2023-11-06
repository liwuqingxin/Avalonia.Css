using System;
using System.Text;
using Avalonia.Platform;

namespace Nlnet.Avalonia.Css
{
    internal static class AssetsHelper
    {
        public static string? GetAssetOfString(this Uri uri)
        {
            try
            {
                using var stream = AssetLoader.Open(uri);

                var bytes  = new byte[stream.Length];
                var length = stream.Read(bytes, 0, (int)stream.Length);

                // Regex can not recognize utf8-BOM.
                //var source = Encoding.UTF8.GetString(bytes);
                var source = EncodingHelper.BytesToText(bytes, Encoding.UTF8);

                return source;
            }
            catch (Exception e)
            {
                typeof(AssetsHelper).WriteError(e.ToString());
                return null;
            }
        }
    }
}
