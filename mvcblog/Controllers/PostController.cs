using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvcblog.Models;
using mvcblog.Services;

namespace mvcblog.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPostService _postService;

        public PostController(ILogger<PostController> logger, 
                              UserManager<ApplicationUser> userManager,
                              IPostService postService)
        {
            _logger = logger;
            _userManager = userManager;
            _postService = postService;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 3;
            List<Post> posts = await PaginatedList<Post>.CreateAsync(_postService.GetAllPostsWithUsers(), pageNumber ?? 1, pageSize);
            return View(posts);
        }

        public async Task<IActionResult> Userposts(string userId, int? pageNumber)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            int pageSize = 3;
            ViewBag.userid = user.Id;
            List<Post> posts = await PaginatedList<Post>.CreateAsync(_postService.GetAllPostsOfUser(user.Id), pageNumber ?? 1, pageSize);
            return View(posts);
        }

        public async Task<IActionResult> Post(int postId)
        {
            if (postId == 0)
            {
                return BadRequest();
            }
            Post post = await _postService.GetPostWithUser(postId);
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
        [Authorize]
        public async Task<IActionResult> CreatePost(Post submitData)
        {
            var postObj = new Post
            {
                Title = submitData.Title,
                Body = submitData.Body,
                PostDate = System.DateTime.Now,
                IdentityUserId = submitData.IdentityUserId
            };
            await _postService.AddPost(postObj);

            return RedirectToAction("Index", "Post");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
