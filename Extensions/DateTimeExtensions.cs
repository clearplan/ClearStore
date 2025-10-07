namespace ClearStore.Extensions
{
    public static class DateTimeExtensions
    {
        public static TimeSpan? ToLocalOffsetIfUtc(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return null;
            }

            var value = dateTime.Value;

            return value.Kind == DateTimeKind.Utc
                ? value.ToLocalTime().ToLocalOffsetIfUtc()
                : ((DateTimeOffset)value).Offset;
        }

        public static TimeSpan ToLocalOffsetIfUtc(this DateTime dateTime)
        {
            return dateTime.Kind == DateTimeKind.Utc
                ? dateTime.ToLocalTime().ToLocalOffsetIfUtc()
                : ((DateTimeOffset)dateTime).Offset;
        }

        public static DateTime DateYear2Digit(this DateTime dateTime)
        {
            return dateTime.ToString("MM/dd/yy") is string formattedDate
                ? DateTime.Parse(formattedDate)
                : dateTime;
        }
    }
}
