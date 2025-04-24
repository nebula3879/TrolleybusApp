using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TrolleybusApp.Models
{
    public class EventLogEntry
    {
        public DateTime Timestamp { get; }
        public string Message { get; }

        public EventLogEntry(string message)
        {
            Timestamp = DateTime.Now;
            Message = message;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {Message}";
        }
    }

    public class TrolleybusEventLog : ObservableObject
    {
        private static TrolleybusEventLog? _instance;
        public static TrolleybusEventLog Instance => _instance ??= new TrolleybusEventLog();

        public ObservableCollection<EventLogEntry> Entries { get; } = new ObservableCollection<EventLogEntry>();

        private TrolleybusEventLog()
        {
        }

        public void AddEntry(string message)
        {
            var entry = new EventLogEntry(message);
            
            // Добавляем запись в UI потоке
            Dispatcher.UIThread.Post(() =>
            {
                Entries.Add(entry);
            });
        }
    }
} 