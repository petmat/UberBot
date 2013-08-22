using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberBot
{
    public class IrcConfiguration
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string Nick { get; set; }
        public string Name { get; set; }

        public static IrcConfiguration GetConfig()
        {
            return new IrcConfiguration()
            {
                HostName = ConfigurationManager.AppSettings["HostName"],
                Port = int.Parse(ConfigurationManager.AppSettings["Port"]),
                Nick = ConfigurationManager.AppSettings["Nick"],
                Name = ConfigurationManager.AppSettings["Name"],
            };
        }
    }
}
