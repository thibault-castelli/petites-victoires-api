using PetitesVictoires.Core.PostAggregate;
using Vogen;

namespace PetitesVictoires.Infrastructure.Data.Configurations;

[EfCoreConverter<PostId>]
[EfCoreConverter<PostContent>]
internal partial class VogenEfCoreConverters;
