using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetCartAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(
            userId,
            cancellationToken);

        return MapToDto(cart);
    }

    public async Task<CartDto> AddItemAsync(
        Guid userId,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            throw new InvalidOperationException(
                "Product does not exist.");
        }

        if (!product.IsActive)
        {
            throw new InvalidOperationException(
                "Product is not available.");
        }

        if (product.StockQuantity < request.Quantity)
        {
            throw new InvalidOperationException(
                "Not enough stock is available.");
        }

        var cart = await GetOrCreateCartAsync(
            userId,
            cancellationToken);

        var existingItem = cart.Items.FirstOrDefault(
            item => item.ProductId == request.ProductId);

        if (existingItem is not null)
        {
            var newQuantity =
                existingItem.Quantity + request.Quantity;

            if (newQuantity > product.StockQuantity)
            {
                throw new InvalidOperationException(
                    "Requested quantity exceeds available stock.");
            }

            existingItem.UpdateQuantity(newQuantity);
        }
        else
        {
            cart.Items.Add(
                new CartItem(
                    cart.Id,
                    product.Id,
                    request.Quantity));
        }

        cart.UpdateTimestamp();

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return MapToDto(cart);
    }

    private async Task<ShoppingCart> GetOrCreateCartAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (cart is not null)
        {
            return cart;
        }

        cart = new ShoppingCart(userId);

        await _cartRepository.AddAsync(
            cart,
            cancellationToken);

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return cart;
    }

    private static CartDto MapToDto(
        ShoppingCart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.Items
                .Select(item => new CartItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    SKU = item.Product.SKU,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Quantity
                })
                .ToList()
        };
    }
}