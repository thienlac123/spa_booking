// DoAnSPA/Repositories/IKhachHangRepository.cs
using DoAnSPA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnSPA.Repositories
{
    public interface IKhachHangRepository
    {
        Task<IEnumerable<CustomerProfile>> GetAllAsync();
        Task<CustomerProfile?> GetByUserIdAsync(string userId);
        Task AddAsync(CustomerProfile profile);
        Task UpdateAsync(CustomerProfile profile);
        Task DeleteAsync(string userId);
    }
}
