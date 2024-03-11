using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;

namespace JobNet.CoreApi.Services.PostService;

public interface IPostService
{
    Task<List<PostSimpleApiResponse>> GetAllPosts();

    Task<PostSimpleApiResponse> GetOnePost(int postId);
    Task<CreatePostApiResponse> CreatePost(int userId, CreatePostApiRequest createPostApiRequest);
    
}