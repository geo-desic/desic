using Desic.Domain.EntityTypes;
using Desic.Domain.Iso3166Countries;
using Desic.Domain.Labels;
using Desic.Domain.Tags;
using Desic.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Common.Interfaces;

public interface IApplicationDbContext : IBaseDbContext
{
    DbSet<EntityType> EntityTypes { get; }
    DbSet<Iso3166Country> Iso3166Countries { get; }
    DbSet<Label> Labels { get; }
    DbSet<Tag> Tags { get; }
    DbSet<User> Users { get; }
}
