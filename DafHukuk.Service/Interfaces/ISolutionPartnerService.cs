using DafHukuk.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DafHukuk.Service.Interfaces
{
    public interface ISolutionPartnerService
    {
        Task<List<SolutionPartner>> GetAll();
        Task<SolutionPartner?> GetById(int id);
        Task<SolutionPartner> Create(SolutionPartner partner);
        Task<SolutionPartner?> Update(int id, SolutionPartner partner);
        Task<bool> Delete(int id);
    }
}