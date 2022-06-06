using System.IO;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IExportFile
    {
        string FileName { get; }

        string ContentType { get; }

        Task<MemoryStream> Export();
    }
}
