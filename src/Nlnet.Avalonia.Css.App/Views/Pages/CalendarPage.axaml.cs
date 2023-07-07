using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using JetBrains.Annotations;
using Nlnet.Avalonia.SampleAssistant;

namespace Nlnet.Avalonia.Css.App.Views.Pages
{
    [GalleryItem("Calendar")]
    public partial class CalendarPage : UserControl
    {
        public CalendarPage()
        {
            this.DataContext = new CalendarPageViewModel();

            InitializeComponent();
        }
    }

    public sealed class CalendarPageViewModel : INotifyPropertyChanged
    {
        private DateTimeOffset? _dateTime;
        private TimeSpan?       _timeSpan;

        public DateTimeOffset? DateTime
        {
            get => _dateTime;
            set
            {
                if (Nullable.Equals(value, _dateTime))
                    return;
                _dateTime = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan? Time
        {
            get => _timeSpan;
            set
            {
                if (Nullable.Equals(value, _timeSpan))
                    return;
                _timeSpan = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
