using DoAnSPA.Data;
using DoAnSPA.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EFDichVuRepository : IDichVuRepository
{
    private readonly SpaDbContext _context;

    public EFDichVuRepository(SpaDbContext context)
    {
        _context = context;
    }

    // Async Methods
    public async Task<IEnumerable<DichVu>> GetAllAsync()
    {
        return await _context.DichVus.ToListAsync();
    }

    public async Task<DichVu> GetByIdAsync(int id)
    {
        return await _context.DichVus.FindAsync(id);
    }

    public async Task AddAsync(DichVu dichVu)
    {
        _context.DichVus.Add(dichVu);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DichVu dichVu)
    {
        _context.DichVus.Update(dichVu);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var dichVu = await _context.DichVus.FindAsync(id);
        if (dichVu != null)
        {
            _context.DichVus.Remove(dichVu);
            await _context.SaveChangesAsync();
        }
    }

    // Sync Methods - BẮT BUỘC VIẾT ĐỦ
    public IEnumerable<DichVu> GetAll()
    {
        return _context.DichVus.ToList();
    }

    public DichVu GetById(int id)
    {
        return _context.DichVus.Find(id);
    }

    public void Add(DichVu dichVu)
    {
        _context.DichVus.Add(dichVu);
        _context.SaveChanges();
    }

    public void Update(DichVu dichVu)
    {
        _context.DichVus.Update(dichVu);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var dichVu = _context.DichVus.Find(id);
        if (dichVu != null)
        {
            _context.DichVus.Remove(dichVu);
            _context.SaveChanges();
        }
    }
}
