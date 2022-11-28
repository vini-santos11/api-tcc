using Domain.Exports.Excel.Base;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.Contact;
using Domain.Services;
using System.Threading.Tasks;

namespace Domain.Exports.Excel
{
    public class ContactExcel : BaseExcelPage<PageQuery, ContactQuery>
    {
        private ContactService ContactService { get; }

        public ContactExcel(ContactService contactService, PageQuery pageQuery) : base(pageQuery)
        {
            ContactService = contactService;
        }

        public override string PrefixFileName()
        {
            return "Clientes";
        }

        protected override string Title()
        {
            return "Clientes";
        }

        public override Task<PageData<ContactQuery>> DataTable(PageQuery pageQuery)
        {
            return ContactService.FindAllContacts(pageQuery);
        }
    }
}
