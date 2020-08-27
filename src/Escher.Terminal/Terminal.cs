using System;
using System.Collections.Generic;
using System.IO;

namespace Escher.Terminal
{
    public class Terminal
    {
        public readonly TextWriter o;
        public char BorderChar = '*';
        public int BorderWidth = 4;

        public Terminal()
        {
            o = Console.Out;
        }

        public int ScreenWidth => Console.WindowWidth - 1;

        public void Logo(string text)
        {
            BorderLine();
            Whiskers();
            Whiskers(text);
            Whiskers();
            Whiskers("© 2020 by Max Stankevich");
            Whiskers("https://github.com/gavar");
            Whiskers();
        }

        public void Title(string text)
        {
            BorderLine();
            WhiskersTight(text.ToUpper(), ConsoleColor.Yellow);
            BorderLine();
        }

        public int AskMenu(IDictionary<int, string> menu)
        {
            Options(menu);
            return AskOption(menu.Keys);
        }

        private void Options(IDictionary<int, string> options)
        {
            foreach (var (key, label) in options)
                Option(key, label);
        }

        public void Option(int key, string label)
        {
            o.Write(key);
            o.Write(". ");
            o.Write(label);
            o.WriteLine();
        }

        public void Success(string operation)
        {
            o.WriteLine();
            using (new FgColor(ConsoleColor.Green))
            {
                o.Write(operation);
                o.Write(" has completed successfully!");
            }

            o.WriteLine();
        }

        public void Deny(string operation, string details = null)
        {
            o.WriteLine();
            using (new FgColor(ConsoleColor.Red))
            {
                o.Write(operation.ToUpper());
                o.Write(" DENIED");
            }

            if (!string.IsNullOrWhiteSpace(details))
            {
                o.WriteLine();
                o.Write(details);
            }

            o.WriteLine();
        }

        public int AskOption(ICollection<int> options)
        {
            while (true)
            {
                o.Write("Please select an option: ");
                if (int.TryParse(Console.ReadLine(), out var option))
                    if (options.Contains(option))
                        return option;
            }
        }

        public bool AskYesNo(string label, bool optional = false)
        {
            return Ask(label, ParseYesNo, optional, null);
        }

        public string Ask(string label, bool optional = false)
        {
            return Ask(label, Identity, optional);
        }

        public T Ask<T>(string label, Func<string, T> parse, bool optional = false, string delimiter = ":")
        {
            while (true)
            {
                o.Write(optional ? "  " : "* ");
                o.Write(label);
                o.Write(delimiter);
                o.Write(' ');

                var input = (Console.ReadLine() ?? "").Trim();
                if (optional) return parse(input);

                if (string.IsNullOrEmpty(input))
                {
                    o.Write(label);
                    o.Write(" is a required field!");
                    o.WriteLine();
                    continue;
                }

                try
                {
                    return parse(input);
                }
                catch (Exception e)
                {
                    WriteLine(e.Message, ConsoleColor.Red);
                }
            }
        }

        public void Whiskers()
        {
            Repeat(BorderChar, BorderWidth);
            Repeat(' ', ScreenWidth - 2 * BorderWidth);
            Repeat(BorderChar, BorderWidth);
            o.WriteLine();
        }

        public void Whiskers(string text)
        {
            var space = ScreenWidth - 2 * BorderWidth;
            var empty = space - text.Length;
            var left = empty / 2;
            var right = empty - left;

            Repeat(BorderChar, BorderWidth);
            Repeat(' ', left);
            o.Write(text);
            Repeat(' ', right);
            Repeat(BorderChar, BorderWidth);
            o.WriteLine();
        }

        public void WhiskersTight(string text, ConsoleColor? fg = null)
        {
            const int margin = 4;
            var fill = ScreenWidth - margin * 2 - text.Length;
            var left = fill / 2;
            var right = fill - left;

            Repeat(BorderChar, left);
            Repeat(' ', margin);
            Write(text, fg);
            Repeat(' ', margin);
            Repeat(BorderChar, right);
            o.WriteLine();
        }

        public void BorderLine()
        {
            Line(BorderChar, ScreenWidth);
        }

        public void Line(char c)
        {
            Line(c, ScreenWidth);
        }

        public void Line(char c, int width)
        {
            Repeat(c, width);
            o.WriteLine();
        }

        public void Repeat(char c, int count)
        {
            for (var i = 0; i < count; i++)
                o.Write(c);
        }

        public void Write(string text)
        {
            o.Write(text);
        }

        public void Write(string text, ConsoleColor? fg)
        {
            using (new FgColor(fg))
            {
                o.Write(text);
            }
        }

        public void WriteLine(string text = null, ConsoleColor? fg = null)
        {
            using (new FgColor(fg))
            {
                o.WriteLine(text);
            }
        }

        private static string Identity(string input)
        {
            return input;
        }

        private static bool ParseYesNo(string input)
        {
            switch (input)
            {
                case "y":
                case "Y":
                    return true;
                case "n":
                case "N":
                    return false;
                default:
                    throw new ArgumentException("Please type 'y' for 'yes' or 'n' for 'no'");
            }
        }

        private readonly ref struct FgColor
        {
            private readonly ConsoleColor original;

            public FgColor(ConsoleColor? color)
            {
                original = Console.ForegroundColor;
                Console.ForegroundColor = color.GetValueOrDefault(original);
            }

            public void Dispose()
            {
                Console.ForegroundColor = original;
            }
        }
    }
}