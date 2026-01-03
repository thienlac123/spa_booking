using DoAnSPA.Data;
using DoAnSPA.Models;
using DoAnSPA.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EFNhanVienRepository : INhanVienRepository
{
    private readonly SpaDbContext _context;

    public EFNhanVienRepository(SpaDbContext context)
    {
        _context = context;
    }

    // Async Methods
    public async Task<IEnumerable<NhanVien>> GetAllAsync()
    {
        return await _context.NhanViens.ToListAsync();
    }

    public async Task<NhanVien> GetByIdAsync(int id)
    {
        return await _context.NhanViens.FindAsync(id);
    }

    public async Task AddAsync(NhanVien nhanVien)
    {
        _context.NhanViens.Add(nhanVien);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(NhanVien nhanVien)
    {
        _context.NhanViens.Update(nhanVien);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var nv = await _context.NhanViens.FindAsync(id);
        if (nv != null)
        {
            _context.NhanViens.Remove(nv);
            await _context.SaveChangesAsync();
        }
    }

    // Sync Methods BẮT BUỘC để đúng interface
    public IEnumerable<NhanVien> GetAll()
    {
        return _context.NhanViens.ToList();
    }

    public NhanVien GetById(int id)
    {
        return _context.NhanViens.Find(id);
    }

    public void Add(NhanVien nhanVien)
    {
        _context.NhanViens.Add(nhanVien);
        _context.SaveChanges();
    }

    public void Update(NhanVien nhanVien)
    {
        _context.NhanViens.Update(nhanVien);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var nv = _context.NhanViens.Find(id);
        if (nv != null)
        {
            _context.NhanViens.Remove(nv);
            _context.SaveChanges();
        }
    }
}
