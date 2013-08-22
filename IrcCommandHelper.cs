using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberBot
{
    public static class IrcCommandHelper
    {
        public static IrcCommand ParseIrcCommand(string value)
        {
            IrcCommand command = new IrcCommand();

            if (!string.IsNullOrEmpty(value))
            {
                string[] data = value.Split(new char[] { ' ' }, 4);

                string identity = data[0];
                string[] identityData = identity.Split('!');

                if (identityData.Length > 1)
                {
                    command.Nick = identityData[0].TrimStart(':');
                    command.User = identityData[1];
                }
                else
                {
                    command.Nick = identity;
                }

                if (data.Length > 1)
                {
                    command.Command = data[1];
                }
                if (data.Length > 3)
                {
                    command.Parameter = data[3];
                }
            }

            return command;
        }

        public static bool IsBotCommand(IrcCommand ircCommand)
        {
            return (ircCommand.Command == IrcCommandName.PrivateMessage 
                && !string.IsNullOrEmpty(ircCommand.Parameter) 
                && ircCommand.Parameter.StartsWith(":!"));
        }
    }
}
