using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public interface IPriorityRepo
    {
        Priority GetPriority(int id);
        IEnumerable<Priority> GetAllPriority();
        Priority Add(Priority priority);
        Priority Update(Priority Changedpriority);
        Priority Delete(int id);
        IEnumerable<Priority> ShowAllPriority();
    }
}
