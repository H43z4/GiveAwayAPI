using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DatabaseModels.DSSDatabaseObjects.Setup;
using Models.ViewModels.DSAuth.Setup;
using Models.ViewModels.PostManagemrnts;
using RepositoryLayer;
using SharedLib.Interfaces;
using UserManagement;

namespace PostManagement
{
    public interface IPostManagementService : ICurrentDSUser
    {
        Task<int> craetePost(CreatePostVM createPost);
        Task<int> UpdatePost(UpdatePostVM createPost);
        Task<int> DeletePost(int postId);
        Task<int> ChnagePostStatus(ChangePostStatusVM changePostStatus);
        Task<int> ChnagePostApprovalStatus(ChangePostStatusVM changePostStatus);
        Task<List<ResponsePostVM>> GetAllPosts();
        Task<List<ResponsePostVM>> GetAllInActivePosts();
        Task<List<CountsByCategory>> GetCountByCategory();
        Task<List<CountsByUserType>> GetCountsByUserType();
        Task<List<ResponsePostVM>> SearchPostsbyTitle(string searchstr, int catogaryid);
        Task<ResponsePostVM> GetPostsById(int Id);
        Task<UserWithPostsVM> GetUserByIdWithPosts();
    }
    public class PostManagementService : IPostManagementService
    {
        private readonly AppDbContext _context;
        private readonly IUserManagement UserManagement;
        public VwDSUser VwUser { get; set; }
        public VwDSUser VwDSUser { get; set; }
        public PostManagementService(AppDbContext context, IUserManagement userManagement)
        {
            _context = context;
            UserManagement = userManagement;
        }

        public async Task<int> ChnagePostStatus(ChangePostStatusVM changePostStatus)
        {
            var data = await _context.Posts.Where(x => x.Id == changePostStatus.Id).FirstOrDefaultAsync();
            if (data == null)
                return 0;
            data.IsActive = changePostStatus.Status;
            _context.Posts.Update(data);
            await _context.SaveChangesAsync();
            return 1;
        }  
        public async Task<int> ChnagePostApprovalStatus(ChangePostStatusVM changePostStatus)
        {
            var data = await _context.Posts.Where(x => x.Id == changePostStatus.Id).FirstOrDefaultAsync();
            if (data == null)
                return 0;
            data.ApproveStatus = changePostStatus.Status;
            _context.Posts.Update(data);
            await _context.SaveChangesAsync();
            return 1;
        }

