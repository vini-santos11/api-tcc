using Domain.Interfaces.Repositories;

namespace Domain.Services
{
    public class ProductService
    {
        public IProductRepository ProductRepository { get; set; }
        public ProductService(
            IProductRepository productRepository) 
        {
            ProductRepository = productRepository;
        }
    }
}
