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
        /// <see cref="SourceChanged"/> event should be fired in this method.
        /// </summary>
        public void OnSourceChanged();

        /// <summary>
        /// Get the key path of the source. It is unique for a source.
        /// </summary>
        /// <returns></returns>
        public string GetKeyPath();

        /// <summary>
        /// Get the source content.
        /// </summary>
        /// <returns></returns>
        public string? GetSource();

        /// <summary>
        /// Check if this source is valid.
        /// </summary>
        /// <returns></returns>
        bool IsValid();

        /// <summary>
        /// Create a source from a path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        ISource CreateFromPath(string path);

        /// <summary>
        /// The source attached to the context.
        /// </summary>
        void Attached(IAcssContext context);
    }
}
