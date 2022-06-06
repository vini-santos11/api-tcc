using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Repositories.Base;
using System.Text;

namespace Infra.Repositories
{
    public class ContactRepository : CrudRepository<long, AppContact>, IContactRepository
    {
        public ContactRepository(IUserContext userContext, IDBContext dbContext) : base(userContext, dbContext)
        {

        }

        public AppContact FindByDocumentNumber(string documentNumber)
        {
            var sql = new StringBuilder();
            sql.Append(" Select con.* ");
            sql.Append("   From App_Contact con ");
            sql.Append("  Where con.DocumentNumber = @DocumentNumber ");

            return QuerySingleOrDefault<AppContact>(sql, new { documentNumber });
        }
    }
}
