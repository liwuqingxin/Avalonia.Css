using System;
using Avalonia;

namespace Nlnet.Avalonia.Css
{
    没有生效：看Combobox.acss; 且这里需要整理，使用代码生成；
    public partial class Acss : IBehaviorDeclarer
    {
        public static AcssBehavior GetComboBoxPopupAlignBehavior(Visual host)
        {
            return host.GetValue(ComboBoxPopupAlignBehaviorProperty);
        }
        public static void SetComboBoxPopupAlignBehavior(Visual host, AcssBehavior value)
        {
            host.SetValue(ComboBoxPopupAlignBehaviorProperty, value);
        }
        public static readonly AttachedProperty<AcssBehavior> ComboBoxPopupAlignBehaviorProperty = AvaloniaProperty
            .RegisterAttached<Acss, Visual, AcssBehavior>("ComboBoxPopupAlignBehavior");
    }

    public partial class Acss : AvaloniaObject
    {
        private static void OnInstanceChanged(AvaloniaPropertyChangedEventArgs<AcssBehavior> args)
        {
            if (args.OldValue is { HasValue: true, Value: not null })
            {
                args.OldValue.Value.Detach(args.Sender);
            }
            if (args.NewValue is { HasValue: true, Value: not null })
            {
                args.NewValue.Value.Attach(args.Sender);
            }
        }

        static Acss()
        {
            ComboBoxPopupAlignBehaviorProperty.Changed.Subscribe(OnInstanceChanged);
        }
    }
}
