﻿using System;
using System.IO;
using System.Threading;

namespace Nlnet.Avalonia.Css
{
    public class FileSource : SourceBase<string>
    {
        private readonly string  _standardPath;
        private readonly string? _preferStandardPath;

        public FileSource(string path)
        {
            _standardPath = path.GetStandardPath();
        }

        public FileSource(string path, string preferPath)
        {
            _standardPath = path.GetStandardPath();
            _preferStandardPath = preferPath.GetStandardPath();
        }

        public override string GetKey()
        {
            if (string.IsNullOrEmpty(_preferStandardPath) == false && File.Exists(_preferStandardPath))
            {
                return _preferStandardPath;
            }
            return _standardPath;
        }

        public override string? GetSource()
        {
            var path = string.Empty;
            if (File.Exists(_preferStandardPath))
            {
                path = _preferStandardPath;
            }
            else if (File.Exists(_standardPath))
            {
                path = _standardPath;
            }
            else
            {
                this.WriteError($"Can not find acss file {path}. Skip it.");
                return null;
            }

            string acssSource;
            lock (this)
            {
                try
                {
                    acssSource = File.ReadAllText(path);
                }
                catch
                {
                    Thread.Sleep(20);
                    try
                    {
                        acssSource = File.ReadAllText(path);
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

        public override bool IsValid()
        {
            return File.Exists(_preferStandardPath) || File.Exists(_standardPath);
        }

        public override ISource CreateFromPath(string path, bool alignPathToThis)
        {
            if (alignPathToThis)
            {
                path = GetPathAlignToThis(path);
            }
            return new FileSource(path);
        }

        public override void Attach(IAcssContext context)
        {
            var monitor = context.GetService<IFileSourceMonitor>();
            monitor.Monitor(this);
        }

        public override void Detach(IAcssContext context)
        {
            var monitor = context.GetService<IFileSourceMonitor>();
            monitor.StopMonitor(this);
        }



        private string GetPathAlignToThis(string path)
        {
            if (File.Exists(path))
            {
                return path;
            }

            var currentKeyPath = GetKey();
            var dir = Path.GetDirectoryName(currentKeyPath);
            return string.IsNullOrEmpty(dir) ? path : Path.Combine(dir, path);
        }
    }
}
