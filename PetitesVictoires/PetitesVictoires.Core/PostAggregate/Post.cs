using Ardalis.SharedKernel;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Core.PostAggregate;

public class Post : BaseEntity<PostId>, IAggregateRoot, IAuditable, ISoftDeletable
{
    private Post()
    {
    } // for EF Core

    public Post(PostContent content, UserId userId)
    {
        Content = content;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public PostContent Content { get; private set; }
    public UserId UserId { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public DateTime? DeletedAt { get; private set; }


    public void MarkSoftDeleted()
    {
        DeletedAt = DateTime.UtcNow;
    }

    public Post UpdateContent(PostContent newContent)
    {
        if (Content == newContent) return this;

        Content = newContent;
        return this;
    }
}
