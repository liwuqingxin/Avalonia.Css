using System;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// Define an acss source.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Source changed event.
        /// </summary>
        public event EventHandler<EventArgs> SourceChanged;

        /// <summary>
        /// Get the source content.
        /// </summary>
        /// <returns></returns>
        public string? GetSource();

        /// <summary>
        /// Check if this source is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid();

        /// <summary>
        /// Create a source from a path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="alignPathToThis"></param>
        /// <returns></returns>
        public ISource CreateFromPath(string path, bool alignPathToThis);

        /// <summary>
        /// Attach the source to the context.
        /// </summary>
        public void Attach(IAcssContext context);

        /// <summary>
        /// Detach from the context.
        /// </summary>
        /// <param name="context"></param>
        public void Detach(IAcssContext context);
    }
}
