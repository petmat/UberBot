using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberBot.Data
{
    public class BotCommand
    {
        public const string AddAdmin = ":!addadmin";
        public const string AddRule = ":!addrule";
        public const string Ban = ":!ban";
        public const string Join = ":!join";
        public const string Kick = ":!kick";
        public const string Op = ":!op";
        public const string OpMe = ":!opme";
        public const string Part = ":!part";
        public const string Quit = ":!quit";
        public const string RemoveAdmin = ":!removeadmin";
        public const string RemoveRules = ":!removerule";
        public const string Say = ":!say";

        public BotCommand()
            : this(string.Empty)
        { ;}

        public BotCommand(string line)
        {
            this.Parameters = new List<string>();
            if (!string.IsNullOrEmpty(line))
            {
                SetProperties(IrcCommandHelper.ParseIrcCommand(line), this);
            }
        }

        public BotCommand(IrcCommand ircCommand)
        {
            this.Parameters = new List<string>();
            SetProperties(ircCommand, this);
        }

        public static void SetProperties(IrcCommand source, BotCommand target)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");

            string[] botCommandParts = source.Parameter.Split(' ');

            target.Command = botCommandParts[0];
            target.IrcCommand = source.Command;
            target.Nick = source.Nick;

            foreach (var parameter in botCommandParts.Skip(1))
            {
                target.Parameters.Add(parameter);
            }

            target.User = source.User;
        }

        public static BotCommand Parse(IrcCommand ircCommand)
        {
            BotCommand botCommand = new BotCommand();
            SetProperties(ircCommand, botCommand);
            return botCommand;
        }

        public string Nick { get; set; }

        public string User { get; set; }

        public string IrcCommand { get; set; }

        public string Command { get; set; }

        public IList<string> Parameters { get; set; }
    }
}
