using Domain.Commands.Product;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.Product;
using System.Threading.Tasks;

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

        public Task<PageData<ProductQuery>> FindAllProducts(PageQuery pageQuery)
        {
            return ProductRepository.FindAllProducts(pageQuery);
        }

        public AppProduct FindProductById(long id)
        {
            return ProductRepository.Find(id) ?? throw new ValidateException(Messages.ProductNotFound);
        }

        public AppProduct CreateProduct(ProductCommand command)
        {
            var product = new AppProduct
            {
                Name = command.Name,
                Description = command.Description,
                DefaultMeansurement = command.DefaultMeansurement,
                Price = command.Price,
                ImageName = command.ImageName,
                ImageUrl = command.ImageUrl
            };

            ProductRepository.Add(product);
            return product;
        }

        public AppProduct UpdateProduct(long id, ProductCommand command)
        {
            var product = ProductRepository.Find(id) ?? throw new ValidateException(Messages.ProductNotFound);

            product.Name = command.Name;
            product.Description = command.Description;
            product.DefaultMeansurement = command.DefaultMeansurement;
            product.Price = command.Price;
            product.ImageName = command.ImageName;
            product.ImageUrl = command.ImageUrl;

            ProductRepository.Update(product);
            return product;
        }

        public void DeleteProduct(long id)
        {
            var product = ProductRepository.Find(id) ?? throw new ValidateException(Messages.ProductNotFound);

            ProductRepository.Remove(product);
        }
    }
}
