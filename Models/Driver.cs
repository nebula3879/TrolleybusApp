#nullable enable
using System;
using System.Threading.Tasks;
using TrolleybusApp.Models;

namespace TrolleybusApp.Models
{
    public class Driver
    {
        private readonly TrolleybusEventLog _eventLog = TrolleybusEventLog.Instance;
        private Trolleybus? _currentTrolleybus;
        private readonly Random _random = new Random();
        
        public string Name { get; }
        public Trolleybus? CurrentTrolleybus => _currentTrolleybus;
        
        public Driver(string name)
        {
            Name = name;
        }
        
        public void AssignTrolleybus(Trolleybus trolleybus)
        {
            _currentTrolleybus = trolleybus;
            trolleybus.CurrentDriver = this;
            _eventLog.AddEntry($"Водитель {Name} назначен на троллейбус №{trolleybus.Id}");
        }
        
        public async Task DriveRoute()
        {
            if (_currentTrolleybus == null)
            {
                _eventLog.AddEntry($"Водитель {Name} не может начать маршрут: не назначен троллейбус");
                return;
            }
            
            _eventLog.AddEntry($"Водитель {Name} начинает маршрут на троллейбусе №{_currentTrolleybus.Id}");
            
            // Симуляция движения по маршруту
            await Task.Delay(5000);
            
            // Случайная вероятность поломки
            if (_random.Next(100) < 20) // 20% шанс поломки
            {
                _currentTrolleybus.Breakdown();
            }
            
            _eventLog.AddEntry($"Водитель {Name} завершил маршрут на троллейбусе №{_currentTrolleybus.Id}");
            
            // Начинаем новый маршрут
            if (_currentTrolleybus.Status == TrolleybusStatus.Ready)
            {
                await Task.Delay(2000); // Небольшая пауза между маршрутами
                await DriveRoute();
            }
        }
    }
}