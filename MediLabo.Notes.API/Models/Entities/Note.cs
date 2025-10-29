using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediLabo.Notes.API.Models.Entities;

public class Note
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("patId")]
    public required int PatientId { get; set; }

    [BsonElement("note")]
    public required string Content { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false;

    [BsonElement("deletedAt")]
    public DateTime? DeletedAt { get; set; }
}
