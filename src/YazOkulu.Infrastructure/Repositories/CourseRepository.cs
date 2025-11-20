using Microsoft.EntityFrameworkCore;
using YazOkulu.Domain.Entities;
using YazOkulu.Infrastructure.Data;

namespace YazOkulu.Infrastructure.Repositories;

public class CourseRepository : GenericRepository<Course>
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Course?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Applications)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.Applications)
            .ToListAsync();
    }
}
