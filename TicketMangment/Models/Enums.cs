using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public enum Enums
    {

    }
    public enum TicketStatus
    {
        Open = 1,
        Closed = 2,
        Solved = 3,
        Asssigned = 4,
        OnHold = 5,
        ReOpened = 6,
        InProgress = 7
    }

    public enum RecordStatus
    {
        notdeleted = 0,
        deleted = 1
    }

    public enum InDepRole
    {
        None = 0,
        Admin = 1,
        Employee = 2
    }

}
