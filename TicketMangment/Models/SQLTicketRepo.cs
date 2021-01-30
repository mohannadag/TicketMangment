using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class SQLTicketRepo : ITicketRepo
    {
        private readonly AppDbContext context;

        public SQLTicketRepo(AppDbContext context)
        {
            this.context = context;
        }

        public Ticket Add(Ticket ticket)
        {
            //ticket.User = context.Users.Find(ticket.UserId);
            context.Tickets.Add(ticket);
            context.SaveChanges();
            return ticket;
        }

        public Ticket Delete(int Id)
        {
            Ticket ticket = context.Tickets.Find(Id);
            if (ticket != null)
            {
                context.Tickets.Remove(ticket);
                context.SaveChanges();
            }
            return ticket;
        }

        public IEnumerable<Ticket> GetAllTickets()
        {
            return (context.Tickets.Include(t => t.Department).Include(t => t.Priority).AsNoTracking()).ToList();
        }

        public IEnumerable<Ticket> GetAllTicketsInCompany(int companyId)
        {
            // return context.Tickets.Where(t => t.RecordStatus == RecordStatus.notdeleted && t.CompanyId == companyId);

            return context.Tickets.Include(t => t.Department)
                                  .Include(t => t.Priority)
                                  .AsNoTracking().Where(t => t.RecordStatus == RecordStatus.notdeleted && t.CompanyId == companyId)
                                  .ToList();
        }

        public IEnumerable<Ticket> ShowAllTickets()
        {
            return context.Tickets;
        }

        public Ticket GetTicket(int Id)
        {
            var ticket = context.Tickets
                .Include(t => t.Department)
                .Include(t => t.Priority)
                .Include(t => t.User)
                .Include(t => t.AssignedUser)
                .FirstOrDefault(r => r.TicketId == Id);

            //var ticket = context.Tickets.Find(Id);

            //var ticket = context.Tickets.Find(Id);
            ////ticket.Department = context.Departments.Find(ticket.DepartmentId);  // I can use this or the method GetDepartment in the controller
            //ticket.Priority = context.Priorities.Find(ticket.PriorityId);
            //ticket.User = context.Users.Find(ticket.UserId);
            //ticket.AssignedUser = context.Users.Find(ticket.AssignedTo);
            ////ticket.CreatedBy = ticket.User.UserName;
            return ticket;
        }

        public Ticket Update(Ticket ChangedTicket)
        {
            var ticket = context.Tickets.Attach(ChangedTicket);
            ticket.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return ChangedTicket;
        }

        public SelectList GetListofDepartments()
        {
            var departmentList = new SelectList(context.Departments.Where(
                d => d.RecordStatus == RecordStatus.notdeleted),
                "DepartmentId", "DepartmentName");
            return departmentList;
        }

        public SelectList GetListofPriorites()
        {
            var prioritiesList = new SelectList(context.Priorities.Where(
                p => p.RecordStatus == RecordStatus.notdeleted),
                "PriorityId", "PriorityName");
            return prioritiesList;
        }

        public SelectList GetListofUsers()
        {
            var UsersList = new SelectList(context.Users, "Id", "UserName");
            return UsersList;
        }

        public IEnumerable<Ticket> GetTicketsByUser(string AssignedToId)
        {
            return context.Tickets
                .Include(t => t.Department)
                .Include(t => t.Priority)
                .Include(t => t.User)
                .Where(t => t.AssignedTo == AssignedToId);
        }

        public IEnumerable<Ticket> GetTicketsByUserId(string userId)
        {
            return context.Tickets
                .Include(t => t.Department)
                .Include(t => t.Priority)
                .Include(t => t.User)
                .Where(t => t.UserId == userId);
        }

        // There is no need for doing this any more
        //public Department GetDepartment(int departmentId)
        //{
        //    return context.Departments.FirstOrDefault(d => d.DepartmentId == departmentId);
        //}

        public Note CreateNote(Note note)
        {
            context.Notes.Add(note);
            context.SaveChanges();
            return note;
        }

        public IEnumerable<Note> GetAllNotes()
        {
            return context.Notes;
        }

        public TicketLogs CreateTicketLog(TicketLogs log)
        {
            context.TicketLogs.Add(log);
            context.SaveChanges();
            return log;
        }

        public IEnumerable<TicketLogs> GetAllTicketLogs()
        {
            return context.TicketLogs;
        }

        public Attachment AddAttachment(Attachment attachment)
        {
            context.Attachments.Add(attachment);
            context.SaveChanges();
            return attachment;
        }

        public IEnumerable<Attachment> GetAttachments()
        {
            return context.Attachments;
        }

        public Attachment GetAttachment(int id)
        {
            return context.Attachments.FirstOrDefault(a => a.Id == id);
        }

    }
}
