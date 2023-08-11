using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Nlnet.Avalonia.Css;

// ReSharper disable SuspiciousTypeConversion.Global

// ReSharper disable InconsistentNaming

namespace Nlnet.Avalonia.Controls
{
    public partial class MessageBox : Window
    {
        protected override Type StyleKeyOverride { get; } = typeof(MessageBox);

        private const string PART_BtnClose  = nameof(PART_BtnClose);
        private const string PART_BtnYes    = nameof(PART_BtnYes);
        private const string PART_BtnNo     = nameof(PART_BtnNo);
        private const string PART_BtnCancel = nameof(PART_BtnCancel);



        public string? Message
        {
            get { return GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly StyledProperty<string?> MessageProperty = AvaloniaProperty
            .Register<MessageBox, string?>(nameof(Message));

        public Results Result
        {
            get { return GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }
        public static readonly StyledProperty<Results> ResultProperty = AvaloniaProperty
            .Register<MessageBox, Results>(nameof(Result));

        public Buttons Buttons
        {
            get { return GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }
        public static readonly StyledProperty<Buttons> ButtonsProperty = AvaloniaProperty
            .Register<MessageBox, Buttons>(nameof(Buttons), Buttons.OK);

        public Images Image
        {
            get { return GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly StyledProperty<Images> ImageProperty = AvaloniaProperty
            .Register<MessageBox, Images>(nameof(Image));

        public bool UseMask
        {
            get { return GetValue(UseMaskProperty); }
            set { SetValue(UseMaskProperty, value); }
        }
        public static readonly StyledProperty<bool> UseMaskProperty = AvaloniaProperty
            .Register<MessageBox, bool>(nameof(UseMask));



        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            this.BeginMoveDrag(e);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            var btnClose = e.NameScope.Find<Button>(PART_BtnClose);
            if (btnClose != null)
            {
                btnClose.Click += (sender, args) =>
                {
                    this.Close();
                };
            }

            var btnYes = e.NameScope.Find<Button>(PART_BtnYes);
            if (btnYes != null)
            {
                btnYes.Click += (sender, args) =>
                {
                    this.Result = this.Buttons switch
                    {
                        Buttons.YesNo or Buttons.YesNoCancel => Results.Yes,
                        Buttons.OK or Buttons.OKCancel       => Results.OK,
                        _                                    => this.Result
                    };
                    this.Close();
                };
            }

            var btnNo = e.NameScope.Find<Button>(PART_BtnNo);
            if (btnNo != null)
            {
                btnNo.Click += (sender, args) =>
                {
                    this.Result = Results.No;
                    this.Close();
                };
            }

            var btnCancel = e.NameScope.Find<Button>(PART_BtnCancel);
            if (btnCancel != null)
            {
                btnCancel.Click += (sender, args) =>
                {
                    this.Result = Results.Cancel;
                    this.Close();
                };
            }
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

// #if DEBUG
//             this.AttachDevTools(new DevToolsOptions()
//             {
//                 StartupScreenIndex = 1,
//             });
// #endif

            if (this.UseMask && this.Owner is IMaskable maskable)
            {
                maskable.ShowMask();
            }
            
            PlatformService.FixScalingInLinux(this);
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);

            if (this.UseMask && this.Owner is IMaskable maskable)
            {
                maskable.HideMask();
            }
        }
    }
}