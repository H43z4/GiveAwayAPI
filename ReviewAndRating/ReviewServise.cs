using Microsoft.EntityFrameworkCore;
using Models.DatabaseModels.Authentication;
using Models.DatabaseModels.DSSDatabaseObjects.Setup;
using Models.ViewModels.ReviewManegment;
using RepositoryLayer;

namespace ReviewAndRating
{
    public interface IReviewServise
    {
        Task<int> ReviewRequest(CreateReviewVM createReviw);
        Task<List<Review>> GetUserReviwToApprove(int userId);
        Task<int> ApproveReview(int reviewId);
        Task<int> RatingToSender(createRatingVM craeteRating);

    }
    public class ReviewServise : IReviewServise
    {
        private readonly AppDbContext _context;

        public ReviewServise(AppDbContext context)
        {
            _context = context;
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

        public async Task<List<Review>> GetUserReviwToApprove(int userId)
        {
            return await _context.Review.Where(x => x.SenderUserId == userId && x.Status == "Pending").ToListAsync();            
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
            }catch (Exception ex)
            {
                throw;
            }
    
        }

        public async Task<int> ReviewRequest(CreateReviewVM createReviw)
        {
            try
            {
                Review add = new Review();
                add.PostId = createReviw.PostId;
                add.SenderUserId = createReviw.SenderUserId;
                add.ReceverUserId = createReviw.ReceverUserId;
                add.Status = "Pending";
                _context.Review.Add(add);
                await _context.SaveChangesAsync();
                return 1;
            }catch (Exception ex)
            {
                throw;
            }
           
        }
    }
}