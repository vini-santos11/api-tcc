using Domain.Commands.Inventory;
using Domain.Enumerables;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.Inventory;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class InventoryService
    {
        public ITransactionRepository TransactionRepository { get; }
        public IInventoryRepository InventoryRepository { get; }
        public IContactRepository ContactRepository { get; }
        public IProductRepository ProductRepository { get; }
        public InventoryService(
            ITransactionRepository transactionRepository,
            IInventoryRepository inventoryRepository,
            IContactRepository contactRepository,
            IProductRepository productRepository) 
        {
            TransactionRepository = transactionRepository;
            InventoryRepository = inventoryRepository;
            ContactRepository = contactRepository;
            ProductRepository = productRepository;
        }

        public Task<PageData<InventoryQuery>> FindInventory(PageQuery pageQuery)
        {
            return InventoryRepository.FindInventory(pageQuery);
        }

        public void InventoryMovement(ProductInventoryCommand command, long userOn)
        {
            var product = ProductRepository.Find(command.ProductId) ?? throw new ValidateException(Messages.ProductNotFound);

            if ((command.Operation == EOperation.Compra) || (command.Operation == EOperation.Producao))
                GenerateBuyTransaction(command, product, userOn);
            else if ((command.Operation == EOperation.Venda) || (command.Operation == EOperation.Consumo))
                GenerateSellTransaction(command, product, userOn);
        }

        public void CreateProductInInventory(EOperation operation, long productId, decimal amount)
        {
            var movement = InventoryRepository.FindByProduct(productId);

            if (movement == null)
            {
                InventoryRepository.Add(new AppInventory
                {
                    ProductId = productId,
                    Amount = amount
                });
            }
            else
            {
                if((operation == EOperation.Compra) || operation == EOperation.Producao)
                    movement.Amount = movement.Amount + amount;
                if((operation == EOperation.Venda) || operation == EOperation.Consumo)
                    movement.Amount = movement.Amount - amount;

                InventoryRepository.Update(movement);
            }
        }

        public void GenerateBuyTransaction(ProductInventoryCommand command, AppProduct product, long userOn)
        {
            CreateProductInInventory(command.Operation, product.Id, command.Amount);

            if(command.ContactId != null) { 
                var contact = ContactRepository.Find(command.ContactId.GetValueOrDefault(0)) ?? throw new ValidateException(Messages.ContactNotFound);

                if (contact.PersonTypeId != (int)EPersonType.LegalPerson)
                    throw new ValidateException(Messages.ContactIsNotLegal);
            }

            TransactionRepository.Add(new AppTransaction
            {
                ContactOriginId = command.Operation == EOperation.Compra ? command.ContactId : null,
                ContactDestinationId = userOn,
                TotalPrice = (product.Price * command.Amount),
                Amount = command.Amount,
                OperationId = command.Operation == EOperation.Compra ? (int)EOperation.Compra : (int)EOperation.Producao
            });
        }

        public void GenerateSellTransaction(ProductInventoryCommand command, AppProduct product, long userOn)
        {
            CreateProductInInventory(command.Operation, product.Id, command.Amount);

            if (!ContactRepository.Exists(command.ContactId.GetValueOrDefault(0)))
                throw new ValidateException(Messages.ContactNotFound);

            TransactionRepository.Add(new AppTransaction
            {
                ContactDestinationId = command.Operation == EOperation.Venda ? command.ContactId : null,
                ContactOriginId = userOn,
                TotalPrice = (product.Price * command.Amount),
                Amount = command.Amount,
                OperationId = command.Operation == EOperation.Venda ? (int)EOperation.Venda : (int)EOperation.Producao
            });
        }
    }
}
