using System;
using API.Entities;

namespace API.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public required string SenderUsername { get; set; }
    public required string SenderPhotoUrl { get; set; }
    public required string RecipientUsername { get; set; }
    public required string RecipientPhotoUrl { get; set; }
    public required string Content { get; set; }
    public DateTime MessageSent { get; set; }
    public DateTime? MessageRead { get; set; }
    public int RecipientId { get; set; }
}
