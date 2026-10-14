using Restaurant.Application.DTOs;

namespace Restaurant.Web.Services
{
    public interface IMonAnApiService
    {
        Task<List<MonAnDto>> GetAllAsync();
    }
}