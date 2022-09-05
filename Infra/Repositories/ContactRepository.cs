﻿using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Infra.Repositories.Base;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class ContactRepository : CrudRepository<long, AppContact>, IContactRepository
    {
        public ContactRepository(IUserContext userContext, IDBContext dbContext) : base(userContext, dbContext)
        {

        }

        public Task<PageData<AppContact>> FindAllContacts(PageQuery pageQuery)
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
            sql.Append("       con.ImageUrl, ");
            sql.Append("       con.ImageName ");
            sql.Append("  FROM db_tcc.App_Contact con ");
            //sql.Append(" Where (con.PersonTypeId Like @Query Or ");
            //sql.Append("        con.Name Like @Query Or ");
            //sql.Append("        con.DocumentNumber @Query Or ");
            //sql.Append("        con.Email @Query)");  

            return PageData<AppContact>(sql, pageQuery, "Name");
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