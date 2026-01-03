using DoAnSPA.Models;

namespace DoAnSPA.Repositories
{
    public interface INhanVienRepository
    {
        Task<IEnumerable<NhanVien>> GetAllAsync();
        Task<NhanVien> GetByIdAsync(int id);
        Task AddAsync(NhanVien nhanVien);
        Task UpdateAsync(NhanVien nhanVien);
        Task DeleteAsync(int id);

        IEnumerable<NhanVien> GetAll();
        NhanVien GetById(int id);
        void Add(NhanVien nhanVien);
        void Update(NhanVien nhanVien);
        void Delete(int id);

    }

}
