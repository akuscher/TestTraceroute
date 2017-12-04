using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestTraceroute
{
    /// <summary>
    /// Класс, реализующий, процедуру Traceroute
    /// </summary>
    public class Resolv
    {
        private static readonly ILog Log = LogManager.GetLogger("TestTraceroute");

        /// <summary>
        /// Получить IP-адрес для хоста
        /// </summary>
        /// <param name="ipAddressOrHostName"></param>
        /// <returns></returns>
        public static IPAddress GetHostEntry(string ipAddressOrHostName)
        {
            Log.InfoFormat("Получить IP-адрес для '{0}'", ipAddressOrHostName);

            try
            {
                IPHostEntry IPList = Dns.GetHostEntry(ipAddressOrHostName);
                IPAddress ipAddress = IPList.AddressList[0];
                Log.InfoFormat("IP-адрес для '{0}' : {1}", ipAddressOrHostName, ipAddress);
                return ipAddress;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Log.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name
                    + "\t" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Получить трассировку для IP-адреса
        /// </summary>
        /// <param name="ipAddressOrHostName"></param>
        /// <returns></returns>
        public void Traceroute(Action functionToExec, IPAddressCheck ipAddressCheck)
        {
            StringBuilder traceResults = new StringBuilder();

            using (Ping pingSender = new Ping())
            {
                PingOptions pingOptions = new PingOptions();
                Stopwatch stopWatch = new Stopwatch();
                byte[] bytes = new byte[32];

                pingOptions.DontFragment = true;

                pingOptions.Ttl = 1;
                int maxHops = 30;

                traceResults.AppendLine(string.Format("Результат traceroute до хоста '{0}', маск. кол-во шагов - {1} :", ipAddressCheck.HostName, maxHops));
                traceResults.AppendLine();

                for (int i = 1; i < maxHops + 1; i++)
                {
                    stopWatch.Reset();
                    stopWatch.Start();

                    PingReply pingReply = pingSender.Send(ipAddressCheck.ipAddress, 5000, new byte[32], pingOptions);

                    stopWatch.Stop();

                    traceResults.AppendLine(string.Format("{0}\t{1} ms\t{2}", i, stopWatch.ElapsedMilliseconds, pingReply.Address));

                    ipAddressCheck.ResCheck = string.Format("шаг №: {0}", i);
                    functionToExec();

                    if (pingReply.Status == IPStatus.Success)
                    {
                        traceResults.AppendLine();
                        traceResults.AppendLine("Трассировка завершена."); break;
                    }
                    pingOptions.Ttl++;


                }

                ipAddressCheck.ResCheck += " - СТОП";
                functionToExec();
            }

            Log.Info(traceResults.ToString());
        }
    }
}