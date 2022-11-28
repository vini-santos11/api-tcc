namespace Domain.Commands.Product
{
    public class ProductCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultMeansurement { get; set; }
        public decimal Price { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageUrl { get; set; }
    }
}
