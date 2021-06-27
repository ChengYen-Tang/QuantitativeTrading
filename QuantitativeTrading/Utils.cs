namespace QuantitativeTrading
{
    public static class Utils
    {
        public static string MinuteToHrOrDay(int minute)
        {
            if (minute >= 60)
                return $"{minute / 60}Hr";
            if (minute >= 1440)
                return $"{minute / 1440}Day";
            return $"{minute}Min";
        }
    }
}
