using DoAnSPA.Data;
using DoAnSPA.Models;
using DoAnSPA.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EFPhanHoiRepository : IPhanHoiRepository
{
    private readonly SpaDbContext _context;

    public EFPhanHoiRepository(SpaDbContext context)
    {
        _context = context;
    }

    // Async Methods
    public async Task<IEnumerable<PhanHoi>> GetAllAsync()
    {
        return await _context.PhanHois.ToListAsync();
    }

    public async Task<PhanHoi> GetByIdAsync(int id)
    {
        return await _context.PhanHois.FindAsync(id);
    }

    public async Task AddAsync(PhanHoi phanHoi)
    {
        _context.PhanHois.Add(phanHoi);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PhanHoi phanHoi)
    {
        _context.PhanHois.Update(phanHoi);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var ph = await _context.PhanHois.FindAsync(id);
        if (ph != null)
        {
            _context.PhanHois.Remove(ph);
            await _context.SaveChangesAsync();
        }
    }

    // Sync Methods - BẮT BUỘC để đúng interface
    public IEnumerable<PhanHoi> GetAll()
    {
        return _context.PhanHois.ToList();
    }

    public PhanHoi GetById(int id)
    {
        return _context.PhanHois.Find(id);
    }

    public void Add(PhanHoi phanHoi)
    {
        _context.PhanHois.Add(phanHoi);
        _context.SaveChanges();
    }

    public void Update(PhanHoi phanHoi)
    {
        _context.PhanHois.Update(phanHoi);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var ph = _context.PhanHois.Find(id);
        if (ph != null)
        {
            _context.PhanHois.Remove(ph);
            _context.SaveChanges();
        }
    }
}
