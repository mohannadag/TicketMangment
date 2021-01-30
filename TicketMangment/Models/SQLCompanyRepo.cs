using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class SQLCompanyRepo : ICompanyRepo
    {
        private readonly AppDbContext context;

        public SQLCompanyRepo(AppDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Company> GetCompanies()
        {
            return context.Companies;
        }

        public Company GetCompany(int id)
        {
            return context.Companies.Find(id);
        }

        public Company Add(Company company)
        {
            context.Companies.Add(company);
            context.SaveChanges();
            return company;
        }

        public Company Update(Company ChangedCompany)
        {
            var company = context.Companies.Attach(ChangedCompany);
            company.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return ChangedCompany;
        }

        public Company Delete(int Id)
        {
            Company company = context.Companies.Find(Id);
            if (company != null)
            {
                context.Companies.Remove(company);
                context.SaveChanges();
            }
            return company;
        }
    }
}
