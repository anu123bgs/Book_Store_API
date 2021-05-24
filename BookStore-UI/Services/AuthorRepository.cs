using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using BookStore_UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStore_UI.Services
{
    public class AuthorRepository : BaseRepository<Author>,IAuthorRepository
    {
        private readonly IHttpClientFactory client_;
        private readonly ILocalStorageService localStorage_;

        public AuthorRepository(IHttpClientFactory client, 
            ILocalStorageService localStorage) : base(client, localStorage)
        {
            client_ = client;
            localStorage_ = localStorage;
        }
    }
}
