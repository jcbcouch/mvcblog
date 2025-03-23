using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvcblog.Models.ViewModels;
using mvcblog.Models;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;

namespace mvcblog.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AccountController(UserManager<ApplicationUser> userManager, 
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IWebHostEnvironment webHostEnvironment)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnurl = null)
        {
            if (!_roleManager.RoleExistsAsync(SD.Admin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.User));
            }
            ViewData["ReturnUrl"] = returnurl;
            RegisterViewModel registerViewModel = new();
            return View(registerViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Registeradmin()
        {
            RegisterViewModel registerViewModel = new();
            return View(registerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registeradmin(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {

                        await _userManager.AddToRoleAsync(user, SD.Admin);
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Post");


                AddErrors(result);
                
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                if (model.Image != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Image.CopyTo(fileStream);
                    }
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        ImagePath = fileName
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {

                        await _userManager.AddToRoleAsync(user, SD.User);
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnurl);


                    AddErrors(result);
                }
                else 
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {

                        await _userManager.AddToRoleAsync(user, SD.User);
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnurl);


                    AddErrors(result);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Login(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                ApplicationUser signedUser = await _userManager.FindByEmailAsync(model.Email);
                var result = await _signInManager.PasswordSignInAsync(signedUser.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                //    lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnurl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Post");
        }

        [HttpGet]
        public IActionResult NoAccess()
        {
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


    }
}


