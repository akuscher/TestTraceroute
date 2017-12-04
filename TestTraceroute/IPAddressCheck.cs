using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestTraceroute
{

    /// <summary>
    /// Класс-обертка для IPAddress
    /// </summary>
    public class IPAddressCheck
    {
        public IPAddress ipAddress { get; set; }

        /// <summary>
        /// текущий шаг трассировки
        /// </summary>
        public string ResCheck  { get; set; }
        
        /// <summary>
        /// имя хоста~IP
        /// </summary>
        public string HostName { get; set; }


        /// <summary>
        /// форматирует вывод IPAddress
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(ResCheck))
               return string.Format("[{0,-15}] - [{1}]", ipAddress.ToString() , HostName);

            return string.Format("[{0,-15}] - [{1}] - [{2}]", ipAddress.ToString(), HostName, ResCheck);
        }
    }
}
