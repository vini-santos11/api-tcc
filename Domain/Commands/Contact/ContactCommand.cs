using Domain.Enumerables;
using System;

namespace Domain.Commands.Contact
{
    public class ContactCommand
    {
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Gender { get; set; }
        public string DocumentNumber { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Birthdate { get; set; }
        public string ImageUrl { get; set; }
        public string ImageName { get; set; }
        public EPersonType PersonType { get; set; }
    }
}
