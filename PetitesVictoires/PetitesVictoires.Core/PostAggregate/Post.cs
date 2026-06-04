using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;

namespace PetitesVictoires.Core.PostAggregate;

public class Post : BaseEntity<PostId>, IAggregateRoot
{
    private Post()
    {
    } // for EF Core

    public Post(PostContent content)
    {
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }

    public PostContent Content { get; private set; }


    public Post UpdateContent(PostContent newContent)
    {
        if (Content == newContent) return this;

        Content = newContent;
        MarkUpdated();
        return this;
    }
}
