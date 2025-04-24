using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrolleybusApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace TrolleybusApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly EmergencyService _emergencyService;
        private readonly TrolleybusEventLog _eventLog = TrolleybusEventLog.Instance;
        private int _nextTrolleybusId = 1;
        
        public ObservableCollection<Trolleybus> Trolleybuses { get; } = new ObservableCollection<Trolleybus>();
        public ObservableCollection<Driver> Drivers { get; } = new ObservableCollection<Driver>();
        public TrolleybusEventLog EventLog => _eventLog;
        
        public IRelayCommand AddTrolleybusWithDriverCommand { get; }
        public IRelayCommand<Trolleybus> RemoveTrolleybusCommand { get; }
        
        private readonly string[] _driverNames = { "Арпине Шхикян", "Николай Никонов", "Владимир Волков", 
                                                 "Иван Иванов", "Петр Петров", "Сергей Сергеев" };
        
        // Список имен водителей, которые уже были использованы
        private readonly List<string> _usedDriverNames = new List<string>();
        
        // Список имен водителей, которые были удалены и могут быть использованы повторно
        private readonly List<string> _availableDriverNames = new List<string>();

        public MainWindowViewModel()
        {
            _emergencyService = new EmergencyService();
            
            AddTrolleybusWithDriverCommand = new RelayCommand(AddTrolleybusWithDriver);
            RemoveTrolleybusCommand = new RelayCommand<Trolleybus>(RemoveTrolleybus, CanRemoveTrolleybus);

            _eventLog.AddEntry("Запуск симуляции троллейбусов");

            // Добавляем первый троллейбус с водителем
            AddTrolleybusWithDriver();
        }
        
        private void AddTrolleybusWithDriver()
        {
            string driverName;
            
            // Сначала проверяем, есть ли доступные водители, которые были удалены
            if (_availableDriverNames.Count > 0)
            {
                // Берем первого доступного водителя из списка удаленных
                driverName = _availableDriverNames[0];
                _availableDriverNames.RemoveAt(0);
            }
            else
            {
                // Если нет доступных удаленных водителей, берем следующего из основного списка
                var unusedDriverNames = _driverNames.Except(_usedDriverNames).ToList();
                if (unusedDriverNames.Count == 0)
                {
                    _eventLog.AddEntry("Невозможно добавить новый троллейбус: нет доступных водителей");
                    return;
                }
                
                driverName = unusedDriverNames[0];
                _usedDriverNames.Add(driverName);
            }
            
            var trolleybus = new Trolleybus(_nextTrolleybusId++);
            var driver = new Driver(driverName);
            
            trolleybus.OnBreakdown += _emergencyService.FixBreakdown;
            driver.AssignTrolleybus(trolleybus);
            
            Trolleybuses.Add(trolleybus);
            Drivers.Add(driver);
            
            _eventLog.AddEntry($"Добавлен троллейбус №{trolleybus.Id} с водителем {driver.Name}");
            
            trolleybus.StartMoving();
        }
        
        private void RemoveTrolleybus(Trolleybus? trolleybus)
        {
            if (trolleybus == null) return;
            
            var driver = trolleybus.CurrentDriver;
            if (driver != null)
            {
                // Добавляем имя водителя в список доступных для повторного использования
                _availableDriverNames.Add(driver.Name);
                Drivers.Remove(driver);
            }
            
            Trolleybuses.Remove(trolleybus);
            _eventLog.AddEntry($"Троллейбус №{trolleybus.Id} удален из системы");
        }
        
        private bool CanRemoveTrolleybus(Trolleybus? trolleybus)
        {
            return trolleybus != null && Trolleybuses.Count > 0;
        }
    }
}