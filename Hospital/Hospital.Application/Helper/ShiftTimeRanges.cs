using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Helper
{
    public static class ShiftTimeRanges
    {
        public static readonly Dictionary<AppointmentShift, (TimeSpan Start, TimeSpan End)> Shifts =
            new()
            {
                { AppointmentShift.Morning,   (TimeSpan.Parse("10:00"), TimeSpan.Parse("13:00")) },
                { AppointmentShift.Afternoon, (TimeSpan.Parse("14:00"), TimeSpan.Parse("17:00")) },
                { AppointmentShift.Evening,   (TimeSpan.Parse("18:00"), TimeSpan.Parse("21:00")) }
            };
        public static (TimeSpan Start, TimeSpan End) GetShiftRange(AppointmentShift shift)
        {
            if (!Shifts.ContainsKey(shift))
                throw new ArgumentException("Invalid shift");

            return Shifts[shift];
        }
    }
}
