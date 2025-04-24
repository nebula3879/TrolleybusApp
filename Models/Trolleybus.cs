using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace TrolleybusApp.Models
{
    public class Trolleybus : INotifyPropertyChanged
    {
        private readonly TrolleybusEventLog _eventLog = TrolleybusEventLog.Instance;
        
        public int Id { get; }
        private bool _isBroken;
        private bool _arePolesOff;
        public Driver? CurrentDriver { get; set; }

        public bool IsBroken
        {
            get => _isBroken;
            private set
            {
                if (_isBroken != value)
                {
                    _isBroken = value;
                    OnPropertyChanged(nameof(IsBroken));
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public bool ArePolesOff
        {
            get => _arePolesOff;
            private set
            {
                if (_arePolesOff != value)
                {
                    _arePolesOff = value;
                    OnPropertyChanged(nameof(ArePolesOff));
                    OnPropertyChanged(nameof(Status));
                    
                    if (value && CurrentDriver != null)
                    {
                        // Если штанги соскочили, водитель автоматически их исправит через некоторое время
                        Task.Run(async () => 
                        {
                            await Task.Delay(3000);
                            if (ArePolesOff) // Проверяем, что штанги все еще сняты
                            {
                                _eventLog.AddEntry($"Водитель {CurrentDriver.Name} начал устанавливать штанги на троллейбусе №{Id}");
                                await Task.Delay(2000);
                                FixPoles();
                                _eventLog.AddEntry($"Водитель {CurrentDriver.Name} установил штанги на троллейбусе №{Id}");
                                
                                // Если троллейбус готов к работе, продолжаем маршрут
                                if (Status == TrolleybusStatus.Ready && CurrentDriver != null)
                                {
                                    await Task.Delay(1000);
                                    await CurrentDriver.DriveRoute();
                                }
                            }
                        });
                    }
                }
            }
        }

        public TrolleybusStatus Status
        {
            get
            {
                if (IsBroken) return TrolleybusStatus.Broken;
                if (ArePolesOff) return TrolleybusStatus.PolesOff;
                return TrolleybusStatus.Ready;
            }
        }

        public event Action<Trolleybus>? OnBreakdown;
        public event Action<Trolleybus>? OnPolesOff;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Trolleybus(int id)
        {
            Id = id;
            IsBroken = false;
            ArePolesOff = false;
            
            _eventLog.AddEntry($"Создан троллейбус №{Id}");
        }

        public void Breakdown()
        {
            if (!IsBroken)
            {
                IsBroken = true;
                _eventLog.AddEntry($"Троллейбус №{Id} сломался");
                OnBreakdown?.Invoke(this);
            }
        }

        public void StartMoving()
        {
            _eventLog.AddEntry($"Троллейбус №{Id} начал движение");
            
            // Запускаем первый маршрут
            if (CurrentDriver != null)
            {
                Task.Run(async () => 
                {
                    await Task.Delay(1000);
                    await CurrentDriver.DriveRoute();
                });
            }
            
            // Запускаем симуляцию случайных событий
            Task.Run(() =>
            {
                Random random = new Random();
                while (true)
                {
                    Thread.Sleep(2000); // Имитация движения
                    if (random.Next(100) < 10) // 10% вероятность поломки
                    {
                        if (!IsBroken)
                        {
                            Breakdown();
                        }
                    }
                    if (random.Next(100) < 10) // 10% вероятность соскакивания штанг
                    {
                        if (!ArePolesOff)
                        {
                            ArePolesOff = true;
                            _eventLog.AddEntry($"У троллейбуса №{Id} соскочили штанги");
                            OnPolesOff?.Invoke(this);
                        }
                    }
                }
            });
        }

        public void Fix()
        {
            IsBroken = false;
            _eventLog.AddEntry($"Троллейбус №{Id} починен");
            
            // Если троллейбус полностью исправен, продолжаем маршрут
            if (Status == TrolleybusStatus.Ready && CurrentDriver != null)
            {
                Task.Run(async () => 
                {
                    await Task.Delay(1000);
                    await CurrentDriver.DriveRoute();
                });
            }
        }

        public void FixPoles()
        {
            ArePolesOff = false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}