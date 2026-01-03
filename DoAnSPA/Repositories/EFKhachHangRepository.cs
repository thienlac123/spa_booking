// DoAnSPA/Repositories/EFKhachHangRepository.cs
using DoAnSPA.Data;
using DoAnSPA.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnSPA.Repositories
{
    public class EFKhachHangRepository : IKhachHangRepository
    {
        private readonly SpaDbContext _context;

        public EFKhachHangRepository(SpaDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ hồ sơ kèm user (Identity)
        public async Task<IEnumerable<CustomerProfile>> GetAllAsync()
        {
            return await _context.CustomerProfiles
                                 .Include(p => p.User)   // SpaUser
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        // Lấy theo khóa chính (UserId = AspNetUsers.Id)
        public async Task<CustomerProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.CustomerProfiles
                                 .Include(p => p.User)
                                 .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task AddAsync(CustomerProfile profile)
        {
            _context.CustomerProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomerProfile profile)
        {
            _context.CustomerProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            var p = await _context.CustomerProfiles.FindAsync(userId);
            if (p != null)
            {
                _context.CustomerProfiles.Remove(p);
                await _context.SaveChangesAsync();
            }
        }
    }
}
