using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class SQLPriorityRepo : IPriorityRepo
    {
        private readonly AppDbContext context;

        public SQLPriorityRepo(AppDbContext context)
        {
            this.context = context;
        }
        public Priority Add(Priority priority)
        {
            context.Priorities.Add(priority);
            context.SaveChanges();
            return priority;
        }

        public Priority Delete(int id)
        {
            Priority priority = context.Priorities.Find(id);
            if(priority != null)
            {
                context.Priorities.Remove(priority);
                context.SaveChanges();
            }
            return priority;
        }

        public IEnumerable<Priority> GetAllPriority()
        {
            return context.Priorities.Where(p => p.RecordStatus == RecordStatus.notdeleted);
        }

        public IEnumerable<Priority> ShowAllPriority()
        {
            return context.Priorities;
        }

        public Priority GetPriority(int id)
        {
            return context.Priorities.Find(id);
        }

        public Priority Update(Priority Changedpriority)
        {
            var priorty = context.Priorities.Attach(Changedpriority);
            priorty.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return Changedpriority;
        }
    }
}
