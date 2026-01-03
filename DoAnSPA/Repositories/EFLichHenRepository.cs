using DoAnSPA.Data;
using DoAnSPA.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EFLichHenRepository : ILichHenRepository
{
    private readonly SpaDbContext _context;

    public EFLichHenRepository(SpaDbContext context)
    {
        _context = context;
    }

    // Async Methods
    public async Task<IEnumerable<LichHen>> GetAllAsync()
    {
        return await _context.LichHens.ToListAsync();
    }

    public async Task<LichHen> GetByIdAsync(int id)
    {
        return await _context.LichHens.FindAsync(id);
    }

    public async Task AddAsync(LichHen lichHen)
    {
        _context.LichHens.Add(lichHen);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LichHen lichHen)
    {
        _context.LichHens.Update(lichHen);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var lichHen = await _context.LichHens.FindAsync(id);
        if (lichHen != null)
        {
            _context.LichHens.Remove(lichHen);
            await _context.SaveChangesAsync();
        }
    }

    // Sync Methods (BẮT BUỘC để đúng interface)
    public IEnumerable<LichHen> GetAll()
    {
        return _context.LichHens.ToList();
    }

    public LichHen GetById(int id)
    {
        return _context.LichHens.Find(id);
    }

    public void Add(LichHen lichHen)
    {
        _context.LichHens.Add(lichHen);
        _context.SaveChanges();
    }

    public void Update(LichHen lichHen)
    {
        _context.LichHens.Update(lichHen);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var lichHen = _context.LichHens.Find(id);
        if (lichHen != null)
        {
            _context.LichHens.Remove(lichHen);
            _context.SaveChanges();
        }
    }
}
