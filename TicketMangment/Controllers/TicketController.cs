using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketMangment.Models;
using TicketMangment.ViewModel;
using MailKit.Net.Smtp;
using MimeKit;
using TicketMangment.SharedClasses;

namespace TicketMangment.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ITicketRepo ticketRepo;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDepartmentRepo departmentRepo;
        private readonly IPriorityRepo priorityRepo;
        private readonly IWebHostEnvironment hostingEnvironment;

        public TicketController(ITicketRepo ticketRepo, UserManager<ApplicationUser> userManager,
            IDepartmentRepo departmentRepo, IPriorityRepo priorityRepo, IWebHostEnvironment hostingEnvironment)
        {
            this.ticketRepo = ticketRepo;
            this.userManager = userManager;
            this.departmentRepo = departmentRepo;
            this.priorityRepo = priorityRepo;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: TicketController
        public async Task<ActionResult> Index([Optional] string getAllTickets)
        {
            int companyId = (await GetCurrentUser()).CompanyId;

            if (getAllTickets != null)
            {
                if (getAllTickets.ToLower() == "all")
                { 
                    var model = ticketRepo.GetAllTicketsInCompany(companyId);
                    return View(model);
                }
                else
                {
                    var model = ticketRepo.GetAllTicketsInCompany(companyId)
                        .Where(t => t.RecordStatus == RecordStatus.notdeleted);
                    return View(model);
                }
            }
            else
            {
                var model = ticketRepo.GetAllTicketsInCompany(companyId).
                    Where(t => t.RecordStatus == RecordStatus.notdeleted);
                return View(model);
            }

        }

        // GET: TicketController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            Ticket ticket = ticketRepo.GetTicket(id);
            
            if(ticket == null || ticket.RecordStatus == RecordStatus.deleted || ((await GetCurrentUser()).CompanyId) != ticket.CompanyId)
            {
                Response.StatusCode = 404;
                ViewBag.ErrorMessage = "Ticket with id = " + id + " is not found";
                return View("NotFound");
            }
            //ViewBag.path = HttpContext.Request.Path;
            ticket.Notes = ticketRepo.GetAllNotes().Where(t => t.TicketId == id).ToList();

            // this viewbag to get the returned url
            ViewBag.returnUrl = Request.Headers["Referer"].ToString();

            return View(ticket);
        }

        // GET: TicketController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.departments = ticketRepo.GetListofDepartments((await GetCurrentUser()).CompanyId);
            ViewBag.priorites = ticketRepo.GetListofPriorites();
            
            return View();
        }

        // POST: TicketController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TicketCreatViewModel model)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await GetCurrentUser();

                    Ticket newticket = new Ticket
                    {
                        Subject = model.Subject,
                        RequestDetail = model.RequestDetail,
                        Location = model.Location,
                        CreateDate = DateTime.Now,
                        CreatedBy = user.Id,
                        UserId = user.Id,
                        DepartmentId = model.DepartmentId,
                        PriorityId = model.PriorityId,
                        AssignedTo = model.AssignedTo,
                        TicketStatus = TicketStatus.Open,
                        RecordStatus = RecordStatus.notdeleted,
                        CompanyId = user.CompanyId
                    };
                    Ticket ticket = ticketRepo.Add(newticket);

                    LoggerTicketProcessor.LogCreatedTicket(ticket.TicketId, user, ticketRepo);
                    // TODO: create new class to handle taking notes when the note class complete (attachment,worktime,...)
                    if (model.NoteBody != null)
                    { 
                        Note note = new Note
                        {
                            NoteBody = model.NoteBody,
                            TicketId = newticket.TicketId
                        };
                        ticketRepo.CreateNote(note);
                    }
                    return RedirectToAction("details", new { id = newticket.TicketId });
                }
                return View();
            }
            catch (Exception ex)
            {
                // TODO: log the exeption in the log file
                return View(model);
            }
        }

        // GET: TicketController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Ticket ticket = ticketRepo.GetTicket(id);

            var user = await GetCurrentUser();

            if (ticket == null && ticket.RecordStatus == RecordStatus.deleted)
            {
                Response.StatusCode = 404;
                ViewBag.ErrorMessage = "Ticket with id = " + id + " is not found";
                return View("NotFound");
            }

            EditTicketViewModel model = new EditTicketViewModel
            {
                TicketId = ticket.TicketId,
                Location = ticket.Location,
                RequestDetail = ticket.RequestDetail,
                DepartmentId = ticket.DepartmentId,
                Department = ticket.Department,
                PriorityId = ticket.PriorityId,
                Subject = ticket.Subject,
                AssignedTo = ticket.AssignedTo,
                TicketStatus = ticket.TicketStatus,
                Notes = ticketRepo.GetAllNotes().Where(n => n.TicketId == id),
                TicketLogs = ticketRepo.GetAllTicketLogs().Where(n => n.TicketId == id),
                User = user,
                Attachments = ticketRepo.GetAttachments().Where(t => t.TicketId == ticket.TicketId)
            };

            ViewBag.priorities = ticketRepo.GetListofPriorites();
            ViewBag.departments = ticketRepo.GetListofDepartments(user.CompanyId);
            ViewBag.Users = userManager.Users.Where(u => u.DepartmentId == ticket.DepartmentId).ToList();

            return View(model);
        }

        // POST: TicketController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditTicketViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await GetCurrentUser();

                Ticket ticket = ticketRepo.GetTicket(model.TicketId);
                ticket.Location = model.Location;
                ticket.RequestDetail = model.RequestDetail;
                ticket.Subject = model.Subject;
                ticket.DepartmentId = model.DepartmentId;
                ticket.AssignedTo = model.AssignedTo;
                ticket.TicketStatus = model.TicketStatus;
                ticket.ModifiedBy = user.Id;
                ticket.ModifyDate = DateTime.Now;

                if (model.NoteBody != null)
                { 
                    Note note = new Note
                    {
                        NoteBody = model.NoteBody,
                        TicketId = ticket.TicketId
                    };
                    ticketRepo.CreateNote(note);
                }

                if (model.Files != null)
                {
                    foreach(var file in model.Files)
                    {
                        Attachment attachment = new Attachment
                        {
                            ServerFileName = FilesProcessor.UploadedFile(file, ticket.TicketId, hostingEnvironment),
                            FileName = file.FileName,
                            TicketId = ticket.TicketId,
                            UserId = user.Id,
                            FilePath = Path.Combine(hostingEnvironment.WebRootPath, "attachments", ticket.TicketId.ToString())
                        };
                        ticketRepo.AddAttachment(attachment);
                    }
                }

                ticketRepo.Update(ticket);

                LoggerTicketProcessor.LogEditedTicket(ticket.TicketId, user, ticketRepo);

                if(ticket.TicketStatus == TicketStatus.Solved)
                {
                    var createdByUser = await userManager.FindByIdAsync(ticket.CreatedBy);
                    EmailProcessor.SendEmail(user.UserName, user.Email, "Your ticket has been solved",
                        "please see your ticket and confirm the fix by folow this link " + "https://localhost:44333/ticket/details/" + ticket.TicketId);
                }

                return RedirectToAction("index");
            }
            return View();
        }

        // POST: TicketController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {

            //if(ticket == null)
            //{
            //    Response.StatusCode = 404;
            //    return View("Ticket not found", ticket.TicketId);
            //}
            //ticketRepo.Delete(ticket.TicketId);
            Ticket ticket = ticketRepo.GetTicket(id);
            ticket.RecordStatus = RecordStatus.deleted;
            ticket.ModifyDate = DateTime.Now;
            ticketRepo.Update(ticket);

            return Ok();
            //return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> ShowDeletedTickets()
        {
            var user = await GetCurrentUser();

            DeletedTicketsViewModel model = new DeletedTicketsViewModel
            {
                Departments = departmentRepo.ShowAllDepartments().Where(t => t.RecordStatus == RecordStatus.deleted &&
                t.CompanyId == user.CompanyId),
                Priorities = priorityRepo.ShowAllPriority().Where(t => t.RecordStatus == RecordStatus.deleted),
                Tickets = ticketRepo.ShowAllTickets().Where(t => t.RecordStatus == RecordStatus.deleted &&
                t.CompanyId == user.CompanyId)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForceDelete(int id)
        {
            Ticket ticket = ticketRepo.GetTicket(id);
            if (ticket == null)
            {
                ViewBag.ErrorMessage = "Ticket with id = " + id + " is not found";
                return View("NotFound");
            }
            ticketRepo.Delete(id);
            return Ok();
        }


        // this method to restore the deleted ticket 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(int id)
        {
            Ticket ticket = ticketRepo.GetTicket(id);
            if (ticket == null)
            {
                ViewBag.ErrorMessage = "Ticket with id = " + id + " is not found";
                return View("NotFound");
            }
            ticket.RecordStatus = RecordStatus.notdeleted;
            ticketRepo.Update(ticket);
            return Ok();
        }

        [HttpGet]
        public IActionResult DownloadFile(int id)
        {
            Attachment file = ticketRepo.GetAttachment(id);
            var path = Path.Combine(file.FilePath, file.ServerFileName);
            var fs = new FileStream(path, FileMode.Open);

            // Return the file. A byte array can also be used instead of a stream
            return File(fs, "application/octet-stream", file.FileName);
        }

        public IActionResult DashBoard(DateTime? startDate, DateTime? endDate)
        {
            if(startDate == null)
            {
                startDate = DateTime.MinValue;
            }
            if(endDate == null)
            {
                endDate = DateTime.Now;
            }
            var tickets = ticketRepo.GetAllTickets().Where(t => startDate <= t.CreateDate && t.CreateDate <= endDate);
            var model = new DashBoardViewModel
            {
                StartDate = (DateTime)startDate,
                EndDate = (DateTime)endDate,
                PreviousMonthTickets = ticketRepo.GetAllTickets().Where(t => DateTime.Now.AddMonths(-1) >= t.CreateDate),
                Tickets = tickets,
                Departments = departmentRepo.GetAllDepartments()
            };
            //ViewBag.PreviousMonthTickets = ticketRepo.GetAllTickets().Where(t => DateTime.Now.AddMonths(-1) >= t.CreateDate).Count();
            return View(model);
        }

        public ChartClass PopulationChart()
        {
            return ChartProcessor.PopulatChart(ticketRepo);
        }

        public Chart2Class FeedChart()
        {
            return ChartProcessor.PopulatChart(departmentRepo, ticketRepo);
        }

        public async Task<ActionResult> Index1()
        {
            var user = await GetCurrentUser();

            var allTickets = ticketRepo.GetAllTicketsInCompany(user.CompanyId);
            List<ShowTicketsViewModel> departmentTickets = new List<ShowTicketsViewModel>();
            List<ShowTicketsViewModel> myTickets = new List<ShowTicketsViewModel>();

            foreach (var ticket in allTickets.Where(t => t.DepartmentId == user.DepartmentId))
            {
                ShowTicketsViewModel item = new ShowTicketsViewModel
                {
                    TicketId = ticket.TicketId,
                    TicketStatus = ticket.TicketStatus,
                    AssignedTo = ticket.AssignedTo,
                    CreateDate = ticket.CreateDate,
                    CreatedBy = ticket.CreatedBy,
                    ModifiedBy = ticket.ModifiedBy,
                    ModifyDate = ticket.ModifyDate,
                    Notes = ticket.Notes,
                    Priority = ticket.Priority.PriorityName,
                    RequestDetail = ticket.RequestDetail,
                    Subject = ticket.Subject
                };
                departmentTickets.Add(item);
            }

            foreach (var ticket in allTickets.Where(t => t.AssignedTo == user.Id))
            {
                ShowTicketsViewModel item = new ShowTicketsViewModel
                {
                    TicketId = ticket.TicketId,
                    TicketStatus = ticket.TicketStatus,
                    AssignedTo = ticket.AssignedTo,
                    CreateDate = ticket.CreateDate,
                    CreatedBy = ticket.CreatedBy,
                    ModifiedBy = ticket.ModifiedBy,
                    ModifyDate = ticket.ModifyDate,
                    Notes = ticket.Notes,
                    Priority = ticket.Priority.PriorityName,
                    RequestDetail = ticket.RequestDetail,
                    Subject = ticket.Subject
                };
                myTickets.Add(item);
            }

            TicketsViewModel model = new TicketsViewModel
            {
                DepartmentTickets = departmentTickets,
                MyTickets = myTickets
            };


            return View(model);
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)); // will give the signedin user

            return user;
        }

    }
}
