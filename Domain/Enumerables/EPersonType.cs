using System.ComponentModel;

namespace Domain.Enumerables
{
    public enum EPersonType
    {
        //Pessoa Física
        [Description("Natural Person")]
        NaturalPerson = 1,
        //Pessoa Jurídica(empresa)
        [Description("Legal Person")]
        LegalPerson = 2
    }
}
