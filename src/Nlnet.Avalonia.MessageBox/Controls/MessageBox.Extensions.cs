using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Nlnet.Avalonia.Css;

namespace Nlnet.Avalonia.Controls
{
    public partial class MessageBox
    {
        #region Public Show Methods Async

        /// <summary>
        /// Show a modal message box over the active or main window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="defaultResult"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(
          string? message,
          string? caption,
          Buttons button,
          Images icon,
          Results defaultResult)
        {
            return SafeShowAsync(null, message, caption, button, icon, defaultResult);
        }

        /// <summary>
        /// Show a modal message box over the active or main window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(
          string? message,
          string? caption,
          Buttons button,
          Images icon)
        {
            return SafeShowAsync(null, message, caption, button, icon, Results.None);
        }

        /// <summary>
        /// Show a modal message box over the active or main window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(
          string? message,
          string? caption,
          Buttons button)
        {
            return SafeShowAsync(null, message, caption, button, Images.Info, Results.None);
        }

        /// <summary>
        /// Show a modal message box over the active or main window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(
            string? message,
            string? caption)
        {
            return SafeShowAsync(null, message, caption, Buttons.OK, Images.None, Results.None);
        }

        /// <summary>
        /// Show a modal message box over the active or main window. Note that the message
        /// will be shown at the location of caption.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(
            string? message)
        {
            return SafeShowAsync(null, string.Empty, message, Buttons.OK, Images.None, Results.None);
        }

        /// <summary>
        /// Show a modal message box over the owner.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="defaultResult"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(
            Window owner,
            string? message,
            string? caption,
            Buttons button,
            Images icon,
            Results defaultResult)
        {
            return SafeShowAsync(owner, message, caption, button, icon, defaultResult);
        }

        /// <summary>
        /// Show a modal message box over the owner.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(
            Window owner,
            string? message,
            string? caption,
            Buttons button,
            Images icon)
        {
            return SafeShowAsync(owner, message, caption, button, icon, Results.None);
        }

        /// <summary>
        /// Show a modal message box over the owner.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(
            Window owner,
            string? message,
            string? caption,
            Buttons button)
        {
            return SafeShowAsync(owner, message, caption, button, Images.Info, Results.None);
        }

        /// <summary>
        /// Show a modal message box over the owner.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(
            Window owner,
            string? message,
            string? caption)
        {
            return SafeShowAsync(owner, message, caption, Buttons.OK, Images.None, Results.None);
        }

        /// <summary>
        /// Show a modal message box over the owner. Note that the message will be shown
        /// at the location of caption.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(
            Window owner,
            string? message)
        {
            return SafeShowAsync(owner, string.Empty, message, Buttons.OK, Images.None, Results.None);
        }

        #endregion



        #region Show Exception Async

        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(Exception exception, string? caption, string? message)
        {
            var realMessage = string.IsNullOrEmpty(message) ? string.Empty : $"{message}{Environment.NewLine}";
            return MessageBox.SafeShowAsync(null, $"{realMessage}{Environment.NewLine}{BuildMessage(exception, false, false, false)}", caption, Buttons.OK, Images.Error, Results.None);
        }

        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(Exception exception, string? caption)
        {
            return MessageBox.SafeShowAsync(null, BuildMessage(exception), caption, Buttons.OK, Images.Error, Results.None);
        }

        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Task<Results> ShowAsync(Exception exception)
        {
            return MessageBox.SafeShowAsync(null, BuildMessage(exception, false, false, false), "EXCEPTION NOTIFICATION", Buttons.OK, Images.Error, Results.None);
        }
        
        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task<Results> ShowHighlightAsync(Exception exception, string? caption, string? message)
        {
            var realMessage = string.IsNullOrEmpty(message) ? string.Empty : $"{message}{Environment.NewLine}";
            return MessageBox.SafeShowAsync(null, $"{realMessage}{Environment.NewLine}{BuildMessage(exception, false, false, false)}", caption, Buttons.OK, Images.Error, Results.None);
        }

        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static Task<Results> ShowHighlightAsync(Exception exception, string? caption)
        {
            return MessageBox.SafeShowAsync(null, BuildMessage(exception), caption, Buttons.OK, Images.Error, Results.None);
        }

        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Task<Results> ShowHighlightAsync(Exception exception)
        {
            return MessageBox.SafeShowAsync(null, BuildMessage(exception), "EXCEPTION NOTIFICATION", Buttons.OK, Images.Error, Results.None);
        }

        #endregion
        
        
        
        #region Public Show Methods
        
        /// <summary>
        /// Show a modal message box over the active or main window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="defaultResult"></param>
        /// <returns></returns>
        public static Results Show(
          string? message,
          string? caption,
          Buttons button,
          Images icon,
          Results defaultResult)
        {
            return SafeShow(null, message, caption, button, icon, defaultResult);
        }
        
        /// <summary>
        /// Show a modal message box over the active or main window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static Results Show(
          string? message,
          string? caption,
          Buttons button,
          Images icon)
        {
            return SafeShow(null, message, caption, button, icon, Results.None);
        }
        
        /// <summary>
        /// Show a modal message box over the active or main window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public static Results Show(
          string? message,
          string? caption,
          Buttons button)
        {
            return SafeShow(null, message, caption, button, Images.Info, Results.None);
        }
        
        /// <summary>
        /// Show a modal message box over the active or main window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static Results Show(
            string? message,
            string? caption)
        {
            return SafeShow(null, message, caption, Buttons.OK, Images.None, Results.None);
        }
        
        /// <summary>
        /// Show a modal message box over the active or main window. Note that the message
        /// will be shown at the location of caption.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Results Show(
            string? message)
        {
            return SafeShow(null, string.Empty, message, Buttons.OK, Images.None, Results.None);
        }
        
        /// <summary>
        /// Show a modal message box over the owner.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="defaultResult"></param>
        /// <returns></returns>
        public static Results Show(
            Window owner,
            string? message,
            string? caption,
            Buttons button,
            Images icon,
            Results defaultResult)
        {
            return SafeShow(owner, message, caption, button, icon, defaultResult);
        }
        
        /// <summary>
        /// Show a modal message box over the owner.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static Results Show(
            Window owner,
            string? message,
            string? caption,
            Buttons button,
            Images icon)
        {
            return SafeShow(owner, message, caption, button, icon, Results.None);
        }
        
        /// <summary>
        /// Show a modal message box over the owner.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public static Results Show(
            Window owner,
            string? message,
            string? caption,
            Buttons button)
        {
            return SafeShow(owner, message, caption, button, Images.Info, Results.None);
        }
        
        /// <summary>
        /// Show a modal message box over the owner.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static Results Show(
            Window owner,
            string? message,
            string? caption)
        {
            return SafeShow(owner, message, caption, Buttons.OK, Images.None, Results.None);
        }
        
        /// <summary>
        /// Show a modal message box over the owner. Note that the message will be shown
        /// at the location of caption.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Results Show(
            Window owner,
            string? message)
        {
            return SafeShow(owner, string.Empty, message, Buttons.OK, Images.None, Results.None);
        }
        
        #endregion
        
        
        
        #region Show Exception
        
        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Results Show(Exception exception, string? caption, string? message)
        {
            var realMessage = string.IsNullOrEmpty(message) ? string.Empty : $"{message}{Environment.NewLine}";
            return MessageBox.SafeShow(null, $"{realMessage}{Environment.NewLine}{BuildMessage(exception, false, false, false)}", caption, Buttons.OK, Images.Error, Results.None);
        }
        
        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static Results Show(Exception exception, string? caption)
        {
            return MessageBox.SafeShow(null, BuildMessage(exception), caption, Buttons.OK, Images.Error, Results.None);
        }
        
        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Results Show(Exception exception)
        {
            return MessageBox.SafeShow(null, BuildMessage(exception, false, false, false), "EXCEPTION NOTIFICATION", Buttons.OK, Images.Error, Results.None);
        }
        
        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Results ShowHighlight(Exception exception, string? caption, string? message)
        {
            var realMessage = string.IsNullOrEmpty(message) ? string.Empty : $"{message}{Environment.NewLine}";
            return MessageBox.SafeShow(null, $"{realMessage}{Environment.NewLine}{BuildMessage(exception, false, false, false)}", caption, Buttons.OK, Images.Error, Results.None);
        }
        
        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static Results ShowHighlight(Exception exception, string? caption)
        {
            return MessageBox.SafeShow(null, BuildMessage(exception), caption, Buttons.OK, Images.Error, Results.None);
        }
        
        /// <summary>
        /// Show exception in a MessageBox.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Results ShowHighlight(Exception exception)
        {
            return MessageBox.SafeShow(null, BuildMessage(exception), "EXCEPTION NOTIFICATION", Buttons.OK, Images.Error, Results.None);
        }
        
        #endregion



        #region Core Methods for Show

        private static string BuildMessage(Exception? e, bool highlight = true, bool highlightFile = true, bool highlightLine = false)
        {
            var builder = new StringBuilder();

            while (e != null)
            {
#if DEBUG
                // Message
                var title = e.Message
                    .Replace("[", "(")
                    .Replace("]", ")")
                    .B()
                    .Red()
                    .Size(18);
                builder.AppendLine(title);

                // Exception type
                var type = e.GetType().Name
                    .Size(16);
                builder.AppendLine(type);

                // Stack trace only in debug mode.
                var stackTrace = e.StackTrace;
                if (stackTrace != null)
                {
#pragma warning disable CS0618
                    if (highlightFile)
                    {
                        stackTrace = stackTrace
                            .B(new Regex("[a-zA-Z]+[\\.g]*\\.cs"))
                            .Green(new Regex("[a-zA-Z]+[\\.g]*\\.cs"))
                            .B(new Regex("[a-zA-Z]+\\.xaml"))
                            .DeepSkyBlue(new Regex("[a-zA-Z]+\\.[a]?xaml"));
                    }
                    if (highlightLine)
                    {
                        stackTrace = stackTrace
                            .Orange(new Regex(":line [0-9]+"));
                    }
#pragma warning restore CS0618
                }

                builder.AppendLine(stackTrace);
#else
                // Message
                var title = $"● {e.Message}";
                builder.AppendLine(title);
#endif
                e = e.InnerException;
            }

            return builder.ToString();
        }
        
        private static async Task<Results> SafeShowAsync(
            Window? owner,
            string? message,
            string? caption,
            Buttons button,
            Images icon,
            Results defaultResult)
        {
            // Do nothing if application is almost to close.
            if (Application.Current == null)
            {
                return Results.None;
            }

            if (Application.Current.CheckAccess())
            {
                return await ShowCoreAsync(owner, message, caption, button, icon, defaultResult);
            }
            else
            {
                return await global::Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => ShowCoreAsync(owner, message, caption, button, icon, defaultResult));
            }
        }

        private static async Task<Results> ShowCoreAsync(
            Window? owner,
            string? message,
            string? caption,
            Buttons button,
            Images icon,
            Results defaultResult)
        {
            owner ??= WindowsVisitor.GetActiveOrMainWindow();
            if (owner == null)
            {
                throw new Exception($"{nameof(MessageBox)} can not be shown because the owner is null and the application can not find a active window or a main window as the owner");
            }

            var box = new MessageBox
            {
                Owner = owner,
                Message = message,
                Title = caption,
                Buttons = button,
                Image = icon,
                UseMask = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            if (owner.Topmost)
            {
                box.Topmost = true;
            }

            box.Icon = owner.Icon;
            
            await box.ShowDialog(owner);

            // If the result of MessageBox is None, return the default result.
            return box.Result == Results.None ? defaultResult : box.Result;
        }
        
        private static Results SafeShow(
            Window? owner,
            string? message,
            string? caption,
            Buttons button,
            Images icon,
            Results defaultResult)
        {
            // Do nothing if application is almost to close.
            if (Application.Current == null)
            {
                return Results.None;
            }

            if (Application.Current.CheckAccess())
            {
                return ShowCore(owner, message, caption, button, icon, defaultResult);
            }
            else
            {
                return Dispatcher.UIThread.InvokeAsync(() => ShowCore(owner, message, caption, button, icon, defaultResult)).Result;
            }
        }

        private static Results ShowCore(
            Window? owner,
            string? message,
            string? caption,
            Buttons button,
            Images icon,
            Results defaultResult)
        {
            owner ??= WindowsVisitor.GetActiveOrMainWindow();
            if (owner == null)
            {
                throw new Exception($"{nameof(MessageBox)} can not be shown because the owner is null and the application can not find a active window or a main window as the owner");
            }

            var box = new MessageBox
            {
                Owner = owner,
                Message = message,
                Title = caption,
                Buttons = button,
                Image = icon,
                UseMask = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            if (owner.Topmost)
            {
                box.Topmost = true;
            }

            box.Icon = owner.Icon;
            
            var frame = new DispatcherFrame();
            box.ShowDialog(owner).ContinueWith(t =>
            {
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);

            // If the result of MessageBox is None, return the default result.
            return box.Result == Results.None ? defaultResult : box.Result;
        }

        #endregion
    }
}