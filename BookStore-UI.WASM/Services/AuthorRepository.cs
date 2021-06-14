using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using BookStore_UI.WASM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStore_UI.WASM.Services
{
    public class AuthorRepository : BaseRepository<Author>,IAuthorRepository
    {
        private readonly HttpClient client_;
        private readonly ILocalStorageService localStorage_;

        public AuthorRepository(HttpClient client, 
            ILocalStorageService localStorage) : base(client, localStorage)
        {
            client_ = client;
            localStorage_ = localStorage;
        }
    }
}
