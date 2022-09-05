using System;

namespace Domain.Querys.Contact
{
    public class ContactQuery
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PersonType { get; set; }
        public string ImageUrl { get; set; }
        public string ImageName { get; set; }
    }
}
