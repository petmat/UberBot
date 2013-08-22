using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UberBot.Data.Mapping;

namespace UberBot
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            MapInitializer.Initialize();

            using (IrcBot bot = new IrcBot(IrcConfiguration.GetConfig()))
            {
                bot.OpenConnection();
                bot.Start();
            }
        }
    }
}
