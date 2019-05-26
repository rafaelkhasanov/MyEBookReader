using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace MyEBookReader
{
    class Program
    {
        private static string theEBook = "";
        static void Main(string[] args)
        {
            GetBook();
            Console.ReadLine();
        }

        static void GetBook()
        {
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (s, eArgs) =>
            {
                theEBook = eArgs.Result;
                Console.WriteLine("Download complete.");
                GetStats();
            };
            //Загрузить электронную книгу Чарльза Диккенса "A Tale of Two Cities".
            //Может потребоваться двухкратное выполнение этого кода, если ранее вы
            //не посещали даный сайт, поскольку при первом его посещении появляется
            //окно с сообщением, предотвращающее нормальное выполнение кода.
            wc.DownloadStringAsync(new Uri("http://www.gutenberg.org/files/98/98-8.txt"));
        }

        static void GetStats()
        {
            //Получить слова из электронной книги
            var words = theEBook.Split(new char[] {' ', '\u000A', ',', '.', ';', ':', '-', '?', '/'}, StringSplitOptions.RemoveEmptyEntries);
            //Найти 10 наиболее часто встречающихся слов
            string[] tenMostCommon = null;
            //Получить самое длинное слово
            var longestWord = string.Empty;
            //Когда все задачи завершены, построить строку, показывающую всю статистику в окне сообщений
            Parallel.Invoke(
                () => tenMostCommon = FindTenMostCommon(words),
                () => { longestWord = FindLongestWord(words); });
            StringBuilder bookStats = new StringBuilder("Ten Most Common Words are: \n");
            foreach (var s in tenMostCommon)
            {
                bookStats.AppendLine(s);
            }

            bookStats.AppendFormat($"Longest word is: {longestWord}"); //Самое длинное слово
            bookStats.AppendLine();
            Console.WriteLine(bookStats.ToString(), "Book info"); //Информация о книге
        }

        private static string[] FindTenMostCommon(string[] words)
        {
            var frequencyOrder = from word in words
                where word.Length > 6
                group word by word
                into g
                orderby g.Count() descending
                select g.Key;
            var commonWords = (frequencyOrder.Take(10)).ToArray();
            return commonWords;
        }

        private static string FindLongestWord(string[] words)
        {
            return (from word in words orderby word.Length descending
                select word).FirstOrDefault();
        }
    }
}
