using Book_Store_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Store_API.Contract
{
    public interface IBookRepository : IRepositoryBase<Book>
    {
    }
}
