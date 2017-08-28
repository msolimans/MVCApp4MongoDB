using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using M101DotNet.WebApp.Models;
using M101DotNet.WebApp.Models.Home;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace M101DotNet.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var blogContext = new BlogContext();
            // XXX WORK HERE
            // find the most recent 10 posts and order them
            // from newest to oldest

            int count = 10;
            List<Post> recentPosts = new List<Post>();
            var filter = Builders<Post>.Filter.Where(_ => true);

            using (var cursor = await blogContext.Posts.Find(filter).Sort("{_id: -1}").Limit(10).ToCursorAsync())
            {
                while (count-- > 0 && await cursor.MoveNextAsync())
                {
                    var c = cursor.Current;
                    
                    recentPosts.AddRange(c);
                }
            }

            var model = new IndexModel
            {
                RecentPosts = recentPosts
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult NewPost()
        {
            return View(new NewPostModel());
        }

        [HttpPost]
        public async Task<ActionResult> NewPost(NewPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var blogContext = new BlogContext();
            // XXX WORK HERE
            // Insert the post into the posts collection
        
            var post = new Post();
            post.Title = model.Title;
            post.Content = model.Content;
            post.Tags = model.Tags;
            post.Author = this.User.Identity.Name;
            await blogContext.Posts.InsertOneAsync(post);
           
            
            return RedirectToAction("Post", new { id = post.Id.ToString() });
        }

        [HttpGet]
        public async Task<ActionResult> Post(string id)
        {
            var blogContext = new BlogContext();

            // XXX WORK HERE
            // Find the post with the given identifier
          

            var filter = Builders<Post>.Filter.Eq("_id", new ObjectId(id));

            var col = await blogContext.Posts.FindAsync<Post>(filter);
            var post = col.FirstOrDefault();
            
                
            
            
            if (post == null)
            {
                return RedirectToAction("Index");
            }

            var model = new PostModel
            {
                Post = post
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Posts(string tag = null)
        {
            var blogContext = new BlogContext();

            
            // XXX WORK HERE
            // Find all the posts with the given tag if it exists.
            // Otherwise, return all the posts.
            // Each of these results should be in descending order.

            List<Post> posts = new List<Post>();
            var filter = new BsonDocument { { "tags", new BsonDocument { { "$regex", (tag == null ? ".*": tag) }, { "$options", "i" } } } };


            var cursor = await blogContext.Posts.Find(filter).ToCursorAsync();

            if(cursor != null)
            {
                while (await cursor.MoveNextAsync())
                {
                    var c = cursor.Current;
                    posts.AddRange(c);
                }
            }
            
            return View(posts);
            
        }

        [HttpPost]
        public async Task<ActionResult> NewComment(NewCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = model.PostId });
            }

            var blogContext = new BlogContext();
            // XXX WORK HERE
            // add a comment to the post identified by model.PostId.
            // you can get the author from "this.User.Identity.Name"

            var update = Builders<Post>.Update.Push(p => p.Comments, new Comment() {Content = model.Content, Author = this.User.Identity.Name });
            var filter = Builders<Post>.Filter.Where(x => x.Id == new ObjectId(model.PostId));
            
            await blogContext.Posts.FindOneAndUpdateAsync(filter, update);
            
            return RedirectToAction("Post", new { id = model.PostId });
        }
    }
}