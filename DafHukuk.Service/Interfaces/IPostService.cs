using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DafHukuk.Core.Entities;

namespace DafHukuk.Service.Interfaces
{
    public interface IPostService
    {
        Task<List<Post>> GetAll(int? categoryId = null);
        Task<List<Post>> GetByServiceType(ServiceType serviceType);
        Task<Post?> GetById(int id);
        Task<Post> Create(Post post);
        Task<Post?> Update(int id, Post post);
        Task<bool> Delete(int id);
        Task<List<Post>> Search(string query);
    }
}