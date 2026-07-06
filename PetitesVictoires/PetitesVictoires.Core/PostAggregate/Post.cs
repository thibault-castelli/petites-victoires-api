using Ardalis.SharedKernel;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Core.PostAggregate;

public class Post : BaseEntity<PostId>, IAggregateRoot
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


    public Post UpdateContent(PostContent newContent)
    {
        if (Content == newContent) return this;

        Content = newContent;
        return this;
    }
}
