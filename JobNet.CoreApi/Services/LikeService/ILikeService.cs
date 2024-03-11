namespace JobNet.CoreApi.Services;

public interface ILikeService
{
    Task<string> LikePostWithUserIdAndPostId(int userId, int postId);

    Task<string> UnlikePostWithUserIdAndPostId(int userId, int postId);
}