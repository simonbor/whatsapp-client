using System;

// colors preview - https://i.stack.imgur.com/EbbxO.png

namespace Listener
{
    public static class ConsoleProxy
    {
        private static readonly ConsoleColor _defBgColor = Console.BackgroundColor;
        private static readonly ConsoleColor _defFgColor = Console.ForegroundColor;

        public static void WriteLine(ConsoleColor? bgColor, ConsoleColor? fgColor, string key, string val)
        {
            Console.BackgroundColor = bgColor ?? _defBgColor;
            Console.ForegroundColor = fgColor ?? _defFgColor;

            string darkedColorName = Console.ForegroundColor.ToString();
            string colorName = darkedColorName.Replace("Dark", "");

            Console.Write(key);
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colorName, true);
            Console.WriteLine(val);
        }
        public static void WriteLine(ConsoleColor? bgColor, ConsoleColor? fgColor, string value)
        {
            Console.BackgroundColor = bgColor ?? _defBgColor;
            Console.ForegroundColor = fgColor ?? _defFgColor;

            Console.WriteLine(value);
        }
    }
}
