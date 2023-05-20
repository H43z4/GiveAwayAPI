using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.DatabaseModels.DSSDatabaseObjects.Setup;
using Models.ViewModels.DSAuth.Setup;
using Models.ViewModels.PostManagemrnts;
using RepositoryLayer;
using SharedLib.Interfaces;

namespace PostManagement
{
    public interface IPostManagementService : ICurrentDSUser
    {
        int craetePost(CreatePostVM createPost); 
        int UpdatePost(UpdatePostVM createPost);
        int ChnagePostStatus (ChangePostStatusVM changePostStatus); 
        List<ResponsePostVM> GetAllPosts();
        ResponsePostVM GetAllPostsById(int Id);       
    }
    public class PostManagementService : IPostManagementService
    {
        private readonly AppDbContext _context;
        public VwDSUser VwUser { get; set; }
        public VwDSUser VwDSUser { get; set; }
        public PostManagementService(AppDbContext context)
        {
            _context = context;
        }

        public int ChnagePostStatus(ChangePostStatusVM changePostStatus)
        {
            var data = _context.Posts.Where(x=>x.Id== changePostStatus.Id).FirstOrDefault();
            if (data == null)
                return 0;
            data.IsActive = changePostStatus.Status;
            _context.Posts.Update(data);
            _context.SaveChanges();
            return 1;
        }

        public int craetePost(CreatePostVM createPost)
        {
            try
            {
                Post newpost = new Post();
                newpost.UserId = createPost.UserId;
                newpost.CategoryId = createPost.CategoryId;
                newpost.PostTitle = createPost.PostTitle;
                newpost.PostDiscription = createPost.PostDiscription;
                newpost.Quantity = createPost.Quantity;
                newpost.ValidDate = createPost.ValidDate;
                newpost.ItemSize = createPost.ItemSize;
                newpost.Location = createPost.Location;
                newpost.CreatePost = DateTime.Now;
                newpost.IsActive = true;
                _context.Posts.Add(newpost);
                _context.SaveChangesAsync();
                foreach (var post in createPost.Images)
                {
                    var temp = UploadPicture(post, newpost.Id);
                }
            }catch (Exception ex)
            {
                throw;
            }
           
          
           
            return 1;

        }
        
        public List<ResponsePostVM> GetAllPosts()
        {
            DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);

            //disable post before then 30 days
            var disable= _context.Posts.Where(x => x.CreatePost<thirtyDaysAgo && x.IsActive == true).ToList();
            foreach(var item in disable)
            {
                var temp = new ChangePostStatusVM();
                temp.Id = item.Id;
                temp.Status = false;
                var chk = ChnagePostStatus(temp);
            }

            //get all posts
            var data = (from _post in _context.Posts
                        where (_post.CreatePost >= thirtyDaysAgo&& _post.IsActive==true)
                    select new ResponsePostVM()
                    {
                        Id = _post.Id,
                        UserId = _post.UserId,
                        CategoryId= _post.CategoryId,
                        PostTitle= _post.PostTitle,
                        PostDiscription= _post.PostDiscription,
                        Quantity= _post.Quantity,
                        ValidDate=_post.ValidDate,
                        ItemSize= _post.ItemSize,
                        Location= _post.Location,                          
                    }).ToList();

           foreach(var post in data)
            {
               post.Images=GetPicture(post.Id);
            }

            return data;
        }

        public ResponsePostVM GetAllPostsById(int Id)
        {
            DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);
            var data = _context.Posts.Where(x=>x.Id==Id && x.CreatePost >= thirtyDaysAgo&& x.IsActive==true).FirstOrDefault();
            if (data == null)
                return null;

            var post= new ResponsePostVM();
            post.Id = data.Id;
            post.UserId = data.UserId;
            post.CategoryId = data.CategoryId;
            post.PostTitle = data.PostTitle;
            post.PostDiscription = data.PostDiscription;
            post.Quantity = data.Quantity;
            post.ValidDate = data.ValidDate;    
            post.ItemSize = data.ItemSize;
            post.Location = data.Location;
            post.Images = GetPicture(post.Id);
            return post;
        }

        public int UpdatePost(UpdatePostVM UpdatePost)
        {
            var data= _context.Posts.Where(x=>x.Id== UpdatePost.Id).FirstOrDefault();
            if (data == null)
                return 0;
            
            data.UserId = UpdatePost.UserId;
            data.CategoryId = UpdatePost.CategoryId;
            data.PostTitle = UpdatePost.PostTitle;
            data.PostDiscription = UpdatePost.PostDiscription;
            data.Quantity = UpdatePost.Quantity;
            data.ValidDate = UpdatePost.ValidDate;
            data.ItemSize = UpdatePost.ItemSize;
            data.Location = UpdatePost.Location;
            data.UpdatePost = DateTime.Now;

            //remove old pics
            var RemoveOldpic= _context.Picture.Where(x=>x.PostId==data.Id).ToList();
            _context.Picture.RemoveRange(RemoveOldpic);
            _context.SaveChanges(); 

            //add new images
            foreach (var post in UpdatePost.Images)
            {
                var temp = UploadPicture(post, data.Id);
            }

            _context.Posts.Update(data);
            _context.SaveChanges();
            return 1;
        }
        private List<IFormFile> GetPicture(int id)
        {
            var picture = _context.Picture.Where(x => x.PostId == id).ToList();
            if (picture.Count == 0)
            {
                return null;
            }
            var PicList = new List<IFormFile>();
            foreach(var item in  picture)
            {
                var memoryStream = new MemoryStream(item.ImageData);
                var file = new FormFile(memoryStream, 0, memoryStream.Length, "image.jpg", "image/jpeg");

                PicList.Add(file);
            }    
            return PicList;
        }
        private int UploadPicture(IFormFile file, int PostId)
        {
            if (file == null || file.Length == 0)
            {
                return 0;
            }
            using (var memoryStream = new MemoryStream())
            {
                file.CopyToAsync(memoryStream);
                byte[] imageData = memoryStream.ToArray();
                var picture = new Pictures
                { 
                    PostId = PostId,
                    ImageData = imageData
                };
                _context.Picture.Add(picture);
                _context.SaveChangesAsync();

                return 1;
            }
        }
        
    }
}