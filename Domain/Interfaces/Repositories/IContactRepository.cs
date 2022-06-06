using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IContactRepository : ICrudRepository<long, AppContact>
    {
        AppContact FindByDocumentNumber(string documentNumber);
    }
}
