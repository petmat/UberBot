using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UberBot.Data;

namespace UberBot
{
    public class IrcBot : IDisposable
    {
        private readonly IrcConfiguration _ircConfig;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;
        private bool _opened;
        private List<BotCommandResponse> _botCommandResponses;
        private Dictionary<string, string> _errorMessages;

        public IrcBot(IrcConfiguration ircConfig)
        {
            this._ircConfig = ircConfig;
            this.InitializeBotCommandResponses();
            this.InitializeErrorMessages();
        }

        private void InitializeErrorMessages()
        {
            this._errorMessages = new Dictionary<string, string>()
            {
                { "482", "I don't have channel op priviledges :( Op me first!" }
            };
        }

        private void InitializeBotCommandResponses()
        {
            this._botCommandResponses = new List<BotCommandResponse>()
            {
                new BotCommandResponse(BotCommand.Join, 
                    (bot, botCommand) => 
                        {
                            string channelName = botCommand.Parameters[0];
                            bot.SendData(IrcCommandName.Join, channelName);
                            bot.CurrentChannel = channelName;
                        }),
                new BotCommandResponse(BotCommand.Part, 
                    (bot, botCommand) => 
                        {
                            bot.SendData(IrcCommandName.Part, bot.CurrentChannel);
                        }),
                new BotCommandResponse(BotCommand.Say, 
                    (bot, botCommand) => 
                        {
                            bot.SendPrivateMessage(bot.CurrentChannel, string.Join(" ", botCommand.Parameters));
                        }),
                new BotCommandResponse(BotCommand.Quit, 
                    (bot, botCommand) => 
                        {
                            bot.SendData(IrcCommandName.Quit, botCommand.Parameters[0]);
                            bot.IsRunning = false;
                        }),
                new BotCommandResponse(BotCommand.OpMe, 
                    (bot, botCommand) => 
                        {
                            bot.SendData(IrcCommandName.Mode, bot.CurrentChannel + " +o " + botCommand.Nick);
                        }),
                new BotCommandResponse(BotCommand.AddAdmin, 
                    (bot, botCommand) => 
                        {
                            bot.AddAdmin(botCommand.Parameters[0], botCommand.Parameters[1]);
                        })
            };
        }

        public void SendPrivateMessage(string target, string message)
        {
            this.SendData(IrcCommandName.PrivateMessage, target + " :" + message);
        }       

        public IrcConfiguration IrcConnection
        {
            get
            {
                return this._ircConfig;
            }
        }

        public void OpenConnection()
        {
            this._tcpClient = new TcpClient(this._ircConfig.HostName, this._ircConfig.Port);
            this._networkStream = this._tcpClient.GetStream();
            this._streamReader = new StreamReader(this._networkStream);
            this._streamWriter = new StreamWriter(this._networkStream);
            this._opened = true;

            this.SendUserInformation();
        }

        private void SendUserInformation()
        {
            this.SendData(IrcCommandName.User, this._ircConfig.Nick + " bot.com " + " bot.com" + " :" + this._ircConfig.Name);
            this.SendData(IrcCommandName.Nick, this._ircConfig.Nick);
        }

        public void CloseConnection()
        {
            if (this._opened)
            {
                this._streamReader.Close();
                this._streamWriter.Close();
                this._tcpClient.Close();
            }
        }

        private void SendData(string command, string parameter = "")
        {
            if (!this._opened) throw new InvalidOperationException("The connection is not open.");

            string fullCommand = command + (!string.IsNullOrEmpty(parameter) ? " " + parameter : string.Empty);
            this._streamWriter.WriteLine(fullCommand);
            this._streamWriter.Flush();
            Console.WriteLine(fullCommand);
        }

        public void Dispose()
        {
            this.CloseConnection();
        }

        public bool IsRunning { get; set; }

        public string CurrentChannel { get; set; }

        public string PrevCommandNick { get; set; }

        public void Start()
        {
            this.IsRunning = true;

            while (this.IsRunning)
            {
                this.Listen();
            }
        }

        public void Listen()
        {
            while (this.IsRunning)
            {
                string dataLine = this._streamReader.ReadLine();

                if (!string.IsNullOrEmpty(dataLine))
                {
                    Console.WriteLine(dataLine);
                    char[] charSeparator = new char[] { ' ' };
                    string[] data = dataLine.Split(charSeparator, 5);

                    if (this.IsPing(data))
                    {
                        this.SendPong(data);
                    }                    
                    else if (data.Length > 1)
                    {
                        IrcCommand ircCommand = IrcCommandHelper.ParseIrcCommand(dataLine);
                        
                        if (IrcCommandHelper.IsBotCommand(ircCommand))
                        {
                            BotCommand botCommand = new BotCommand(ircCommand);

                            BotCommandResponse botCommandResponse = this._botCommandResponses.SingleOrDefault(r => r.Command == botCommand.Command);

                            if (botCommandResponse != null && this.AuthorizeUser(botCommand.Nick, botCommand.User))
                            {
                                botCommandResponse.Execute(this, botCommand);
                            }

                            this.PrevCommandNick = botCommand.Nick;
                        }
                        else if (this.IsError(ircCommand.Command) && !string.IsNullOrEmpty(this.PrevCommandNick))
                        {
                            this.SendPrivateMessage(this.PrevCommandNick, this.GetErrorMessage(ircCommand.Command)); 
                        }
                    }
                }
            }
        }

        private string GetErrorMessage(string command)
        {
            return this._errorMessages[command];
        }

        private bool IsError(string command)
        {
            return this._errorMessages.ContainsKey(command);
        }

        private bool AuthorizeUser(string nick, string user)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            IEnumerable<Admin> admins = unitOfWork.AdminRepository.GetAdmins();

            return admins.Any(a => a.Nick == nick && Regex.IsMatch(user, a.UserFilter));
        }

        public void AddAdmin(string nick, string userFilter)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            
            unitOfWork.AdminRepository.AddAdmin(new Admin()
                {
                    Nick = nick,
                    UserFilter = userFilter
                });

            unitOfWork.SaveChanges();
        }

        private void SendPong(string[] data)
        {
            if (data.Length > 0)
            {
                this.SendData("PONG", data[1]);
            }
        }

        private bool IsPing(string[] data)
        {
            return data[0] == "PING";
        }
    }
}
    