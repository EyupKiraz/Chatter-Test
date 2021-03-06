using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestApplicaton.Data;
using TestApplicaton.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Dynamic;

namespace TestApplicaton.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user != null)
            {
                var group = _dbContext.Groups.FirstOrDefault(x => x.Id == user.GroupId);
                if(group != null)
                {
                    var rawPosts = _dbContext.Posts.Where(post => post.GroupId == group.Id);
                    var posts = new List<Object>();

                    foreach (var post in rawPosts)
                    {
                        var postUser = _dbContext.Users.First(x => x.Id == post.ApplicationUserId);
                        dynamic newPost = new ExpandoObject();
                        newPost.Id = post.Id;
                        newPost.PostUser = postUser;
                        newPost.Content = post.Content;
                        newPost.UpdatedAt = post.CreatedAt.ToString("HH:mm");
                        newPost.Likes = post.Likes;
                        newPost.Dislikes = post.Dislikes;
                        posts.Add(newPost);
                    }

                    ViewBag.Group = group;
                    ViewBag.Posts = posts;
                }
                ViewBag.User = user;
            }


            return View();
        }

        public FileContentResult Photo(string userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
            return new FileContentResult(user.ProfilePicture, "image/jpeg");
        }

        [HttpPost]
        public async Task<IActionResult> SharePost(string content)
        {
            var post = new Post();
            var user = await _userManager.GetUserAsync(User);
            var group = _dbContext.Groups.First(x => x.Id == user.GroupId);
            post.GroupId = group.Id;
            post.ApplicationUserId = user.Id;
            post.Content = content;
            post.CreatedAt = DateTime.Now;
            post.UpdatedAt = DateTime.Now;

            _dbContext.Posts.Add(post);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(string content, int id)
        {
            var post = _dbContext.Posts.FirstOrDefault(x => x.Id == id);
            var user = await _userManager.GetUserAsync(User);

            if (post != null && post.ApplicationUserId == user.Id)
            {
                post.Content = content;
                post.UpdatedAt = DateTime.Now;
                _dbContext.SaveChanges();
            }


            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = _dbContext.Posts.FirstOrDefault(x => x.Id == id);

            _dbContext.Posts.Remove(post);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> LikePost(int id)
        {
            var post = _dbContext.Posts.FirstOrDefault(x => x.Id == id);
            var user = await _userManager.GetUserAsync(User);
            var idString = "(" + id + "),";

            if(user.Dislikes.Contains(idString))
            {
                user.Dislikes = user.Dislikes.Remove(user.Dislikes.IndexOf(idString), idString.Length);
                post.Dislikes -= 1;
            }

            if (user.Likes.Contains(idString))
            {
                user.Likes = user.Likes.Remove(user.Likes.IndexOf(idString), idString.Length);
                post.Likes -= 1;

                _dbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            if(user.Likes != null)
            {
                user.Likes += idString;
            } else
            {
                user.Likes = idString;
            }

            post.Likes += 1;

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> DislikePost(int id)
        {
            var post = _dbContext.Posts.FirstOrDefault(x => x.Id == id);
            var user = await _userManager.GetUserAsync(User);

            var idString = "(" + id + "),";

            if (user.Likes.Contains(idString))
            {
                user.Likes = user.Likes.Remove(user.Likes.IndexOf(idString), idString.Length);
                post.Likes -= 1;
            }

            if (user.Dislikes.Contains(idString))
            {
                user.Dislikes = user.Dislikes.Remove(user.Dislikes.IndexOf(idString), idString.Length);
                post.Dislikes -= 1;

                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            if (user.Dislikes != null)
            {
                user.Dislikes += idString;
            }
            else
            {
                user.Dislikes = idString;
            }

            post.Dislikes += 1;

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
