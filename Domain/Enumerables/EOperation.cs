using System.ComponentModel;

namespace Domain.Enumerables
{
    public enum EOperation
    {
        [Description("Compra")]
        Compra = 1,
        [Description("Produção")]
        Producao = 2,
        [Description("Venda")]
        Venda = 3,
        [Description("Consumo")]
        Consumo = 4
    }
}