        public async Task<int> craetePost(CreatePostVM createPost)
        {
            try
            {
                Post newpost = new Post();
                newpost.UserId = (int)VwDSUser.UserId;
                newpost.CategoryId = createPost.CategoryId;
                newpost.PostTitle = createPost.PostTitle;
                newpost.PostDiscription = createPost.PostDiscription;
                newpost.Quantity = createPost.Quantity;
                if (createPost.ValidDate != null)
                {
                    newpost.ValidDate = DateTime.Parse(createPost.ValidDate);
                }
                newpost.ItemSize = createPost.ItemSize;
                newpost.Location = createPost.Location;
                newpost.CreatePost = DateTime.Now;
                newpost.IsActive = true;
                _context.Posts.Add(newpost);
                await _context.SaveChangesAsync();
                if (createPost.Images != null)
                {

                    foreach (var post in createPost.Images)
                    {
                        var temp = UploadPicture(post, newpost.Id);
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public async Task<int> DeletePost(int postId)
        {
            var data = await _context.Posts.Where(x => x.Id == postId).FirstOrDefaultAsync();
            if (data != null)
            {
                data.IsActive = false;
                _context.Posts.Update(data);
                await _context.SaveChangesAsync();
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public async Task<List<ResponsePostVM>> GetAllInActivePosts()
        {
            DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);

            //get all posts
            var data = await (from _post in _context.Posts
                              where (_post.CreatePost >= thirtyDaysAgo && _post.IsActive == true && _post.ApproveStatus == null)
                              select new ResponsePostVM()
                              {
                                  Id = _post.Id,
                                  UserId = _post.UserId,
                                  CategoryId = _post.CategoryId,
                                  PostTitle = _post.PostTitle,
                                  PostDiscription = _post.PostDiscription,
                                  Quantity = _post.Quantity,
                                  ValidDate = _post.ValidDate,
                                  ItemSize = _post.ItemSize,
                                  Location = _post.Location,
                              }).ToListAsync();

            return data;
        }
        public async Task<List<ResponsePostVM>> GetAllPosts()
        {
            DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);

            //disable post before then 30 days
            var disable = await _context.Posts.Where(x => x.CreatePost < thirtyDaysAgo && x.IsActive == true).ToListAsync();
            foreach (var item in disable)
            {
                var temp = new ChangePostStatusVM();
                temp.Id = item.Id;
                temp.Status = false;
                var chk = ChnagePostStatus(temp);
            }

            //get all posts
            var data = await (from _post in _context.Posts
                              where (_post.CreatePost >= thirtyDaysAgo && _post.IsActive == true)
                              select new ResponsePostVM()
                              {
                                  Id = _post.Id,
                                  UserId = _post.UserId,
                                  CategoryId = _post.CategoryId,
                                  PostTitle = _post.PostTitle,
                                  PostDiscription = _post.PostDiscription,
                                  Quantity = _post.Quantity,
                                  ValidDate = _post.ValidDate,
                                  ItemSize = _post.ItemSize,
                                  Location = _post.Location,
                              }).ToListAsync();

            foreach (var post in data)
            {
                post.Images = GetPicture(post.Id);

            }

            return data;
        }
        public async Task<List<CountsByUserType>> GetCountsByUserType()
        {
            var postCountsByUserType = await _context.User
        .Where(x => x.UserStatusId == 1)
        .Select(p => new CountsByUserType
        {
            OrganizationalCounts = p.UserTypeId == 2 ? 1 : 0,
            IndividualCounts= p.UserTypeId == 1 ? 1 : 0,
            OtherCounts = p.UserTypeId == 0 ? 1 : 0
        })
        .GroupBy(_ => 1)
    //.Select(g => new CountsByCategory { categoryId = g.Key, totalCount = g.Count() })
    .Select(g => new CountsByUserType
    {
        OrganizationalCounts = g.Sum(p => p.OrganizationalCounts),
        IndividualCounts = g.Sum(p => p.IndividualCounts),
        OtherCounts = g.Sum(p => p.OtherCounts),
    })
        .ToListAsync();

            return postCountsByUserType;
        }
        public async Task<List<CountsByCategory>> GetCountByCategory()
        {
            var postCountByCategory = await _context.Posts
        .Where(x => x.IsActive == true)
        .Select(p => new CountsByCategory
        {
            WantedCounts = p.CategoryId == 4 ? 1 : 0,
            BorrowCounts = p.CategoryId == 3 ? 1 : 0,
            NonFoodCounts = p.CategoryId == 2 ? 1 : 0,
            FoodCounts = p.CategoryId == 1 ? 1 : 0
        })
        .GroupBy(_ => 1)
    //.Select(g => new CountsByCategory { categoryId = g.Key, totalCount = g.Count() })
    .Select(g => new CountsByCategory
    {
        WantedCounts = g.Sum(p => p.WantedCounts),
        BorrowCounts = g.Sum(p => p.BorrowCounts),
        NonFoodCounts = g.Sum(p => p.NonFoodCounts),
        FoodCounts = g.Sum(p => p.FoodCounts)
    })
        .ToListAsync();

            return postCountByCategory;
        }
        public async Task<ResponsePostVM> GetPostsById(int Id)
        {
            try
            {
                DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);
                var data = _context.Posts.Where(x => x.Id == Id && x.CreatePost >= thirtyDaysAgo && x.IsActive == true).FirstOrDefault();
                if (data == null)
                    return null;

                var datapost = new ResponsePostVM();
                datapost.Id = data.Id;
                datapost.UserId = data.UserId;
                var userDetail = _context.User.Where(x=>x.UserId == datapost.UserId ).FirstOrDefault();
                if (userDetail != null)
                {
                    datapost.UserName = userDetail.FullName;
                }
                datapost.CategoryId = data.CategoryId;
                datapost.PostTitle = data.PostTitle;
                datapost.PostDiscription = data.PostDiscription;
                datapost.Quantity = data.Quantity;
                datapost.ValidDate = data.ValidDate;
                datapost.ItemSize = data.ItemSize;
                datapost.Location = data.Location;
                datapost.CreateDate = data.CreatePost;
                datapost.Images = await Task.Run(() => GetPicture(datapost.Id));
                return datapost;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<UserWithPostsVM> GetUserByIdWithPosts()
        {
            try
            {
                var UserId = (int)VwDSUser.UserId;

                DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);
                var userWithPostsVM = new UserWithPostsVM();
                var data = await UserManagement.GetUserByUserId(UserId);
                if (data == null)
                    return null;
                userWithPostsVM.FullName = data.FullName;
                userWithPostsVM.Address = data.Address;
                userWithPostsVM.PhoneNumber = data.PhoneNumber;
                userWithPostsVM.Email = data.Email;
                userWithPostsVM.userId = data.UserId;

                userWithPostsVM.Posts = await (from _post in _context.Posts
                                               where (_post.CreatePost >= thirtyDaysAgo && _post.IsActive == true && _post.UserId == UserId)
                                               select new ResponsePostVM()
                                               {
                                                   Id = _post.Id,
                                                   UserId = _post.UserId,
                                                   CategoryId = _post.CategoryId,
                                                   PostTitle = _post.PostTitle,
                                                   PostDiscription = _post.PostDiscription,
                                                   Quantity = _post.Quantity,
                                                   ValidDate = _post.ValidDate,
                                                   ItemSize = _post.ItemSize,
                                                   Location = _post.Location,
                                               }).ToListAsync();
                userWithPostsVM.ReqPosts = await (from _post in _context.Posts
                                               where (_post.CreatePost >= thirtyDaysAgo && _post.IsActive == true && _post.UserId != UserId)
                                               select new ResponsePostVM()
                                               {
                                                   Id = _post.Id,
                                                   UserId = _post.UserId,
                                                   CategoryId = _post.CategoryId,
                                                   PostTitle = _post.PostTitle,
                                                   PostDiscription = _post.PostDiscription,
                                                   Quantity = _post.Quantity,
                                                   ValidDate = _post.ValidDate,
                                                   ItemSize = _post.ItemSize,
                                                   Location = _post.Location,
                                               }).Take(2).ToListAsync();

                //foreach (var post in userWithPostsVM.Posts)
                //{
                //    post.Images = GetPicture(post.Id);

                //}
                return userWithPostsVM;
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        public async Task<List<ResponsePostVM>> SearchPostsbyTitle(string searchstr, int catogaryid)
        {
            DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);
            var data = new List<ResponsePostVM>();
            if (catogaryid > 0 && (searchstr == null || searchstr.Trim() == "")) { 
            data = await (from _post in _context.Posts
                          where (_post.CreatePost >= thirtyDaysAgo && _post.IsActive == true && _post.CategoryId == catogaryid)
                          select new ResponsePostVM()
                          {
                              Id = _post.Id,
                              UserId = _post.UserId,
                              CategoryId = _post.CategoryId,
                              PostTitle = _post.PostTitle,
                              PostDiscription = _post.PostDiscription,
                              Quantity = _post.Quantity,
                              ValidDate = _post.ValidDate,
                              ItemSize = _post.ItemSize,
                              Location = _post.Location,
                          }).ToListAsync();
            }
            else if (searchstr != null && searchstr.Trim() != "" && catogaryid > 0)
            {
                data = await (from _post in _context.Posts
                              where (_post.CreatePost >= thirtyDaysAgo && _post.IsActive == true && _post.PostTitle.Contains(searchstr) && _post.CategoryId == catogaryid)
                              select new ResponsePostVM()
                              {
                                  Id = _post.Id,
                                  UserId = _post.UserId,
                                  CategoryId = _post.CategoryId,
                                  PostTitle = _post.PostTitle,
                                  PostDiscription = _post.PostDiscription,
                                  Quantity = _post.Quantity,
                                  ValidDate = _post.ValidDate,
                                  ItemSize = _post.ItemSize,
                                  Location = _post.Location,
                              }).ToListAsync();
            }else if (searchstr != null && searchstr.Trim() != "" && catogaryid == 0)
            {
                data = await (from _post in _context.Posts
                              where (_post.CreatePost >= thirtyDaysAgo && _post.IsActive == true && _post.PostTitle.Contains(searchstr))
                              select new ResponsePostVM()
                              {
                                  Id = _post.Id,
                                  UserId = _post.UserId,
                                  CategoryId = _post.CategoryId,
                                  PostTitle = _post.PostTitle,
                                  PostDiscription = _post.PostDiscription,
                                  Quantity = _post.Quantity,
                                  ValidDate = _post.ValidDate,
                                  ItemSize = _post.ItemSize,
                                  Location = _post.Location,
                              }).ToListAsync();
            }
            else
            {
                var allData = GetAllPosts();
                data = allData.Result;
            }
            foreach (var post in data)
            {
                post.Images = GetPicture(post.Id);
            }

            return data;
        }
        public async Task<int> UpdatePost(UpdatePostVM UpdatePost)
        {
            var data = await _context.Posts.Where(x => x.Id == UpdatePost.Id).FirstOrDefaultAsync();
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
            //var RemoveOldpic= _context.Pictures.Where(x=>x.PostId==data.Id).ToList();
            //_context.Pictures.RemoveRange(RemoveOldpic);
            //await _context.SaveChangesAsync(); 

            //add new images
            foreach (var post in UpdatePost.Images)
            {
                var temp = UploadPicture(post, data.Id);
            }

            _context.Posts.Update(data);
            await _context.SaveChangesAsync();
            return 1;
        }
        private List<string> GetPicture(int id)
        {
            var picture = _context.Pictures.Where(x => x.PostId == id).ToList();
            if (picture.Count == 0)
            {
                return null;
            }
            var PicList = new List<string>();
            foreach (var item in picture)
            {
                //    var memoryStream = new MemoryStream(item.ImageData);
                //    var file = new FormFile(memoryStream, 0, memoryStream.Length, "image.jpg", "image/jpeg");
                //var imageUrls = $"data:image/jpeg;base64,{Convert.ToBase64String(item.ImageData)}");
                var imageUrls = $"data:image/jpeg;base64,{Convert.ToBase64String(item.ImageData)}";

                PicList.Add(imageUrls);
            }
            return PicList;
        }
        private async Task<int> UploadPicture(IFormFile file, int PostId)
        {
            if (file == null || file.Length == 0)
            {
                return 0;
            }
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    byte[] imageData = memoryStream.ToArray();
                    var picture = new Pictures
                    {
                        PostId = PostId,
                        ImageData = imageData
                    };
                    _context.Pictures.Add(picture);
                    await _context.SaveChangesAsync();

                    return 1;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}