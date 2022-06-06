using Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Commands.User
{
    public class RegisterCommand
    {
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Gender { get; set; }
        public EPersonType PersonType { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
