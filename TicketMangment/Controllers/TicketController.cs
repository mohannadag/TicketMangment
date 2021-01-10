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

        //private readonly RoleManager<ApplicationUser> roleManager;

        public TicketController(ITicketRepo ticketRepo, UserManager<ApplicationUser> userManager,
            IDepartmentRepo departmentRepo, IPriorityRepo priorityRepo, IWebHostEnvironment hostingEnvironment)
            //RoleManager<ApplicationUser> roleManager)
        {
            this.ticketRepo = ticketRepo;
            this.userManager = userManager;
            this.departmentRepo = departmentRepo;
            this.priorityRepo = priorityRepo;
            this.hostingEnvironment = hostingEnvironment;
            //this.roleManager = roleManager;
        }
        // GET: TicketController
        public ActionResult Index([Optional] string getAllTickets)
        {
            if (getAllTickets != null)
            {
                if (getAllTickets.ToLower() == "all")
                { 
                    var model = ticketRepo.GetAllTickets();
                    return View(model);
                }
                else
                {
                    var model = ticketRepo.GetAllTickets().Where(t => t.RecordStatus == RecordStatus.notdeleted);
                    return View(model);
                }
            }
            else
            {
                var model = ticketRepo.GetAllTickets().Where(t => t.RecordStatus == RecordStatus.notdeleted);
                return View(model);
            }

    }

        // GET: TicketController/Details/5
        public ActionResult Details(int id)
        {
            Ticket ticket = ticketRepo.GetTicket(id);
            //ticket.Department = ticketRepo.GetDepartment(ticket.DepartmentId);

            if(ticket == null)
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }
            //ViewBag.path = HttpContext.Request.Path;
            ticket.Notes = ticketRepo.GetAllNotes().Where(t => t.TicketId == id).ToList();

            // this viewbag to get the returned url
            ViewBag.returnUrl = Request.Headers["Referer"].ToString();

            
            return View(ticket);
        }

        // GET: TicketController/Create
        public /*async Task<IActionResult>*/ IActionResult Create()
        {
            ViewBag.departments = ticketRepo.GetListofDepartments();
            ViewBag.priorites = ticketRepo.GetListofPriorites();
            //var users = userManager.Users;
            //var usersList = new List<ApplicationUser> { };
            //foreach (var user in users)
            //{
            //    if(await userManager.IsInRoleAsync(user,"Admin"))
            //    {
            //        usersList.Add(user);
            //    }
            //}
            //var selectListUsers = new SelectList(usersList, "Id", "UserName");
            //ViewBag.Users = selectListUsers;
            return View();
        }

        // POST: TicketController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TicketCreatViewModel model)
        {
            
            try
            {
                if (ModelState.IsValid)
                {

                    Ticket newticket = new Ticket
                    {
                        Subject = model.Subject,
                        RequestDetail = model.RequestDetail,
                        Location = model.Location,
                        CreateDate = DateTime.Now,
                        CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier), // will give the user's userId
                        DepartmentId = model.DepartmentId,
                        PriorityId = model.PriorityId,
                        AssignedTo = model.AssignedTo,
                        TicketStatus = TicketStatus.Open,
                        RecordStatus = RecordStatus.notdeleted
                    };
                    Ticket ticket = ticketRepo.Add(newticket);

                    TicketLogs ticketLogs = new TicketLogs
                    {
                        TicketId = ticket.TicketId,
                        UserLog = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        Message = "The ticket has been created",
                        LogDate = DateTime.Now
                    };
                    ticketRepo.CreateTicketLog(ticketLogs);

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
            catch
            {
                return View();
            }
        }

        // GET: TicketController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Ticket ticket = ticketRepo.GetTicket(id);
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
                User = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                Attachments = ticketRepo.GetAttachments().Where(t => t.TicketId == ticket.TicketId)
            };

            ViewBag.priorities = ticketRepo.GetListofPriorites();
            ViewBag.departments = ticketRepo.GetListofDepartments();
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
                ApplicationUser user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));


                Ticket ticket = ticketRepo.GetTicket(model.TicketId);
                //ticket.UserId = model.UserId;
                ticket.Location = model.Location;
                //ticket.Photo = model.Photo;
                //ticket.Priority = model.Priority;
                ticket.RequestDetail = model.RequestDetail;
                //ticket.Department = model.Department;
                ticket.Subject = model.Subject;
                ticket.DepartmentId = model.DepartmentId;
                ticket.AssignedTo = model.AssignedTo;
                ticket.TicketStatus = model.TicketStatus;
                ticket.ModifiedBy = user.Id;
                ticket.ModifyDate = DateTime.Now;

                //if (ticket.AssignedTo != null)
                //{
                //    ticket.TicketStatus = TicketStatus.Asssigned;
                //}
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
                            ServerFileName = ProccessUploadedFile(file, ticket.TicketId),
                            FileName = file.FileName,
                            TicketId = ticket.TicketId,
                            UserId = user.Id,
                            FilePath = Path.Combine(hostingEnvironment.WebRootPath, "attachments", ticket.TicketId.ToString())
                        };
                        ticketRepo.AddAttachment(attachment);
                    }
                }

                ticketRepo.Update(ticket);

                TicketLogs ticketLogs = new TicketLogs
                {
                    TicketId = model.TicketId,
                    UserLog = user.Id,
                    LogDate = DateTime.Now,
                    Message = "The ticket has been modified"
                };
                ticketRepo.CreateTicketLog(ticketLogs);

                if(ticket.TicketStatus == TicketStatus.Solved)
                {
                    SendEmail(user.UserName, user.Email, "Your ticket has been solved",
                        "please see your ticket and confirm the fix by folow this link " + "https://localhost:44333/ticket/details/6");
                }

                // if the ticketstatus is solved we need to do 
                // something like send email or notefication to 
                // the user whom created the ticket to approve 
                // that the problem has solved and close the ticket

                
                return RedirectToAction("index");
            }
            return View();
        }

        // GET: TicketController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    //ticketRepo.Delete(id);
        //    //return RedirectToAction("index");

        //    Ticket ticket = ticketRepo.GetTicket(id);
        //    return View(ticket);
        //}

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

        //public IActionResult MyTickets()
        //{
        //    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
        //    var tickets = ticketRepo.GetTicketsByUser(userId);
        //    //var createdTickets = ticketRepo.GetTicketsByUserId(userId);
        //    if (tickets == null)
        //    {
        //        //Response.StatusCode = 404;
        //        return View("There is non for you", userId);
        //    }
        //    return View(tickets);
        //}

        public async Task<IActionResult> MyTickets()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var user = await userManager.FindByIdAsync(userId);
            var tickets = ticketRepo.GetTicketsByUser(userId).Where(t => t.DepartmentId == user.DepartmentId)
                .Where(t => t.RecordStatus == RecordStatus.notdeleted); // Get all tickets the user assigned to
            var createdTickets = ticketRepo.GetTicketsByUserId(userId); // Get all tickets created by the user
            if (tickets == null)
            {
                //Response.StatusCode = 404;
                return View("There is non for you", userId);
            }

            var model = new MyTicketsViewModel
            {
                CreatedTickets = createdTickets,
                MyTickets = tickets
            };
            return View(model);
        }

        // I can use this method when i need to add notes in the edit method
        [HttpPost]
        public ActionResult CreatNote(string model)
        {
            Note note = new Note
            {
                NoteBody = model,
                CreatedDate = DateTime.Now,
                CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            ticketRepo.CreateNote(note);
            return View();
        }

        [HttpPost]
        public ActionResult CreateNote(string model, int ticketId)
        {
            if(model != null)
            { 
                Note note = new Note
                {
                    NoteBody = model,
                    TicketId = ticketId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                ticketRepo.CreateNote(note);

                TicketLogs ticketLogs = new TicketLogs
                {
                    TicketId = ticketId,
                    UserLog = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    LogDate = DateTime.Now,
                    Message = "New note created"
                };
                ticketRepo.CreateTicketLog(ticketLogs);
                return Ok();
            }
            else
            {
                return BadRequest("Invalid data.");
            }
            //return View();
        }

        [HttpGet]
        public IActionResult ShowDeletedTickets()
        {
            DeletedTicketsViewModel model = new DeletedTicketsViewModel
            {
                Departments = departmentRepo.ShowAllDepartments().Where(t => t.RecordStatus == RecordStatus.deleted),
                Priorities = priorityRepo.ShowAllPriority().Where(t => t.RecordStatus == RecordStatus.deleted),
                Tickets = ticketRepo.ShowAllTickets().Where(t => t.RecordStatus == RecordStatus.deleted)
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

        private string ProccessUploadedFile(IFormFile model , int id)
        {
            string uniqueFileName = null;
            if (model != null)
            {
                string path = Path.Combine(hostingEnvironment.WebRootPath, "attachments", id.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "attachments");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FileName;
                string filePath = Path.Combine(path, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
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

        private void SendEmail(string name, string email, string subject, string body)
        {
            // Prepare an email message to be sent

            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress("mohannad", "ag.mohannad1@gmail.com");
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(name, email);
            message.To.Add(to);

            message.Subject = subject;

            // Add email body and file attachments

            BodyBuilder bodyBuilder = new BodyBuilder();
            //bodyBuilder.HtmlBody = "<h1>Hello from the ticket mangment system</h1>";
            bodyBuilder.TextBody = body;

            // if we need to  add attachments with the email we uncomment the next line and specify the file we want
            //bodyBuilder.Attachments.Add(hostingEnvironment.WebRootPath + "\\file.png");

            message.Body = bodyBuilder.ToMessageBody();

            // Connect and authenticate with the SMTP server

            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate("ag.mohannad1@gmail.com", "1993Swat");

            // Send email message

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
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
            List<string> liststring = new List<string>();
            List<int> solvedTickets = new List<int>();
            List<int> allTickets = new List<int>();
            var tickets = ticketRepo.GetAllTickets();
            DateTime date = DateTime.Now.AddYears(-1);
            for(int i=1; i <=12; i++)
            {
                liststring.Add((date.AddMonths(i)).ToString("MMM"));
                allTickets.Add((tickets.Where(t => t.CreateDate >= date.AddMonths(i-1) && t.CreateDate <= date.AddMonths(i))).Count());
                solvedTickets.Add((tickets.Where(t => t.TicketStatus == TicketStatus.Solved &&
                        t.CreateDate >= date.AddMonths(i-1) && t.CreateDate <= date.AddMonths(i))).Count());
            }

            ChartClass chart = new ChartClass
            {
                Labels = liststring.ToArray(),
                AllTicketsValues = allTickets.ToArray(),
                SolvedTicketsValues = solvedTickets.ToArray()
            };
            return chart;
            
        }

        public Chart2Class FeedChart()
        {
            List<string> departmentsNames = new List<string>();
            List<int> ticketsNumber = new List<int>();

            foreach(var dep in departmentRepo.GetAllDepartments())
            {
                departmentsNames.Add(dep.DepartmentName);
                ticketsNumber.Add(ticketRepo.GetAllTickets().Where(t => t.DepartmentId == dep.DepartmentId).Count());
            }
            Chart2Class chart = new Chart2Class
            {
                Labels = departmentsNames.Take(4).ToArray(),
                TicketsCount = ticketsNumber.Take(4).ToArray()
            };

            return chart;
        }

        //public ActionResult CreateTicketLog(int ticketId, string userId, string message)
        //{
        //    if(ticketId != null && userId != null && message != null)
        //    {
        //        var log = new TicketLogs
        //        {
        //            TicketId = ticketId,
        //            UserLog = userId,
        //            Message = message
        //        };
        //        ticketRepo.CreateTicketLog(log);
        //        return Ok();
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }


        //}

    }
}
