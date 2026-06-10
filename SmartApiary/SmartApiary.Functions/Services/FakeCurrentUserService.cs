using SmartApiary.Application.Interfaces.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Functions.Services
{
    public class FakeCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => null;
    }
}
