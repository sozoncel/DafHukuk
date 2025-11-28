using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DafHukuk.Core.Entities;

namespace DafHukuk.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAll();
        Task<Category?> GetById(int id);
        Task<Category> Create(Category category);
        Task<Category?> Update(int id, Category category);
        Task<bool> Delete(int id);
    }
}
