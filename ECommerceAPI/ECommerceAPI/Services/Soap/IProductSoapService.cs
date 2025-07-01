using System.ServiceModel;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services.Soap
{
    [ServiceContract(Namespace = "http://ecommerceapi.example.com/soap")]
    public interface IProductSoapService
    {
        [OperationContract]
        List<Product> GetAllProducts();
        
        [OperationContract]
        Product GetProductById(int id);
        
        [OperationContract]
        void AddProduct(Product product);
        
        [OperationContract]
        void UpdateProduct(Product product);
        
        [OperationContract]
        bool DeleteProduct(int id);
    }
}