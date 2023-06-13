using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Nlnet.Avalonia.Behaviors
{
    public class ComboBoxAlignPopupToSelectionBehavior : Behavior<ComboBox>
    {
        private double _horizontalOffset;
        private double _verticalOffset;
        private bool   _isAttached;

        protected override void OnAttached()
        {
            if (this.AssociatedObject == null)
            {
                return;
            }

            if (this.AssociatedObject.IsLoaded)
            {
                AttachToComboBox();
            }
            else
            {
                this.AssociatedObject.Loaded += OnAssociatedObjectOnLoaded;
            }
        }

        private void OnAssociatedObjectOnLoaded(object? sender, RoutedEventArgs args)
        {
            this.AssociatedObject!.Loaded -= OnAssociatedObjectOnLoaded;
            
            AttachToComboBox();
        }

        private void AttachToComboBox()
        {
            var popup = this.AssociatedObject?.FindDescendantOfType<Popup>();
            if (popup == null || _isAttached)
            {
                return;
            }

            if (_horizontalOffset == 0)
            {
                _horizontalOffset = popup.HorizontalOffset;
            }
            if (_verticalOffset == 0)
            {
                _verticalOffset = popup.VerticalOffset;
            }

            _isAttached = true;

            popup.Opened += Popup_Opened;
        }

        protected override void OnDetaching()
        {
            var popup = this.AssociatedObject?.FindDescendantOfType<Popup>();
            if (popup == null)
            {
                return;
            }

            _isAttached = false;

            popup.Opened -= Popup_Opened;
        }

        private void Popup_Opened(object? sender, EventArgs e)
        {
            var comboBox  = AssociatedObject!;
            var popup     = (Popup) sender!;
            var popupRoot = popup.Child!;

            var index = comboBox.SelectedIndex;
            if (index == -1 && comboBox.ItemCount > 0)
            {
                index = 0;
            }
            if (index == -1)
            {
                return;
            }

            var container = comboBox.ItemContainerGenerator.ContainerFromIndex(index);
            var matrix    = container?.TransformToVisual(popupRoot);
            if (matrix == null)
            {
                return;
            }

            var point = new Point().Transform(matrix.Value);

            var toOffset   = -(comboBox.Bounds.Height + point.Y) + _verticalOffset;
            var fromOffset = toOffset;
            //if (popup.VerticalOffset > toOffset)
            //{
            //    fromOffset += 4;
            //}
            //else if(popup.VerticalOffset < toOffset)
            //{
            //    fromOffset -= 4;
            //}
            var transitions = popup.Transitions;
            popup.Transitions      = null;
            popup.VerticalOffset   = fromOffset;
            popup.Transitions      = transitions;
            popup.VerticalOffset   = toOffset;
            popup.HorizontalOffset = -point.X + _horizontalOffset;
        }
    }
}
