using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P019_CountingSundays : RunableBase
    {
        int firstMondayDay = 1;
        int firstMondayMonth = 1;
        int firstMondayYear = 1900;

        [Run(0)]
        public void SolveIt()
        {
            int yearStart = 1901;
            int yearEnd = 2000;

            int daysCounter = daysToFirstMonday(31, 12, yearStart - 1);
            int sundays = 0;

            for (int year = yearStart; year <= yearEnd; year++)
            {
                for (int month = 1; month < 13; month++)
                {
                    if ((daysCounter + 1) % 7 == 0)
                        sundays++;
                    daysCounter += daysInMonth(month, year);
                }
            }

            DebugFormat("Sundays: {0}", sundays);
        }

        private int daysInMonth(int month, int year)
        {
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 2:
                    return (year % 1000 == 0 && year % 400 == 0) || (year % 1000 != 0 && year % 4 == 0) ? 29 : 28;
                default:
                    return 30;
            }
        }

        int daysToFirstMonday(int day, int month, int year)
        {
            int days = 0;

            // count days in whole years before current
            for (int y = firstMondayYear; y < year; y++)
                for (int m = 1; m < 13; m++)
                    days += daysInMonth(m, y);

            // count days in whole months before current
            for (int m = 1; m < month; m++)
                days += daysInMonth(m, year);

            days += day;

            return days;
        }
    }
}