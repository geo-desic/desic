using AwesomeAssertions;
using Desic.Application.Iso3166Countries;
using Desic.Domain.EntityTypes;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.Iso3166Countries;

public class Iso3166CountryTests
{
    public class Iso3166CountryTests001 : Iso3166CountryTests
    {
        [Fact]
        public void Contructor_WithSoftIso3166CountryEntity_AllPropertiesMappedCorrectly()
        {
            // arrange
            var createdByType = SystemEntityTypes.Unspecified;
            var modifiedByType = SystemEntityTypes.Label;
            var deletedByType = SystemEntityTypes.User;
            var entity = new Domain.Iso3166Countries.Iso3166Country
            {
                Id = 1.ToGuid(),
                CreatedById = 2.ToGuid(),
                CreatedByName = "CreatedByName",
                CreatedByTypeId = createdByType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 3.ToGuid(),
                ModifiedByName = "ModifiedByName",
                ModifiedByTypeId = modifiedByType.Id,
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                DeletedById = 4.ToGuid(),
                DeletedByName = "DeletedByName",
                DeletedByTypeId = deletedByType.Id,
                DeletedOn = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                IsoId = 1,
                Alpha2 = "aa",
                Alpha3 = "bbb",
                Name = "ExpectedName",
            };
            var expected = new Iso3166Country
            {
                Id = entity.Id,
                Created = new()
                {
                    By = new()
                    {
                        Id = entity.CreatedById,
                        Name = entity.CreatedByName,
                        Type = new()
                        {
                            Key = createdByType.Key,
                            Name = createdByType.Name,
                        },
                    },
                    On = entity.CreatedOn,
                },
                Modified = new()
                {
                    By = new()
                    {
                        Id = entity.ModifiedById,
                        Name = entity.ModifiedByName,
                        Type = new()
                        {
                            Key = modifiedByType.Key,
                            Name = modifiedByType.Name,
                        },
                    },
                    On = entity.ModifiedOn,
                },
                Deleted = new()
                {
                    By = new()
                    {
                        Id = entity.DeletedById,
                        Name = entity.DeletedByName,
                        Type = new()
                        {
                            Key = deletedByType.Key,
                            Name = deletedByType.Name,
                        },
                    },
                    On = entity.DeletedOn,
                },
                IsoId = entity.IsoId,
                Alpha2 = entity.Alpha2,
                Alpha3 = entity.Alpha3,
                Name = entity.Name,
            };

            // act
            var result = new Iso3166Country(entity);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
