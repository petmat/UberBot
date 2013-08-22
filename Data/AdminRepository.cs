using AutoMapper;
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
    public class AdminRepository : RepositoryBase
    {
        public AdminRepository(UnitOfWork unitOfWork)
            : base(unitOfWork)
        { ;}

        public IEnumerable<Admin> GetAdmins()
        {
            return this.UnitOfWork.Data.Admins;
        }

        public void AddAdmin(Admin admin)
        {
            if (this.UnitOfWork.Data.Admins.SingleOrDefault(a => a.Nick == admin.Nick) == null)
            {
                List<Admin> admins = this.UnitOfWork.Data.Admins.ToList();
                admins.Add(admin);
                this.UnitOfWork.Data.Admins = admins.ToArray();
            }
        }

        public void RemoveAdmin(Admin admin)
        {
            IEnumerable<Admin> admins = this.UnitOfWork.Data.Admins.Where( a => a.Nick != admin.Nick );
            this.UnitOfWork.Data.Admins = admins.ToArray();
        }
    }
}
