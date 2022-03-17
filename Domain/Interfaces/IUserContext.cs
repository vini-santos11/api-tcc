using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserContext
    {
        long? Id { get; }
        string FirstName { get; }
        string LastName { get; }
        //ERoles Role { get; }
        string FullName { get => $"{FirstName} {LastName}".Trim(); }
    }
}
