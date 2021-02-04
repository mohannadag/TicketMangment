using System.Collections.Generic;

namespace TicketMangment.Models
{
    public interface ICompanyRepo
    {
        Company Add(Company company);
        Company Delete(int Id);
        IEnumerable<Company> GetCompanies();
        Company GetCompany(int id);
        Company Update(Company ChangedCompany);
    }
}