using Domain.Enumerables;
using Domain.Interfaces.Repositories;
using Domain.PageQuerys;
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

        public IEnumerable<MovementHistoryQuery> FindMovementHistory(HistoryPageQuery query)
        {
            return ContactRepository.FindMovementHistory(query);
        }
    }
}
