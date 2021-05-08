using Book_Store_API.Contract;
using Book_Store_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Store_API.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext db_;
        public AuthorRepository(ApplicationDbContext db)
        {
            db_ = db;
        }
        public async Task<bool> Create(Author entity)
        {
            await db_.Authors.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(Author entity)
        {
            db_.Authors.Remove(entity);
            return await Save();
        }

        public async Task<IList<Author>> FindAll()
        {
            var authors = await db_.Authors.ToListAsync();
            return authors;
        }

        public async Task<Author> FindById(int id)
        {
            var author = await db_.Authors.FindAsync(id);
            return author;
        }

        public async Task<bool> Save()
        {
            var changes = await db_.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Author entity)
        {
            db_.Authors.Update(entity);
            return await Save();
        }
    }
}
