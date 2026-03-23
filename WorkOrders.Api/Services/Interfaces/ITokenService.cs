using WorkOrders.Api.Models;

namespace WorkOrders.Api.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}