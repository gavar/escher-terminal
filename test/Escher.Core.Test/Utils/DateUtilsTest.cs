using System;
using Xunit;

namespace Escher.Utils
{
    public class DateUtilsTest
    {
        [Theory]
        [MemberData(nameof(AgeFullYearsData))]
        public void AgeFullYears(DateTime dob, DateTime now, int expected)
        {
            var actual = DateUtils.AgeFullYears(dob, now);
            Assert.Equal(expected, actual);
        }

        public static TheoryData<DateTime, DateTime, int> AgeFullYearsData()
        {
            var now = DateTime.Now;
            var Y2019 = Date(2019, 1, 1);
            var Y2020 = Date(2020, 1, 1);

            return new TheoryData<DateTime, DateTime, int>
            {
                { now, now, 0 }, // now
                { Y2020, Y2019, 0 }, // future
                { Add(now, months: 1), now, 0 }, // future
                { Add(now, months: 13), now, 0 }, // future

                // < 1 year
                { Subtract(now, months: 1), now, 0 },
                { Subtract(now, months: 11), now, 0 },
                { Subtract(Y2020, millis: 1), Y2020, 0 },
                { Add(Y2019, days: 1), Y2020, 0 },

                // 1 year
                { Y2019, Y2020, 1 },
                { Subtract(now, months: 12), now, 1 },
                { Subtract(now, 1, 11), now, 1 },
                { Add(Y2019, hours: 23, minutes: 59, seconds: 59, millis: 999), Y2020, 1 },

                // leap year
                { Date(2016, 02, 29), Date(2020, 02, 28), 3 },
                { Date(2016, 02, 29), Date(2020, 02, 29), 4 },
                { Date(2016, 02, 29), Date(2020, 03, 1), 4 },
                { Date(2016, 03, 1), Date(2020, 03, 1), 4 },
            };
        }

        private static DateTime Date(int year, int month, int day)
        {
            return new DateTime(year, month, day);
        }

        private static DateTime Subtract(DateTime date,
            int years = 0, int months = 0, int days = 0, int hours = 0,
            int minutes = 0, int seconds = 0, int millis = 0)
        {
            var ms = date.Millisecond - millis;
            var ss = date.Second - seconds;
            var mm = date.Minute - minutes;
            var hh = date.Hour - hours;
            var d = date.Day - days;

            // slow but okay for tests
            while (ms < 0) Subtract(ref ms, ref ss, 1000);
            while (ss < 0) Subtract(ref ss, ref mm, 60);
            while (mm < 0) Subtract(ref mm, ref hh, 60);
            while (hh < 0) Subtract(ref hh, ref d, 24);

            var y = date.Year;
            var m = date.Month;

            // DAYS
            while (d < 1)
            {
                days = DateTime.DaysInMonth(y, m);
                Subtract(ref d, ref m, days);
                if (m < 1) Subtract(ref m, ref y, 12);
            }

            // MONTHS
            m -= months;
            while (m < 1) Subtract(ref m, ref y, 12);

            // YEARS
            y -= years;

            return new DateTime(y, m, d, hh, mm, ss, ms);
        }

        private static void Subtract(ref int minor, ref int major, int scale)
        {
            major--;
            minor += scale;
        }

        private static DateTime Add(DateTime date,
            int years = 0, int months = 0, int days = 0, int hours = 0,
            int minutes = 0, int seconds = 0, int millis = 0)
        {
            var ms = date.Millisecond + millis;
            var ss = date.Second + seconds;
            var mm = date.Minute + minutes;
            var hh = date.Hour + hours;
            var d = date.Day + days;

            // slow but okay for tests
            while (ms >= 1000) Add(ref ms, ref ss, 1000);
            while (ss >= 60) Add(ref ss, ref mm, 60);
            while (mm >= 60) Add(ref mm, ref hh, 60);
            while (hh >= 24) Add(ref hh, ref d, 24);

            var y = date.Year;
            var m = date.Month;

            // DAYS
            days = DateTime.DaysInMonth(y, m);
            while (d >= days)
            {
                Add(ref d, ref m, days);
                if (m >= 12) Add(ref m, ref y, 12);
                days = DateTime.DaysInMonth(y, m);
            }

            // MONTHS
            m += months;
            while (m >= 12) Add(ref m, ref y, 12);

            // YEARS
            y += years;

            return new DateTime(y, m, d, hh, mm, ss, ms);
        }

        private static void Add(ref int minor, ref int major, int scale)
        {
            major++;
            minor -= scale;
        }
    }
}