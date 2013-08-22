using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UberBot.Data;

namespace UberBot
{
    public class BotCommandResponse
    {
        public BotCommandResponse(string command, Action<IrcBot, BotCommand> sendDataAction)
        {
            this.Command = command;
            this.SendDataAction = sendDataAction;
        }

        public string Command { get; set; }

        public Action<IrcBot, BotCommand> SendDataAction { get; set; }

        public void Execute(IrcBot bot, BotCommand botCommand)
        {
            if (this.SendDataAction != null)
            {
                this.SendDataAction(bot, botCommand);
            }
        }
    }
}
