using Domain.Interfaces.Repositories;
using Domain.Querys.History;
using System.Collections.Generic;

namespace Domain.Services
{
    public class MovementHistoryService
    {
        public IContactRepository ContactRepository { get; set; }
        public MovementHistoryService(IContactRepository contactRepository)
        {
            ContactRepository = contactRepository;
        }

        public IEnumerable<MovementHistoryQuery> FindCustomerPurchase(long contactId)
        {
            return ContactRepository.FindCustomerPurchase(contactId);
        }
    }
}
