using System;

namespace StudentClinicMIS.Extensions
{
    public static class DateOnlyExtensions
    {
        public static DateTime ToDateTime(this DateOnly dateOnly, TimeOnly timeOnly)
        {
            return new DateTime(
                dateOnly.Year,
                dateOnly.Month,
                dateOnly.Day,
                timeOnly.Hour,
                timeOnly.Minute,
                timeOnly.Second
            );
        }

        // Дополнительные методы при необходимости
        public static DateOnly ToDateOnly(this DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }
    }
}