using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UberBot.Data
{
    public class UnitOfWork
    {
        protected string DataPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\XmlFiles\IrcBotData.xml";
            }
        }

        private IrcBotDataContainer _data;
        public IrcBotDataContainer Data
        {
            get
            {
                if (this._data == null)
                {
                    if (File.Exists(this.DataPath))
                    {
                        using (StreamReader reader = new StreamReader(this.DataPath))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(IrcBotDataContainer));
                            this._data = (IrcBotDataContainer)serializer.Deserialize(reader);
                        }
                    }
                    else
                    {
                        this._data = this.CreateNewDataContainer();
                    }

                }
                return this._data;
            }
        }

        private IrcBotDataContainer CreateNewDataContainer()
        {
            return new IrcBotDataContainer()
            {
                Admins = new Admin[] { },
                Rules = new Rule[] { }
            };
        }

        public void SaveChanges()
        {
            if (this._data != null)
            {
                using (StreamWriter writer = new StreamWriter(this.DataPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IrcBotDataContainer));
                    serializer.Serialize(writer, this._data);
                }
            }
        }

        private AdminRepository _adminRepository;
        public AdminRepository AdminRepository
        {
            get 
            {
                if (this._adminRepository == null)
                {
                    this._adminRepository = new AdminRepository(this);
                }
                return this._adminRepository;
            }
        }
    }
}
