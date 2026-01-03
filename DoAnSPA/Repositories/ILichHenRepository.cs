using DoAnSPA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILichHenRepository
{
    Task<IEnumerable<LichHen>> GetAllAsync();
    Task<LichHen> GetByIdAsync(int id);
    Task AddAsync(LichHen lichHen);
    Task UpdateAsync(LichHen lichHen);
    Task DeleteAsync(int id);

    // BẮT BUỘC viết thêm các hàm Sync
    IEnumerable<LichHen> GetAll();
    LichHen GetById(int id);
    void Add(LichHen lichHen);
    void Update(LichHen lichHen);
    void Delete(int id);
}
