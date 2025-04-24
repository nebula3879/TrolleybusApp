using System;
using System.Threading.Tasks;

namespace TrolleybusApp.Models
{
    public class EmergencyService : IEmergencyService
    {
        private readonly TrolleybusEventLog _eventLog = TrolleybusEventLog.Instance;
        
        public async void FixBreakdown(Trolleybus trolleybus)
        {
            _eventLog.AddEntry($"Аварийная служба вызвана для троллейбуса №{trolleybus.Id}");
            
            await Task.Delay(3000);

            trolleybus.Fix();
            _eventLog.AddEntry($"Аварийная служба починила троллейбус №{trolleybus.Id}");
        }
    }
}