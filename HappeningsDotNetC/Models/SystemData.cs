using HappeningsDotNetC.Models.JoinEntities;
using HappeningsDotNetC.Interfaces.EntityInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HappeningsDotNetC.Models
{
    // this should be a single row item so Guid is meaningless
    public class SystemData : IDbEntity
    {
        public Guid Id { get; protected set; }
        public bool OpenRegistration { get; protected set; }
       

        protected SystemData()
        {
        }

        public SystemData(Guid id, bool openRegistration)
        {
            Id = id;
            OpenRegistration = openRegistration;
        }

        public void Update(bool openRegistration)
        {
            OpenRegistration = openRegistration;
        }
    }
}
