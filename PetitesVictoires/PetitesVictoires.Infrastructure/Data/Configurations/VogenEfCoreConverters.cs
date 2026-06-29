using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using Vogen;

namespace PetitesVictoires.Infrastructure.Data.Configurations;

[EfCoreConverter<PostId>]
[EfCoreConverter<PostContent>]
[EfCoreConverter<UserId>]
[EfCoreConverter<Email>]
[EfCoreConverter<UserName>]
internal partial class VogenEfCoreConverters;
