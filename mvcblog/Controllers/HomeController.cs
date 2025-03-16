using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcblog.Data;
using mvcblog.Models;
using mvcblog.Models.ViewModels;

namespace mvcblog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            //List<Post> posts = _db.Posts.Include(u=>u.User).ToList();
            //return View(posts);
            int pageSize = 3;
            return View(await PaginatedList<Post>.CreateAsync(_db.Posts.Include(u => u.User)
                .AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Userposts(string userId, int? pageNumber)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            //List<Post> posts = _db.Posts.Where(l => l.IdentityUserId == user.Id).ToList();
            //return View(posts);
            int pageSize = 3;
            string userid = user.Id;
            ViewBag.userid = userid;
            return View(await PaginatedList<Post>.CreateAsync(_db.Posts.Where(l => l.IdentityUserId == user.Id)
                .Include(u => u.User).AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Post(int postId)
        {
            if (postId == 0)
            {
                return BadRequest();
            }
            Post post = await _db.Posts.Include(u => u.User).FirstOrDefaultAsync(u => u.Id == postId);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }


        [Authorize]
        public async Task<IActionResult> CreatePost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(Post submitData)
        {
            //ApplicationUser user = await _userManager.FindByIdAsync(submitData.IdentityUserId);
            var postObj = new Post
            {
                Title = submitData.Title,
                Body = submitData.Body,
                IdentityUserId = submitData.IdentityUserId
            };
            await _db.Posts.AddAsync(postObj);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
