using System;

namespace Escher.Utils
{
    public static class DateUtils
    {
        public static int AgeFullYears(DateTime dob)
        {
            return AgeFullYears(dob, DateTime.Now);
        }

        public static int AgeFullYears(DateTime dob, DateTime now)
        {
            var years = now.Year - dob.Year;
            if (years < 1) return 0;

            if (dob.Month > now.Month)
                return years - 1;

            if (dob.Month == now.Month && dob.Day > now.Day)
                return years - 1;

            return years;
        }
    }
}