using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;

namespace JobNet.CoreApi.Services.CommentService;

public interface ICommentService
{
    Task<List<GetAllCommentsSimpleApiResponse>> GetAllComments();

    Task<List<GetAllCommentsSimpleApiResponse>> GetAllCommentsNotDeleted();
    
    Task<string> AddComment(int userId, int postId, CreateCommmentApiRequest createCommentApiRequest);

    Task<string> DeleteComment(int userId, int postId, int commentId);
}