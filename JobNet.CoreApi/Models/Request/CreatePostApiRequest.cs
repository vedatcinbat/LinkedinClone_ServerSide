using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Models.Request;

public class CreatePostApiRequest
{
    public string Caption { get; set; }

    public PostType PostType { get; set; }
    
    public string? TextContent { get; set; }
    
    public string? ImageContent { get; set; }
    
    public string? ImagesContent { get; set; }
    
}