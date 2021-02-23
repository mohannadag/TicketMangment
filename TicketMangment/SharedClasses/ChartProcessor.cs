using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.SharedClasses
{
    public static class ChartProcessor
    {
        public static ChartClass PopulatChart(ITicketRepo ticketRepo, int companyId)
        {
            List<string> liststring = new List<string>();
            List<int> solvedTickets = new List<int>();
            List<int> allTickets = new List<int>();
            var tickets = ticketRepo.GetAllTicketsInCompany(companyId);
            DateTime date = DateTime.Now.AddYears(-1);
            for (int i = 1; i <= 12; i++)
            {
                liststring.Add((date.AddMonths(i)).ToString("MMM"));
                allTickets.Add((tickets.Where(t => t.CreateDate >= date.AddMonths(i - 1) && t.CreateDate <= date.AddMonths(i))).Count());
                solvedTickets.Add((tickets.Where(t => t.TicketStatus == TicketStatus.Solved &&
                        t.CreateDate >= date.AddMonths(i - 1) && t.CreateDate <= date.AddMonths(i))).Count());
            }

            ChartClass chart = new ChartClass
            {
                Labels = liststring.ToArray(),
                AllTicketsValues = allTickets.ToArray(),
                SolvedTicketsValues = solvedTickets.ToArray()
            };
            return chart;
        }

        public static Chart2Class PopulatChart(IDepartmentRepo departmentRepo, ITicketRepo ticketRepo, int companyId)
        {
            List<string> departmentsNames = new List<string>();
            List<int> ticketsNumber = new List<int>();

            foreach (var dep in departmentRepo.GetAllDepartmentsInCompany(companyId))
            {
                departmentsNames.Add(dep.DepartmentName);
                ticketsNumber.Add(ticketRepo.GetAllTicketsInCompany(companyId).Where(t => t.DepartmentId == dep.DepartmentId).Count());
            }
            Chart2Class chart = new Chart2Class
            {
                Labels = departmentsNames.Take(4).ToArray(),
                TicketsCount = ticketsNumber.Take(4).ToArray()
            };

            return chart;
        }

    }
}
