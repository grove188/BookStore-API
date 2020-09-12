using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using BookStore_UI.Models;
using BookStore_UI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStore_UI.Services
{
   public class BookRepo : BaseRepository<Book>, IBookRepository
   {
      private readonly IHttpClientFactory _client;
      private readonly ILocalStorageService _localStorage;

      public BookRepo(IHttpClientFactory client, ILocalStorageService localStorage) : base(client, localStorage)
      {
         _client = client;
         _localStorage = localStorage;
      }
   }
}
