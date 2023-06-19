using Microsoft.EntityFrameworkCore;
using Models.DatabaseModels.Authentication;
using Models.DatabaseModels.DSSDatabaseObjects.Setup;
using Models.ViewModels.DSAuth.Setup;
using Models.ViewModels.PostManagemrnts;
using Models.ViewModels.ReviewManegment;
using PostManagement;
using RepositoryLayer;
using SharedLib.Interfaces;
using UserManagement;

namespace ReviewAndRating
{
    public interface IReviewServise : ICurrentDSUser
    {
        Task<int> ReviewRequest(CreateReviewVM createReviw);
        Task<List<CreateReviewVM>> GetUserReviwToApprove();
        Task<List<CreateReviewVM>> GetUserReviwRequested();
        Task<int> ApproveReview(int reviewId);
        Task<int> RatingToSender(createRatingVM craeteRating);

    }
    public class ReviewServise : IReviewServise
    {
        private readonly AppDbContext _context;
        private readonly IPostManagementService _postManagement;
        private readonly IUserManagement UserManagement;
        public VwDSUser VwUser { get; set; }
        public VwDSUser VwDSUser { get; set; }
        public ReviewServise(AppDbContext context, IPostManagementService postManagement, IUserManagement userManagement)
        {
            _context = context;
            _postManagement = postManagement;
            UserManagement = userManagement;


        }

        public async Task<int> ApproveReview(int reviewId)
        {
            var data = await _context.Review.Where(x => x.ReviewId == reviewId && x.Status == "Pending").FirstOrDefaultAsync();
            if (data == null)
                return 0;
            data.Status = "Approved";
            _context.Review.Update(data);
            await _context.SaveChangesAsync();
            return 1;
        }

        public async Task<List<CreateReviewVM>> GetUserReviwToApprove()
        {
            var approvals =  await _context.Review.Where(x => x.ReceverUserId == (int)VwDSUser.UserId && x.Status == "Pending").ToListAsync();
            if (approvals.Count>0)
            {
                var ReviewList = new  List<CreateReviewVM>();
                foreach (var item in approvals)
                {
                    var reviwWDpost = new CreateReviewVM();
                    reviwWDpost.Post= await _postManagement.GetPostsById(item.PostId);
                    reviwWDpost.SenderUserName = UserManagement.GetUserByUserId(item.SenderUserId).Result.FullName;
                    ReviewList.Add(reviwWDpost);
                }
                return ReviewList;
            }
            return null;
        }
        public async Task<List<CreateReviewVM>> GetUserReviwRequested()
        {
             var requests = await _context.Review.Where(x => x.SenderUserId == (int)VwDSUser.UserId && x.Status == "Pending").ToListAsync();
            if (requests.Count > 0)
            {
                var ReviewList = new List<CreateReviewVM>();
                foreach (var item in requests)
                {
                    var reviwWDpost = new CreateReviewVM();
                    reviwWDpost.Post = await _postManagement.GetPostsById(item.PostId);
                    reviwWDpost.ReceverUserName = UserManagement.GetUserByUserId(item.ReceverUserId).Result.FullName;
                    ReviewList.Add(reviwWDpost);
                }
                return ReviewList;
            }
            return null;
        }

        public async Task<int> RatingToSender(createRatingVM craeteRating)
        {
            try
            {
                Rating obj = new Rating();
                obj.SendReviewUserId = craeteRating.SendReviewUserId;
                obj.RecevedReviewUserId = craeteRating.RecevedReviewUserId;
                obj.PostId = craeteRating.PostId;
                obj.PostRating = craeteRating.PostRating;
                obj.ProfileRating = craeteRating.ProfileRating;

                _context.Rating.Add(obj);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<int> ReviewRequest(CreateReviewVM createReviw)
        {

            try
            {
                var requestData = await _context.Review.Where(x => x.PostId == createReviw.PostId && x.SenderUserId == (int)VwDSUser.UserId && x.ReceverUserId == createReviw.ReceverUserId).FirstOrDefaultAsync();
                if (requestData == null)
                {
                    Review add = new Review();
                    add.PostId = createReviw.PostId;
                    add.SenderUserId = (int)VwDSUser.UserId;
                    add.ReceverUserId = createReviw.ReceverUserId;
                    add.Status = "Pending";
                    _context.Review.Add(add);
                    await _context.SaveChangesAsync();
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}