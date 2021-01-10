using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class SQLDepartmentRepo : IDepartmentRepo
    {
        private readonly AppDbContext context;

        public SQLDepartmentRepo(AppDbContext context)
        {
            this.context = context;
        }
        public Department Add(Department department)
        {
            context.Departments.Add(department);
            context.SaveChanges();
            return department;
        }

        public Department Delete(int id)
        {
            Department department = context.Departments.Find(id);
            if(department != null)
            {
                context.Departments.Remove(department);
                context.SaveChanges();
            }
            return department;
        }

        public IEnumerable<Department> GetAllDepartments()
        {
            return context.Departments.Where(d => d.RecordStatus == RecordStatus.notdeleted);
        }

        public IEnumerable<Department> ShowAllDepartments()
        {
            return context.Departments;
        }

        public Department GetDepartment(int Id)
        {
            return context.Departments.Find(Id);
        }

        public Department Update(Department ChangedDepartment)
        {
            var department = context.Departments.Attach(ChangedDepartment);
            department.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return ChangedDepartment;
        }
    }
}
