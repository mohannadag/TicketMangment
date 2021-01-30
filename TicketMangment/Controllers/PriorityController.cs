using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketMangment.Models;

namespace TicketMangment.Controllers
{
    [Authorize]
    public class PriorityController : Controller
    {
        private readonly IPriorityRepo priorityRepo;

        public PriorityController(IPriorityRepo priorityRepo)
        {
            this.priorityRepo = priorityRepo;
        }
        // GET: PriorityController
        public ActionResult Index()
        {
            var model = priorityRepo.GetAllPriority();
            return View(model);
        }

        // GET: PriorityController/Details/5
        public ActionResult Details(int id)
        {
            Priority priority = priorityRepo.GetPriority(id);
            if(priority == null && priority.RecordStatus == RecordStatus.deleted)
            {
                Response.StatusCode = 404;
                ViewBag.ErrorMessage = "Priority with id = " + id + " is not found";
                return View("NotFound");
            }

            return View(priority);
        }

        // GET: PriorityController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PriorityController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Priority priority)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        Priority newpriority = new Priority
        //        {
        //            PriorityName = priority.PriorityName,
        //            CreateDate = DateTime.Now
        //        };
        //        priorityRepo.Add(newpriority);
        //        return RedirectToAction("details", new { id = newpriority.PriorityId });
        //    }
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string priorityName)
        {

            Priority model = new Priority
            {
                PriorityName = priorityName,
                RecordStatus = RecordStatus.notdeleted,
                CreateDate = DateTime.Now,
                CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            priorityRepo.Add(model);
            //return RedirectToAction("details", new { id = department1.DepartmentId });
            return Ok();


        }

        // GET: PriorityController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    Priority priority = priorityRepo.GetPriority(id);
        //    return View(priority);
        //}

        //// POST: PriorityController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Priority model)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        Priority priority = priorityRepo.GetPriority(model.PriorityId);
        //        priority.PriorityName = model.PriorityName;
        //        priority.ModifyDate = DateTime.Now;
        //        priorityRepo.Update(priority);
        //        return RedirectToAction("index");
        //    }
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string newName)
        {
            Priority priority = priorityRepo.GetPriority(id);

            if (priority == null && priority.RecordStatus == RecordStatus.deleted)
            {
                Response.StatusCode = 404;
                ViewBag.ErrorMessage = "Priority with id = " + id + " is not found";
                return View("NotFound");
            }

            if (priority != null)
            {
                //Department department1 = departmentRepo.GetDepartment(id);
                //there is no need to put the next line becuse we don't need the user to chang id
                //department1.DepartmentId = department.DepartmentId;
                priority.PriorityName = newName;
                priority.ModifiedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                priority.ModifyDate = DateTime.Now;

                priorityRepo.Update(priority);
                return Ok();
            }
            return BadRequest();
        }

        // GET: PriorityController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: PriorityController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id)
        //{
        //    Priority priority = priorityRepo.Delete(id);
        //    return View("index");
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Priority priority = priorityRepo.GetPriority(id);
            if (priority == null)
            {
                Response.StatusCode = 404;
                return BadRequest();
            }
            priority.RecordStatus = RecordStatus.deleted;
            priority.ModifyDate = DateTime.Now;
            priorityRepo.Update(priority);
            //departmentRepo.Delete(department.DepartmentId);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForceDelete(int id)
        {
            Priority priority = priorityRepo.GetPriority(id);
            if (priority == null)
            {
                ViewBag.ErrorMessage = "Department with id = " + id + " is not found";
                return View("NotFound");
            }
            priorityRepo.Delete(id);
            return Ok();
        }


        // this method to restore the deleted priority 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(int id)
        {
            Priority priority = priorityRepo.GetPriority(id);
            if (priority == null)
            {
                ViewBag.ErrorMessage = "Priority with id = " + id + " is not found";
                return View("NotFound");
            }
            priority.RecordStatus = RecordStatus.notdeleted;
            priorityRepo.Update(priority);
            return Ok();
        }

    }
}
