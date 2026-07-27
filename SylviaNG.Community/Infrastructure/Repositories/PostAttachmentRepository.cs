using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class PostAttachmentRepository : Repository<PostAttachment>, IPostAttachmentRepository
    {
        public PostAttachmentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<PostAttachment>> GetByPostIdAsync(long postId)
        {
            return await _dbSet.Where(a => a.PostId == postId).ToListAsync();
        }
    }
}
