using DoAnSPA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDichVuRepository
{
    Task<IEnumerable<DichVu>> GetAllAsync();
    Task<DichVu> GetByIdAsync(int id);
    Task AddAsync(DichVu dichVu);
    Task UpdateAsync(DichVu dichVu);
    Task DeleteAsync(int id);

    // Nếu có dạng đồng bộ (sync) cũng cần viết:
    IEnumerable<DichVu> GetAll();
    DichVu GetById(int id);
    void Add(DichVu dichVu);
    void Update(DichVu dichVu);
    void Delete(int id);
}
