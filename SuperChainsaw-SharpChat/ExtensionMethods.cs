using System;

namespace SuperChainsaw_SharpChat
{
    public static class ExtensionMethods
    {
        public static string atDate(this DateTime date)
            => "@ " + date.Hour + ":" + (date.Minute < 10 ? "0" : "") + date.Minute;
    }
}
