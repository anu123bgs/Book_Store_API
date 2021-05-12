using Book_Store_API.Contract;
using Book_Store_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Store_API.Services
{
    public class BookRepository: IBookRepository
    {
        private readonly ApplicationDbContext db_;
        public BookRepository(ApplicationDbContext db)
        {
            db_ = db;
        }
        
        public async Task<bool> Create(Book entity)
        {
            await db_.Books.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(Book entity)
        {
            db_.Books.Remove(entity);
            return await Save();
        }

        public async Task<IList<Book>> FindAll()
        {
            var books = await db_.Books.ToListAsync();
            return books;
        }

        public async Task<Book> FindById(int id)
        {
            var book = await db_.Books.FindAsync(id);
            return book;
        }

        public async Task<bool> IsExists(int Id)
        {
            return await db_.Books.AnyAsync(q => q.Id == Id);
        }

        public async Task<bool> Save()
        {
            var changes = await db_.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Book entity)
        {
            db_.Books.Update(entity);
            return await Save();
        }
    }
}
