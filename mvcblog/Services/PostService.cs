using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mvcblog.Data;
using mvcblog.Migrations;
using mvcblog.Models;
using mvcblog.Repository;

namespace mvcblog.Services
{
    public interface IPostService
    {
        IQueryable<Post> GetAllPostsWithUsers();
        IQueryable<Post> GetAllPostsOfUser(string id);
        Task<Post> GetPostWithUser(int postId);
        Task AddPost(Post post);
    }
    public class PostService : IPostService
    {
        private ApplicationDbContext _db;
        public IRepository<Post> _post { get; private set; }

        public PostService(ApplicationDbContext db)
        {
            _db = db;
            _post = new Repository<Post>(_db);
        }

        public IQueryable<Post> GetAllPostsWithUsers() 
        {
            IQueryable<Post> posts = _post.GetAll().OrderByDescending(u => u.PostDate).Include(u => u.User).AsNoTracking();
            return posts;
        }

        public IQueryable<Post> GetAllPostsOfUser(string id) 
        {
            IQueryable<Post> posts = _post.GetAll().OrderByDescending(u => u.PostDate)
                .Where(u => u.IdentityUserId == id).Include(u => u.User).AsNoTracking();
            return posts;
        }

        public async Task<Post> GetPostWithUser(int postId) 
        {
            Post post = await _post.GetAll().Where(u => u.Id == postId).Include(u => u.User).FirstOrDefaultAsync();
            return post;
        }

        public async Task AddPost(Post post) 
        {
            await _post.Add(post);
            await _post.SaveChanges();
        }
    }
}
