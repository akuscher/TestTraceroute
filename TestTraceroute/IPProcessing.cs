using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestTraceroute
{
    /// <summary>
    /// конвейер обработки
    /// </summary>
    public class IPProcessing
    {
        private static readonly ILog Log = LogManager.GetLogger("TestTraceroute");

        /// <summary>
        /// набор имен хостов для обработки
        /// </summary>
        static HashSet<string> hosts = new HashSet<string>();

        public static IEnumerable<string> HostsEnumerator()
        {
            foreach (string s in hosts)
                yield return s;
        }

        /// <summary>
        /// связь IP и хоста
        /// </summary>
        static Dictionary<string, IPAddressCheck> addrsDict = new Dictionary<string, IPAddressCheck>();


        /// <summary>
        /// добавить хост и IP в словарь и хеш-таблицу
        /// </summary>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public static bool AddHost(string hostName)
        {
            IPAddress ip = Resolv.GetHostEntry(hostName);

            if (ip == null)
                return false;

            if (!hosts.Contains(hostName))
            {
                hosts.Add(hostName);
                if (!addrsDict.ContainsKey(hostName))
                {
                    addrsDict.Add(hostName, new IPAddressCheck { ipAddress = ip, HostName = hostName });
                    return true;
                }
            }

            return false;
        }

        public static string GetIPbyHost(string hostName)
        {
            if (addrsDict.ContainsKey(hostName))
            {
                IPAddressCheck ip = addrsDict[hostName];
                if (ip != null)
                    return ip.ToString();
            }
            return "";
        }

        public static int AddrsCount()
        {
            return addrsDict.Count();
        }


        /// <summary>
        /// Запуск проверки
        /// </summary>
        /// <param name="functionToExec">обновить состочние на консоле</param>
        public void GoCheck(Action functionToExec)
        {
            foreach (KeyValuePair<string, IPAddressCheck> entry in addrsDict)
            {
                Task task = new Task(() =>
                {
                    Resolv rs = new Resolv();
                    rs.Traceroute(functionToExec, entry.Value);
                });
                task.Start();
            }
        }
    }
}
