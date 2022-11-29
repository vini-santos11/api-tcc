using Domain.Enumerables;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys;
using Domain.Querys.Contact;
using Domain.Querys.History;
using Infra.Repositories.Base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class ContactRepository : CrudRepository<long, AppContact>, IContactRepository
    {
        public ContactRepository(IUserContext userContext, IDBContext dbContext) : base(userContext, dbContext)
        {

        }

        public Task<PageData<ContactQuery>> FindAllContacts(PageQuery pageQuery)
        {
            var sql = new StringBuilder();
            sql.Append("SELECT con.Id, ");
            sql.Append("	   CONCAT(con.Name, ' ',  con.SecondName) as Name,  ");
            sql.Append("       con.Gender, ");
            sql.Append("       con.DocumentNumber, ");
            sql.Append("       con.BirthDate, ");
            sql.Append("       con.Phone, ");
            sql.Append("       con.Address, ");
            sql.Append("       con.Email, ");
            sql.Append("       pty.Description as PersonType, ");
            sql.Append("       con.ImageUrl, ");
            sql.Append("       con.ImageName ");
            sql.Append("  FROM db_tcc.App_Contact con ");
            sql.Append(" Inner Join App_PersonType pty on (pty.Id = con.PersonTypeId) ");
            sql.Append(" Where (pty.Description Like @Query Or ");
            sql.Append("        con.Name Like @Query Or ");
            sql.Append("        con.DocumentNumber Like @Query Or ");
            sql.Append("        con.Email Like @Query)");

            return PageData<ContactQuery>(sql, pageQuery, "Name");
        }

        public AppContact FindByDocumentNumber(string documentNumber)
        {
            var sql = new StringBuilder();
            sql.Append(" Select con.* ");
            sql.Append("   From App_Contact con ");
            sql.Append("  Where con.DocumentNumber = @DocumentNumber ");

            return QuerySingleOrDefault<AppContact>(sql, new { documentNumber });
        }

        public ImageQuery FindImageContact(long contactId)
        {
            var sql = new StringBuilder();
            sql.Append(" Select con.Id, ");
            sql.Append("        con.ImageName, ");
            sql.Append("        con.ImageUrl ");
            sql.Append("   From db_tcc.App_Contact con ");
            sql.Append("  Where con.Id = @ContactId ");

            return QuerySingleOrDefault<ImageQuery>(sql, new { contactId });
        }

        public Task<PageData<MovementHistoryQuery>> FindMovementHistory(HistoryPageQuery query)
        {
            var sql = new StringBuilder();
            sql.Append(" SELECT trn.Id as Id, ");
            sql.Append("        pro.Id as ProductId,");
            sql.Append("        pro.Name as Product, ");
            sql.Append("        trn.Amount, ");
            sql.Append("        trn.TotalPrice, ");
            sql.Append("        concat(con.Name,' ',con.SecondName) as Contact, ");  
            sql.Append("        trn.CreatedAt as BuyDate ");
            sql.Append("   FROM db_tcc.App_Transaction trn ");
            sql.Append("  INNER JOIN db_tcc.App_Product pro on (pro.Id = trn.ProductId) ");
            if(query.Operation == EOperation.Venda || query.Operation == EOperation.Consumo)
                sql.Append("  LEFT JOIN db_tcc.App_Contact con on (con.Id = trn.ContactDestinationId)" );
            if (query.Operation == EOperation.Compra || query.Operation == EOperation.Producao)
                sql.Append("  LEFT JOIN db_tcc.App_Contact con on (con.Id = trn.ContactOriginId)" );
            sql.Append($"  WHERE trn.OperationId = {(int)query.Operation} ");
            sql.Append("   ORDER BY trn.CreatedAt desc  ");

            return PageData<MovementHistoryQuery>(sql, query, "Product");
        }
    }
}
