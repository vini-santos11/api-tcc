using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IContactRepository : ICrudRepository<long, AppContact>
    {
        Task<PageData<AppContact>> FindAllContacts(PageQuery pageQuery);
        AppContact FindByDocumentNumber(string documentNumber);
    }
}
