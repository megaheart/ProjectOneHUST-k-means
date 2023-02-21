using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public enum ColumnAlign { Left, Right, Center }
    public class ColumnStyle
    {
        public ColumnAlign Align { get; set; } = ColumnAlign.Left;
        public int Width { get; set; } = 20;
        public int NumberPrecision { get; set; } = 6;
    }
    public class ColumnContent
    {
        public object Content { get; set; } = null;
        public ConsoleColor Color { get; set; } = ConsoleColor.White;
    }
    public class PrintTable
    {
        IReadOnlyList<ColumnStyle> columnStyles;
        public PrintTable([NotNull]IReadOnlyList<ColumnStyle> columnStyles) {
            //if (tableWidth == null) throw new Exception("Table must be not null.");
            this.columnStyles = columnStyles;
        }
        public void PrintLine()
        {
            int _tableWidth = columnStyles.Sum(x=>x.Width) + columnStyles.Count + 1;
            Console.WriteLine(new string('-', _tableWidth));
        }

        public void PrintRow(params object[] columns)
        {

            Console.Write("|");

            for (var i = 0; i < columns.Length; i++)
            {
                int width = columnStyles[i].Width;
                if (columns[i] is ColumnContent)
                {
                    var content = (ColumnContent)columns[i];
                    string column;
                    if (content.Content is double || content.Content is float)
                    {
                        column = String.Format("{0:F" + columnStyles[i].NumberPrecision + "}", content.Content);
                    }
                    else column = content.Content.ToString();
                    Console.ForegroundColor= content.Color;
                    switch (columnStyles[i].Align)
                    {
                        case ColumnAlign.Left:
                            Console.Write(AlignLeft(column, width));
                            break;
                        case ColumnAlign.Center:
                            Console.Write(AlignCentre(column, width));
                            break;
                        default:
                            Console.Write(AlignRight(column, width));
                            break;
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|");
                }
                else
                {
                    string column;
                    if (columns[i] is double || columns[i] is float)
                    {
                        column = String.Format("{0:F" + columnStyles[i].NumberPrecision + "}", columns[i]);
                    }
                    else column = columns[i].ToString();
                    switch (columnStyles[i].Align)
                    {
                        case ColumnAlign.Left:
                            Console.Write(AlignLeft(column, width) + "|");
                            break;
                        case ColumnAlign.Center:
                            Console.Write(AlignCentre(column, width) + "|");
                            break;
                        default:
                            Console.Write(AlignRight(column, width) + "|");
                            break;
                    }
                }
                
            }

            Console.WriteLine();
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
        static string AlignLeft(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width);
            }
        }
        static string AlignRight(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadLeft(width);
            }
        }
    }
}
