using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public interface ITicketRepo
    {
        Ticket GetTicket(int Id);
        IEnumerable<Ticket> GetAllTickets();
        IEnumerable<Ticket> GetAllTicketsInCompany(int companyId);
        Ticket Add(Ticket ticket);
        Ticket Update(Ticket ChangedTicket);
        Ticket Delete(int Id);
        SelectList GetListofDepartments(int companyId);
        SelectList GetListofPriorites();
        SelectList GetListofUsers();
        IEnumerable<Ticket> GetTicketsByUser(string AssignedToId);
        IEnumerable<Ticket> GetTicketsByUserId(string userId);
        //Department GetDepartment(int departmentId);
        Note CreateNote(Note note);
        TicketLogs CreateTicketLog(TicketLogs log);
        IEnumerable<TicketLogs> GetAllTicketLogs();
        IEnumerable<Note> GetAllNotes();
        IEnumerable<Ticket> ShowAllTickets();
        Attachment AddAttachment(Attachment attachment);
        IEnumerable<Attachment> GetAttachments();
        Attachment GetAttachment(int id);
    }
}
