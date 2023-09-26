using System;
using System.IO;
using System.Threading;

namespace Nlnet.Avalonia.Css
{
    public class FileSource : SourceBase
    {
        private readonly string _standardPath;

        public FileSource(string path)
        {
            _standardPath = path.GetStandardPath();
        }

        public override string GetKeyPath()
        {
            return _standardPath;
        }

        public override string? GetSource()
        {
            if (File.Exists(_standardPath) == false)
            {
                return null;
            }

            string acssSource;
            lock (this)
            {
                try
                {
                    acssSource = File.ReadAllText(_standardPath);
                }
                catch
                {
                    Thread.Sleep(20);
                    try
                    {
                        acssSource = File.ReadAllText(_standardPath);
                    }
                    catch (Exception exception)
                    {
                        this.WriteError(exception.ToString());
                        return null;
                    }
                }
            }

            return acssSource;
        }

        public override ISource CreateFromPath(string path)
        {
            return new FileSource(path);
        }
    }
}
