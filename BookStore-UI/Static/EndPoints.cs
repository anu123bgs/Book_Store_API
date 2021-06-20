using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Static
{
    public static class EndPoints
    {
#if DEBUG
        public static string BaseUrl = "https://localhost:44343/";
#else
        public static string BaseUrl = "";
#endif
        public static string AuthorsEndpoint = $"{BaseUrl}api/authors/";
        public static string BooksEndpoint = $"{BaseUrl}api/books/";
        public static string RegisterEndpoint = $"{BaseUrl}api/users/register/";
        public static string LoginEndpoint = $"{BaseUrl}api/users/login/";
    }
}
