using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using TicketMangment.Models;
using TicketMangment.ViewModel;

namespace TicketMangment.Controllers
{
    // we can change the authorize to make it by policy insted of role
    // [Authorize(Policy = "AdminRolePolicy")]
    [Authorize(Roles = "Admin,Owner")]
    [Authorize(Policy = "AccessControlPanelPolicy")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDepartmentRepo departmentRepo;
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        IDepartmentRepo departmentRepo,
                                        ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.departmentRepo = departmentRepo;
            this.logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ManageClaimsRolePolicy")]
        public async Task<IActionResult> ManageRoleClaims (string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with id {roleId} cannot be found";
                return View("NotFound");
            }

            var existingRoleClaims = await roleManager.GetClaimsAsync(role);

            var model = new RoleClaimsViewModel
            {
                RoleId = roleId
            };

            foreach (var claim in ClaimsStore.AllClaims)
            {
                RoleClaim roleClaim = new RoleClaim
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                };

                if (existingRoleClaims.Any(c => c.Type == claim.Type))
                {
                    roleClaim.IsSelected = true;
                }

                model.Claims.Add(roleClaim);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "ManageClaimsRolePolicy")]
        public async Task<IActionResult> ManageRoleClaims(RoleClaimsViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"User with id {model.RoleId} cannot be found";
                return View("NotFound");
            }

            var claims = await roleManager.GetClaimsAsync(role);
            IdentityResult result = null;
            foreach (var claim in claims)
            {
                result = await roleManager.RemoveClaimAsync(role, claim);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", $"Cannot remove {claim.Type} from this role");
                    return View(model);
                }
            }

            foreach (var claim in model.Claims)
            {
                if (claim.IsSelected)
                {
                    result = await roleManager.AddClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", $"Cannot add {claim.ClaimType} claim to this role");
                        return View(model);
                    }
                }
            }
            
            return RedirectToAction("EditRole", new { Id = model.RoleId });
        }

        [HttpGet]
        [Authorize(Policy = "ManageClaimsUserPolicy")]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id {userId} cannot be found";
                return View("NotFound");
            }

            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                };

                // if the user has the claim set IsSelected property to true,
                // the checkbox next to the claim is checked on the UI
                // the c.value can be fix by adding this (c.Value == "true" || c.Value == c.Type) after the &&
                if (existingUserClaims.Any(c => c.Type == claim.Type /*&& c.Value == "true"*/))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "ManageClaimsUserPolicy")]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id {model.UserId} cannot be found";
                return View("NotFound");
            }

            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            //foreach(var claim in model.Claims)
            //{
            //    if (claim.IsSelected)
            //    {
                    //result = await userManager.AddClaimsAsync(user, model.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)));
                    result = await userManager.AddClaimsAsync(user, model.Claims.Where(c => c.IsSelected == true).Select(c => new Claim(c.ClaimType, c.ClaimValue)));
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", $"{result.Errors}");
                        return View(model);
                    }
            //    }
            //}

            //result = await userManager.AddClaimsAsync(user, model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });
        }


        [HttpGet]
        [Authorize(Policy = "CreateRolePolicy")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "CreateRolePolicy")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if(ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await roleManager.CreateAsync(identityRole);

                if(result.Succeeded)
                {
                    return RedirectToAction("listroles", "administration");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "UsersListPolicy")]
        public async Task<IActionResult> ListUsers()
        {
            var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var users = userManager.Users.Include(u => u.Department).Where(u => u.CompanyId == user.CompanyId);

            ViewBag.departments = departmentRepo.GetAllDepartmentsInCompany(user.CompanyId).ToList();
            return View(users);
        }

        [HttpGet]
        [Authorize(Policy = "EditUserPolicy")]
        public async Task<IActionResult> EditUser (string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} is not found";
                return View("NotFound");
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);

            IEnumerable<Department> departments = departmentRepo.GetAllDepartments();
            ViewBag.ListOfDepartment = departments;

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.DisplayName,
                Email = user.Email,
                DepartmentId = user.DepartmentId,
                Roles = userRoles,
                //Claims = userClaims.Select(c => c.Type + " " + c.Value).ToList()
                Claims = userClaims.Select(c => c.Type).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditUserPolicy")]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {model.Id} is not found";
                return View("NotFound");
            }
            else
            {
                user.DisplayName = model.Name;
                user.Email = model.Email;
                user.DepartmentId = model.DepartmentId;

                var result = await userManager.UpdateAsync(user);

                if(result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

        }

        [HttpGet]
        [Authorize(Policy = "MangeUserRolesPolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await userManager.FindByIdAsync(userId);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach(var role in roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "MangeUserRolesPolicy")]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model , string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot remove user from existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to the user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }


        [HttpPost]
        [Authorize(Policy = "DeleteUserPolicy")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} is not found";
                return View("NotFound");
            }

            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListUsers");
                }
            }
        }

        // we get the policy name from startup file in the ConfigureServices method

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id = {id} is not found";
                return View("NotFound");
            }

            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View("ListRoles");
                    }
                }
                catch(DbUpdateException ex)
                {
                    logger.LogError($"Exception occured: {ex}");
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted";
                    return View("Error");
                }
            }
        }

        [HttpGet]
        [Authorize(Policy = "ListRolesPolicy")]
        public async Task<IActionResult> ListRoles()
        {
            var roles = roleManager.Roles;
            List<EditRoleViewModel> listRoles = new List<EditRoleViewModel>();
            foreach (var role in roles)
            {
                var roleClaims = await roleManager.GetClaimsAsync(role);

                var item = new EditRoleViewModel
                {
                    Id = role.Id,
                    RoleName = role.Name,
                    Claims = roleClaims
                };

                listRoles.Add(item);
            }
            return View(listRoles);
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            var loggedUser = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id{id} cannot be found";
                return View("NotFound");
            }

            var roleClaims = await roleManager.GetClaimsAsync(role);

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
                //Claims = roleClaims.Select(c => c.Type + " " + c.Value).ToList()
                //Claims = roleClaims.Select(c => c.Type).ToList(),
                Claims = roleClaims
            };

            foreach(var user in userManager.Users.Where(u => u.CompanyId == loggedUser.CompanyId))
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.DisplayName);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with id{model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if(result.Succeeded)
                {
                    return RedirectToAction("listroles");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

        }

        [HttpGet]
        [Authorize(Policy = "EditUsersInRolePolicy")]
        public async Task<IActionResult> EditUsersInRole (string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            var loggedUser = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            foreach (var user in userManager.Users.Where(u => u.CompanyId == loggedUser.CompanyId))
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.DisplayName
                };
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditUsersInRolePolicy")]
        public async Task<IActionResult> EditUsersInRole (List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id = {roleId} cannot be found";
                return View("NotFound");
            }

            IdentityResult result = null;

            for (int i=0 ; i<model.Count ; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user,role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if(!model[i].IsSelected && await userManager.IsInRoleAsync(user,role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded) 
                { 
                    if(i<(model.Count -1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

    }
}
