using DafHukuk.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DafHukuk.Service.Interfaces
{
    public interface ILawyerService
    {
        Task<List<Lawyer>> GetAll();
        Task<Lawyer?> GetById(int id);
        Task<Lawyer?> GetBySlug(string slug, string language);
        Task<Lawyer> Create(Lawyer lawyer);
        Task<Lawyer?> Update(int id, Lawyer lawyer);
        Task<bool> Delete(int id);
    }
}