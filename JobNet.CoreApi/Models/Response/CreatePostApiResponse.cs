﻿using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Models.Response;

public class CreatePostApiResponse
{
    public int PostId { get; set; }
    
    public bool IsDeleted { get; set; }
    
    [ForeignKey("UserId")]
    public int? UserId { get; set; }
    
    public UserPostSimpleResponse User { get; set; }
    
    public DateTime PublishTime { get; set; }
    
    public string Caption { get; set; }

    public PostType PostType;
    
    public string? TextContent { get; set; }
    
    public string? ImageContent { get; set; }
    
    public string? ImagesContent { get; set; }
    
    public int CommentCount { get; set; }
    
    public int LikeCount { get; set; }
    
    public ICollection<CommentSimpleResponse> Comments { get; set; } = new List<CommentSimpleResponse>();

    public ICollection<LikeSimpleResponse> Likes { get; set; } = new List<LikeSimpleResponse>();
}