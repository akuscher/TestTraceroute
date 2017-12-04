using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System.Threading;
using System.IO;

namespace TestTraceroute
{
    class Program
    {

        private static readonly ILog Log = LogManager.GetLogger("TestTraceroute");

        private static readonly string inputFile = "traceroute-hosts.txt";

        /// <summary>
        /// максимальное количество хостов 
        /// </summary>
        private static readonly int maxHosts = 10;
        private static readonly int y_pos_comm = 4;

        private static bool hasError = false;
        static object locker = new object();

        /// <summary>
        /// вывод на консоль списка IPProcessing
        /// </summary>
        static void printHosts()
        {
            lock (locker)
            {
                int count = 1;

                foreach (string s in IPProcessing.HostsEnumerator())
                {
                    Console.SetCursorPosition(1, count + 9);
                    ClearCurrentConsoleLine(count + 9);
                    Console.WriteLine(string.Format("{0,2}. ", count.ToString("D2")) + IPProcessing.GetIPbyHost(s));
                    count++;
                }
            }
        }

        /// <summary>
        /// будет добавлен только валидный хост
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static bool tryAddHost(string line)
        {
            if (IPProcessing.AddrsCount() < maxHosts)
            {
                if (IPProcessing.AddHost(line))
                {
                    printHosts();
                    return true;
                }
            }
            return false;
        }

        static void Main(string[] args)
        {
            Log.Info("");
            Log.Info("Запуск TestTraceroute");

            IPProcessing ipProc = new IPProcessing();

            Console.WriteLine("");
            Console.WriteLine("Имя хоста <ENTER> - добавить; c - проверить; q - выход; esc - сброс строки;");
            Console.WriteLine("");
            Console.WriteLine("команда:");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Читаем хосты из traceroute-hosts.txt...");
            Console.WriteLine("Максимальное количество хостов - " + maxHosts);
            Console.WriteLine("Подробрости трассировки в файле Traceroute.log");

            // есть рядом с exe-файлов есть файл hosts.txt URL для обработки
            // будут взяты из него (макс. 10 записей)
            if (File.Exists(inputFile))
            {
                using (StreamReader sr = new StreamReader(inputFile))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                        tryAddHost( line);
                }
            }

            Console.SetCursorPosition(0, y_pos_comm);

            StringBuilder sd = new StringBuilder();

            // чтенин ввода пользователя
            while (true)
            {

                /// убрать сообщенние об ошибке
                if (hasError)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    ClearCurrentConsoleLine();
                    hasError = false;
                }

                ConsoleKeyInfo cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Escape)
                {
                    sd.Clear();
                    ClearCurrentConsoleLine();
                }
                else  if (cki.Key == ConsoleKey.Enter)
                {
                    ClearCurrentConsoleLine();
                    if (sd.Length > 4)
                    {
                        if (!tryAddHost(sd.ToString()))
                        {
                           Console.ForegroundColor = ConsoleColor.Red;
                           Console.Write("! ХОСТ НЕ ДОБАВЛЕН ! ");
                            hasError = true;

                        }
                    }

                    if (sd.ToString() == "q")
                    {
                        Environment.Exit(0);
                    }

                    if (sd.ToString() == "c")
                    {
                        ipProc.GoCheck(printHosts);
                    }

                    sd.Clear();
                   
                }
                else
                {
                    sd.Append(cki.KeyChar);
                    Console.SetCursorPosition(0, y_pos_comm);
                    Console.Write(sd.ToString());
                }
            }
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            ClearCurrentConsoleLine( currentLineCursor);
        }

        public static void ClearCurrentConsoleLine(int currentLineCursor)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

    }
}


// log4net.Appender.IAppender appender = CreateFileAppender("apName", "fName");
// AddAppender(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), appender);
//Log.Info("Start traceroute vk.com");