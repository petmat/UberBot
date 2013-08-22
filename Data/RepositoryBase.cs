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
    public abstract class RepositoryBase
    {
        private readonly UnitOfWork _unitOfWork;

        public RepositoryBase(UnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        protected UnitOfWork UnitOfWork { get { return this._unitOfWork; } }
    }
}
