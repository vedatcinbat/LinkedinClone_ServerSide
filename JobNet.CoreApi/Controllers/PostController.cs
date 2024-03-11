using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Services.PostService;
using Microsoft.AspNetCore.Mvc;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController(IPostService postService) : ControllerBase
{
    private readonly IPostService _postService = postService;



    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var response = await _postService.GetAllPosts();

        return Ok(response);
    }

    [HttpGet("{postId:int}")]
    public async Task<IActionResult> GetOnePostWithPostId([FromRoute] int postId)
    {
        var post = await _postService.GetOnePost(postId);

        return Ok(post);
    }
    

    [HttpPost("{userId:int}/post")]
    public async Task<IActionResult> CreatePost(int userId, CreatePostApiRequest createPostApiRequest)
    {
        var response = await _postService.CreatePost(userId, createPostApiRequest);

        return Ok(response);
    }
    
    
    
    
}