using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public interface IDepartmentRepo
    {
        Department GetDepartment(int Id);
        IEnumerable<Department> GetAllDepartments();
        IEnumerable<Department> GetAllDepartmentsInCompany(int companyId);
        Department Add(Department department);
        Department Update(Department ChangedDepartment);
        Department Delete(int id);
        IEnumerable<Department> ShowAllDepartments();
    }
}
