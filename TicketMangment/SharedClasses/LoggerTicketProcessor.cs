using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.SharedClasses
{
    public static class LoggerTicketProcessor 
    {
        //private readonly ITicketRepo ticketRepo;

        //public LoggerTicketProcessor(ITicketRepo ticketRepo)
        //{
        //    this.ticketRepo = ticketRepo;
        //}
        public static void LogCreatedTicket(int ticketId, ApplicationUser user, ITicketRepo ticketRepo)
        {
            TicketLogs ticketLogs = new TicketLogs
            {
                TicketId = ticketId,
                UserLog = user.Id,
                Message = $"The ticket has been created by {user.UserName}",
                LogDate = DateTime.UtcNow
            };
            ticketRepo.CreateTicketLog(ticketLogs);
        }

        public static void LogEditedTicket(int ticketId, ApplicationUser user, ITicketRepo ticketRepo)
        {
            TicketLogs ticketLogs = new TicketLogs
            {
                TicketId = ticketId,
                UserLog = user.Id,
                Message = $"The ticket has been modified by {user.UserName}",
                LogDate = DateTime.UtcNow
            };
            ticketRepo.CreateTicketLog(ticketLogs);
        }
    }
}
