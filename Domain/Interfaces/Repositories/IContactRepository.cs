using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.Contact;
using Domain.Querys.History;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IContactRepository : ICrudRepository<long, AppContact>
    {
        Task<PageData<ContactQuery>> FindAllContacts(PageQuery pageQuery);
        AppContact FindByDocumentNumber(string documentNumber);
        IEnumerable<MovementHistoryQuery> FindCustomerPurchase(long contactId);
    }
}
