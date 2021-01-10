using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketMangment.Models;
using TicketMangment.ViewModel;

namespace TicketMangment.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepo departmentRepo;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITicketRepo ticketRepo;

        public DepartmentController(IDepartmentRepo departmentRepo,
                                    UserManager<ApplicationUser> userManager,
                                    ITicketRepo ticketRepo)
        {
            this.departmentRepo = departmentRepo;
            this.userManager = userManager;
            this.ticketRepo = ticketRepo;
        }
        // GET: DepartmentController
        public ActionResult Index()
        {
            var model = departmentRepo.GetAllDepartments();
            return View(model);
        }

        // GET: DepartmentController/Details/5
        public ActionResult Details(int id)
        {
            Department department = departmentRepo.GetDepartment(id);
            if (department == null)
            {
                Response.StatusCode = 404;
                ViewBag.ErrorMessage = "Department with id = " + id + " is not found";
                return View("NotFound");
            }

            Department model = new Department()
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName
            };
            return View(model);
        }

        // GET: DepartmentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DepartmentController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Department department)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        Department department1 = new Department
        //        {
        //            DepartmentName = department.DepartmentName
        //        };
        //        departmentRepo.Add(department1);
        //        return RedirectToAction("details", new { id = department1.DepartmentId });
        //    }
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string departmentName)
        {

            Department department1 = new Department
            {
                DepartmentName = departmentName,
                RecordStatus = RecordStatus.notdeleted,
                CreateDate = DateTime.Now,
                CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                departmentRepo.Add(department1);
            //return RedirectToAction("details", new { id = department1.DepartmentId });
            return Ok();
            
            
        }

        // GET: DepartmentController/Edit/5
        public ActionResult Edit(int id)
        {
            Department department = departmentRepo.GetDepartment(id);

            var tickets = ticketRepo.GetAllTickets().Where(t => t.DepartmentId == id);

            Department department1 = new Department
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                Tickets = tickets,
                DepAdmin = department.DepAdmin
            };

            foreach (var user in userManager.Users)
            {
                if (user.DepartmentId == id)
                {
                    department1.ListOfUsersName.Add(user.UserName);
                }
            }

            return View(department1);
        }

        //POST: DepartmentController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Department department)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Department department1 = departmentRepo.GetDepartment(department.DepartmentId);
        //        department1.DepartmentName = department.DepartmentName;

        //        departmentRepo.Update(department1);
        //        return RedirectToAction("index");
        //    }
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string newName)
        {
            Department department1 = departmentRepo.GetDepartment(id);
            if (department1 != null)
            {
                //Department department1 = departmentRepo.GetDepartment(id);
                //there is no need to put the next line becuse we don't need the user to chang id
                //department1.DepartmentId = department.DepartmentId;
                department1.DepartmentName = newName;
                department1.ModifiedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                department1.ModifyDate = DateTime.Now;

                departmentRepo.Update(department1);
                return Ok();
            }
            return BadRequest();
        }

        // GET: DepartmentController/Delete/5
        // we don't need this get method becuse it's will just delete and redirect us to index
        //public ActionResult Delete(int id)
        //{
        //    Department department = departmentRepo.GetDepartment(id);
        //    return View(department);
        //}

        // POST: DepartmentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Department department = departmentRepo.GetDepartment(id);
            if(department == null)
            {
                Response.StatusCode = 404;
                return BadRequest();
            }
            department.RecordStatus = RecordStatus.deleted;
            department.ModifyDate = DateTime.Now;
            departmentRepo.Update(department);
            //departmentRepo.Delete(department.DepartmentId);
            return Ok();
        }


        // this method for deleting the department permenently

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForceDelete(int id)
        {
            Department department = departmentRepo.GetDepartment(id);
            if(department == null)
            {
                ViewBag.ErrorMessage = "Department with id = " + id + " is not found";
                return View("NotFound");
            }
            departmentRepo.Delete(id);
            return Ok();
        }


        // this method to restore the deleted department 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(int id)
        {
            Department department = departmentRepo.GetDepartment(id);
            if(department == null)
            {
                ViewBag.ErrorMessage = "Department with id = " + id + " is not found";
                return View("NotFound");
            }
            department.RecordStatus = RecordStatus.notdeleted;
            departmentRepo.Update(department);
            return Ok();
        }



        [HttpGet]
        public ActionResult AddUserToDep(int id)
        {
            ViewBag.departmentId = id;

            var department = departmentRepo.GetDepartment(id);

            if (department == null)
            {
                ViewBag.ErrorMessage = $"Department with Id = {id} cannot be found";
                return View("NotFound");
            }

            ViewBag.Users = userManager.Users.Where(u => u.DepartmentId == department.DepartmentId).ToList();
            List<AddUserToDepViewModel> model = new List<AddUserToDepViewModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                    //InDepRole = user.InDepRole
                };
                if (user.DepartmentId == department.DepartmentId)
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                if(user.Id == department.DepAdmin)
                {
                    userRoleViewModel.DepAdmin = user.Id;
                }
                var addusertodepviewmodel = new AddUserToDepViewModel
                {
                    UserRoleViewModels = userRoleViewModel,
                    AssignTo = ""
                };
                model.Add(addusertodepviewmodel);
            }

            //var model = new AddUserToDepViewModel
            //{
            //    UserRoleViewModels = newlist
            //};

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToDep(List<AddUserToDepViewModel> model, int departmentId)
        {
            //var department = departmentRepo.GetDepartment(departmentId);

            //if (departmentRepo.GetDepartment(departmentId) == null)
            //{
            //    ViewBag.ErrorMessage = $"Department with id = {departmentId} cannot be found";
            //    return View("NotFound");
            //}

            IdentityResult result = null;

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserRoleViewModels.UserId);
                
                if (model[i].AssignTo != null)
                {
                    //var tickets = ticketRepo.GetAllTickets().Where(t => t.AssignedTo == user.Id);
                    foreach (var ticket in ticketRepo.GetAllTickets().Where(t => t.AssignedTo == user.Id))
                    {
                        ticket.AssignedTo = model[i].AssignTo;
                        ticketRepo.Update(ticket);

                        // add log to the ticket for changing the user assigned to
                        TicketLogs ticketLogs = new TicketLogs
                        {
                            TicketId = ticket.TicketId,
                            UserLog = User.FindFirstValue(ClaimTypes.NameIdentifier),
                            LogDate = DateTime.Now,
                            Message = $"The ticket has been assigned to {user.UserName}"
                        };
                        ticketRepo.CreateTicketLog(ticketLogs);
                    }
                }
                

                if (model[i].UserRoleViewModels.IsSelected && (user.DepartmentId == null))
                {
                    user.DepartmentId = departmentId;
                    departmentRepo.GetDepartment(departmentId).ListOfUsersName.Add(user.UserName);
                    result = await userManager.UpdateAsync(user);
                }
                else if (model[i].UserRoleViewModels.IsSelected && user.DepartmentId != null)
                {
                    // here we need the user to confirm cuz the he is already in anthor department
                    continue;
                }
                else if (!model[i].UserRoleViewModels.IsSelected && user.DepartmentId == departmentId)
                {
                    user.DepartmentId = null;
                    departmentRepo.GetDepartment(departmentId).ListOfUsersName.Remove(user.UserName);
                    result = await userManager.UpdateAsync(user);
                }
                else
                {
                    continue;
                }

                

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("Edit", new { Id = departmentId });
                    }
                }

                
            }

            return RedirectToAction("Edit", new { Id = departmentId });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> MakeUserAdmin(string userId, int depId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var department = departmentRepo.GetDepartment(depId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {userId} cannot be found";
                return View("NotFound");
            }

            if (department == null)
            {
                ViewBag.ErrorMessage = $"Department with id = {depId} cannot be found";
                return View("NotFound");
            }

            if(user.DepartmentId == department.DepartmentId)
            {
                department.DepAdmin = userId;
                departmentRepo.Update(department);
                return Ok();
            }
            else
            {
                return BadRequest("Can't make user as admin");
            }
            

            //foreach (var item in userManager.Users.Where(u => u.DepartmentId == depId))
            //{
            //    item.InDepRole = InDepRole.Employee;
            //    var result1 = await userManager.UpdateAsync(item);

            //    if (result1.Succeeded)
            //    {
            //        continue;
            //    }

            //    //foreach (var error in result1.Errors)
            //    //{
            //    //    ModelState.AddModelError("", error.Description);
            //    //}
            //    //if (!result1.Succeeded)
            //    //{
            //    //    // do something to inform the user this item(user) is unable to change it's states in this department
            //    //    return BadRequest();
            //    //}
            //}
            //user.InDepRole = InDepRole.Admin;
            //var result = await userManager.UpdateAsync(user);
            //if (!result.Succeeded)
            //{
            //    // do something to inform the user this item(user) is unable to change it's states in this department
            //    return BadRequest();
            //}
            //else
            //{
            //    return Ok();
            //}

        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<string> MakeUserAdminS(string userId, int depId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var department = departmentRepo.GetDepartment(depId);

            if (user.DepartmentId == department.DepartmentId)
            {
                department.DepAdmin = userId;
                departmentRepo.Update(department);
                //return Json( new{ code= "0" , messege ="OK"});  we have to change the return type to iactionresult
                return "Ok";

            }
            else
            {
                return "Can't make user as admin";
            }
        }

        [HttpPost]
        public async Task<IActionResult> DelegateUserTickets(string userId, string toUser)
        {
            var oldUser = await userManager.FindByIdAsync(userId);
            var newUser = await userManager.FindByIdAsync(toUser);
            var tickets = ticketRepo.GetAllTickets().Where(t => t.AssignedTo == oldUser.Id);

            foreach(var ticket in tickets)
            {
                ticket.AssignedTo = toUser;
                TicketLogs ticketLog = new TicketLogs
                {
                    TicketId = ticket.TicketId,
                    UserLog = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Message = $"The ticket has been delegated to {newUser.UserName}",
                    LogDate = DateTime.Now
                };
                ticketRepo.CreateTicketLog(ticketLog);
                
            }
            return Json(new { message = "OK" });
        }


        }
    }
