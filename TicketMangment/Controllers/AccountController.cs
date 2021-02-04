using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketMangment.Models;
using TicketMangment.ViewModel;

namespace TicketMangment.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IDepartmentRepo departmentRepo;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ICompanyRepo companyRepo;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
                                IDepartmentRepo departmentRepo, IWebHostEnvironment hostingEnvironment, ICompanyRepo companyRepo,
                                ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.departmentRepo = departmentRepo;
            this.hostingEnvironment = hostingEnvironment;
            this.companyRepo = companyRepo;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var company = new Company
                {
                    Name = model.CompanyName,
                    Email = model.CompanyEmail,
                    Address = model.Address,
                    PhoneNumber = model.CompanyPhoneNumber,
                    CreateDate = DateTime.Now
                };

                var newcompany = companyRepo.Add(company);

                var user = new ApplicationUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    CompanyId = newcompany.Id
                };

                

                var result = await userManager.CreateAsync(user, model.Password);
                if(result.Succeeded)
                {
                    //if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    if (signInManager.IsSignedIn(User))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("index", "home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if(ModelState.IsValid)
            {
                try { 
                    var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password,
                        model.RememberMe, false);

                    if(result.Succeeded)
                    {
                        if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("index", "home");
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
                catch(Exception ex)
                {
                    logger.LogError($"Exception occured: {ex}");
                    ViewBag.ErrorTitle = $"cannot sign in with this username {model.UserName}";
                    ViewBag.ErrorMessage = $"please wait and try again later if this problem keep happening " +
                        $"to you please contact the developer at ag.mohannad@gmail.com and thank you for you patient";
                    return View("Error");
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        [AcceptVerbs("Get","Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"User Name {email} is already in use");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (user == null)
            {
                ViewBag.ErrorMessage = "User cannot be found please contact with the adminstrator";
                return View("NotFound");
            }
            UserViewModel model = new UserViewModel
            {
                ID = user.Id,
                Name = user.UserName,
                Email = user.Email,
                ExistingPhotoPath = user.PhotoPath
            };
            if (user.DepartmentId != null)
            {
                model.Department = departmentRepo.GetDepartment((int)user.DepartmentId).DepartmentName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.ID);

                if (user == null)
                {
                    ViewBag.ErrorMessage = "User cannot be found please contact with the adminstrator";
                    return View("NotFound");
                }

                user.UserName = model.Name;
                //user.Email = model.Email;
                //if (user.PasswordHash == model.OldPassword)
                //{
                //    user.PasswordHash = model.NewPassword;
                //}

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    user.PhotoPath = ProccessUploadedFile(model);
                }

                var result = await userManager.UpdateAsync(user);

                if(result.Succeeded)
                {
                    return RedirectToAction("dashboard", "administration");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            return View(model);
        }

        private string ProccessUploadedFile(UserViewModel model)
        {
            string uniqueFileName = null;
            if(model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
