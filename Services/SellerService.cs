using Microsoft.EntityFrameworkCore;
using SalesWebMVC.Data;
using SalesWebMVC.Models;
using websalesmvc.Services.Exceptions;

namespace SalesWebMVC.Services;

public class SellerService
{
    private readonly SalesWebMVCContext _context;

    public SellerService(SalesWebMVCContext context)
    {
        _context = context;
    }

    public async Task<List<Seller>> FindAllAsync()
    {
        return await _context.Seller.ToListAsync();
    }

    public async Task InsertAsync(Seller obj)
    {
        int highestId = _context.Seller.Max(
            sl => (int?)sl.Id) ?? 0;
        
        int nextId = highestId + 1;
        
        obj.Id = nextId;
        _context.Add(obj);
        await _context.SaveChangesAsync();
    }

    public async Task<Seller> FindByIdAsync(int id) 
    {
        return await _context.Seller.Include(obj => obj.Department).FirstOrDefaultAsync(
            obj => obj.Id == id
        );
    }

    public async Task RemoveAsync(int id)
    {
        var obj = await  _context.Seller.FindAsync(id);
        _context.Seller.Remove(obj);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Seller obj)
    {
        bool hasAny = await _context.Seller.AnyAsync(x => x.Id == obj.Id); 
        if (!hasAny)
        {
             throw new NotFoundException("Seller not found! ");
        }
        try
        {
            _context.Update(obj);
            await _context.SaveChangesAsync ();
        }
        catch (DbConcurrencyException e)
        {
            
            throw new DbConcurrencyException(e.Message);
        } 
    }
} 