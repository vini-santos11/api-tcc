using Domain.Enumerables;
using Domain.Interfaces.Repositories;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.History;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class MovementHistoryService
    {
        public IContactRepository ContactRepository { get; set; }
        public MovementHistoryService(IContactRepository contactRepository)
        {
            ContactRepository = contactRepository;
        }

        public Task<PageData<MovementHistoryQuery>> FindMovementHistory(HistoryPageQuery query)
        {
            return ContactRepository.FindMovementHistory(query);
        }
    }
}
