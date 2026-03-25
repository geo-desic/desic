using AwesomeAssertions;
using Desic.Application.Iso3166Countries;
using Desic.Domain.EntityTypes;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.Iso3166Countries;

public class QueryableExtensionsTests
{
    public class QueryableExtensionsTests001 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_NoFilters_AllItemsReturned()
        {
            // arrange
            var expected = GetItems().ToList();
            var items = GetItems();
            var filter = new Iso3166CountriesFilter();

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableExtensionsTests002 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByAlpha2ThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemA2eA3dId2IsoId1NameC;
            var expected = new List<Domain.Iso3166Countries.Iso3166Country> { expectedItem };
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { Alpha2 = expectedItem.Alpha2 };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests003 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByAlpha2ThatDoesNotMatchAnyItems_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.Iso3166Countries.Iso3166Country>();
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { Alpha2 = "zz" }; // non-existant value

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests004 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByAlpha3ThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemA2eA3dId2IsoId1NameC;
            var expected = new List<Domain.Iso3166Countries.Iso3166Country> { expectedItem };
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { Alpha3 = expectedItem.Alpha3 };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests005 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByAlpha3ThatDoesNotMatchAnyItems_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.Iso3166Countries.Iso3166Country>();
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { Alpha3 = "zzz" }; // non-existant value

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests006 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByIdThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemA2eA3dId2IsoId1NameC;
            var expected = new List<Domain.Iso3166Countries.Iso3166Country> { expectedItem };
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { Id = expectedItem.Id };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests007 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByIdThatDoesNotMatchAnyItems_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.Iso3166Countries.Iso3166Country>();
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { Id = Guid.AllBitsSet }; // non-existant value

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests008 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByIsoIdThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemA2eA3dId2IsoId1NameC;
            var expected = new List<Domain.Iso3166Countries.Iso3166Country> { expectedItem };
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { IsoId = expectedItem.IsoId };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests009 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByIsoIdThatDoesNotMatchAnyItems_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.Iso3166Countries.Iso3166Country>();
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { IsoId = -1 }; // non-existant value

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests010 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByNameThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemA2eA3dId2IsoId1NameC;
            var expected = new List<Domain.Iso3166Countries.Iso3166Country> { expectedItem };
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { Name = expectedItem.Name };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests011 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByNameThatDoesNotMatchAnyItems_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.Iso3166Countries.Iso3166Country>();
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { Name = "DoesNotExist" }; // non-existant value

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests012 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByNameContainsThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemA2eA3dId2IsoId1NameC;
            var expected = new List<Domain.Iso3166Countries.Iso3166Country> { expectedItem };
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { NameContains = expectedItem.Name.Right(length: expectedItem.Name.Length - 1) };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests013 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByNameContainsThatDoesNotMatchAnyItems_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.Iso3166Countries.Iso3166Country>();
            var items = GetItems();
            var filter = new Iso3166CountriesFilter { NameContains = "DoesNotExist" }; // non-existant value

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests014 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByAllThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemA2eA3dId2IsoId1NameC;
            var expected = new List<Domain.Iso3166Countries.Iso3166Country> { expectedItem };
            var items = GetItems();
            var filter = new Iso3166CountriesFilter
            {
                Alpha2 = expectedItem.Alpha2,
                Alpha3 = expectedItem.Alpha3,
                Id = expectedItem.Id,
                IsoId = expectedItem.IsoId,
                Name = expectedItem.Name,
                NameContains = expectedItem.Name,
            };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests015 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByAllThatDoesNotMatchAnItem_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.Iso3166Countries.Iso3166Country>();
            var items = GetItems();
            var filter = new Iso3166CountriesFilter
            {
                // each filter individually matches an item, but none when combined because different source reference is used for the first
                Alpha2 = ItemA2aA3eId3IsoId2NameD.Alpha2,
                Alpha3 = ItemA2eA3dId2IsoId1NameC.Alpha3,
                Id = ItemA2eA3dId2IsoId1NameC.Id,
                IsoId = ItemA2eA3dId2IsoId1NameC.IsoId,
                Name = ItemA2eA3dId2IsoId1NameC.Name,
                NameContains = ItemA2eA3dId2IsoId1NameC.Name,
            };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests016 : QueryableExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(Iso3166CountriesOrderingMethod.Alpha2Asc)]
        [InlineData(Iso3166CountriesOrderingMethod.Alpha2Desc)]
        [InlineData(Iso3166CountriesOrderingMethod.Alpha3Asc)]
        [InlineData(Iso3166CountriesOrderingMethod.Alpha3Desc)]
        [InlineData(Iso3166CountriesOrderingMethod.IdAsc)]
        [InlineData(Iso3166CountriesOrderingMethod.IdDesc)]
        [InlineData(Iso3166CountriesOrderingMethod.IsoIdAsc)]
        [InlineData(Iso3166CountriesOrderingMethod.IsoIdDesc)]
        [InlineData(Iso3166CountriesOrderingMethod.NameAsc)]
        [InlineData(Iso3166CountriesOrderingMethod.NameDesc)]
        public void OrderBy_SpecifiedOrderingMethod_OrdersItemsAsExpected(Iso3166CountriesOrderingMethod? orderingMethod)
        {
            // arrange
            var expected = GetItemsOrdered(orderingMethod).ToList();
            var items = GetItems();

            // act
            var result = QueryableExtensions.OrderBy(source: items.AsQueryable(), orderingMethod: orderingMethod).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableExtensionsTests017 : QueryableExtensionsTests
    {
        [Fact]
        public void SelectToModel_SpecifiedSingleItemQuery_ExpectedModelQueryReturned()
        {
            // arrange
            var createdByType = SystemEntityTypes.Unspecified;
            var modifiedByType = SystemEntityTypes.Label;
            var deletedByType = SystemEntityTypes.User;
            var entity = new Domain.Iso3166Countries.Iso3166Country
            {
                Id = 101.ToGuid(),
                CreatedById = 102.ToGuid(),
                CreatedByName = "CreatedByName",
                CreatedByTypeId = createdByType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 103.ToGuid(),
                ModifiedByName = "ModifiedByName",
                ModifiedByTypeId = modifiedByType.Id,
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                DeletedById = 104.ToGuid(),
                DeletedByName = "DeletedByName",
                DeletedByTypeId = deletedByType.Id,
                DeletedOn = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                IsoId = 101,
                Alpha2 = "zz",
                Alpha3 = "zzz",
                Name = "ExpectedName",
            };
            var expected = new List<Iso3166Country>
            {
                new()
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
                }
            };
            var list = GetItems().ToList();
            list.Add(entity);
            var source = list.AsQueryable().Where(x => x.Id == entity.Id);

            // act
            var result = QueryableExtensions.SelectToModel(source: source).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests018 : QueryableExtensionsTests
    {
        [Fact]
        public void SelectToView_SpecifiedSingleItemQuery_ExpectedModelQueryReturned()
        {
            // arrange
            var createdByType = SystemEntityTypes.Unspecified;
            var modifiedByType = SystemEntityTypes.Label;
            var deletedByType = SystemEntityTypes.User;
            var entity = new Domain.Iso3166Countries.Iso3166Country
            {
                Id = 101.ToGuid(),
                CreatedById = 102.ToGuid(),
                CreatedByName = "CreatedByName",
                CreatedByTypeId = createdByType.Id,
                CreatedOn = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ModifiedById = 103.ToGuid(),
                ModifiedByName = "ModifiedByName",
                ModifiedByTypeId = modifiedByType.Id,
                ModifiedOn = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                DeletedById = 104.ToGuid(),
                DeletedByName = "DeletedByName",
                DeletedByTypeId = deletedByType.Id,
                DeletedOn = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                IsoId = 101,
                Alpha2 = "zz",
                Alpha3 = "zzz",
                Name = "ExpectedName",
            };
            var expected = new List<Iso3166CountryView>
            {
                new()
                {
                    Id = entity.Id,
                    IsoId = entity.IsoId,
                    Alpha2 = entity.Alpha2,
                    Alpha3 = entity.Alpha3,
                    Name = entity.Name,
                }
            };
            var list = GetItems().ToList();
            list.Add(entity);
            var source = list.AsQueryable().Where(x => x.Id == entity.Id);

            // act
            var result = QueryableExtensions.SelectToView(source: source).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    // purposely constructed so that ordering by each property is different
    private static Domain.Iso3166Countries.Iso3166Country ItemA2aA3eId3IsoId2NameD => new() { Alpha2 = "aa", Alpha3 = "eee", Id = 3.ToGuid(), IsoId = 2, Name = "NameD" };
    private static Domain.Iso3166Countries.Iso3166Country ItemA2bA3aId4IsoId3NameE => new() { Alpha2 = "bb", Alpha3 = "aaa", Id = 4.ToGuid(), IsoId = 3, Name = "NameE" };
    private static Domain.Iso3166Countries.Iso3166Country ItemA2cA3bId5IsoId4NameA => new() { Alpha2 = "cc", Alpha3 = "bbb", Id = 5.ToGuid(), IsoId = 4, Name = "NameA" };
    private static Domain.Iso3166Countries.Iso3166Country ItemA2dA3cId1IsoId5NameB => new() { Alpha2 = "dd", Alpha3 = "ccc", Id = 1.ToGuid(), IsoId = 5, Name = "NameB" };
    private static Domain.Iso3166Countries.Iso3166Country ItemA2eA3dId2IsoId1NameC => new() { Alpha2 = "ee", Alpha3 = "ddd", Id = 2.ToGuid(), IsoId = 1, Name = "NameC" };

    private static IEnumerable<Domain.Iso3166Countries.Iso3166Country> GetItems()
    {
        // in no particular order
        yield return ItemA2bA3aId4IsoId3NameE;
        yield return ItemA2dA3cId1IsoId5NameB;
        yield return ItemA2eA3dId2IsoId1NameC;
        yield return ItemA2cA3bId5IsoId4NameA;
        yield return ItemA2aA3eId3IsoId2NameD;
    }

    private static IEnumerable<Domain.Iso3166Countries.Iso3166Country> GetItemsOrdered(Iso3166CountriesOrderingMethod? orderingMethod)
    {
        switch (orderingMethod)
        {
            case Iso3166CountriesOrderingMethod.Alpha2Asc:
                yield return ItemA2aA3eId3IsoId2NameD;
                yield return ItemA2bA3aId4IsoId3NameE;
                yield return ItemA2cA3bId5IsoId4NameA;
                yield return ItemA2dA3cId1IsoId5NameB;
                yield return ItemA2eA3dId2IsoId1NameC;
                break;
            case Iso3166CountriesOrderingMethod.Alpha2Desc:
                yield return ItemA2eA3dId2IsoId1NameC;
                yield return ItemA2dA3cId1IsoId5NameB;
                yield return ItemA2cA3bId5IsoId4NameA;
                yield return ItemA2bA3aId4IsoId3NameE;
                yield return ItemA2aA3eId3IsoId2NameD;
                break;
            case Iso3166CountriesOrderingMethod.Alpha3Asc:
                yield return ItemA2bA3aId4IsoId3NameE;
                yield return ItemA2cA3bId5IsoId4NameA;
                yield return ItemA2dA3cId1IsoId5NameB;
                yield return ItemA2eA3dId2IsoId1NameC;
                yield return ItemA2aA3eId3IsoId2NameD;
                break;
            case Iso3166CountriesOrderingMethod.Alpha3Desc:
                yield return ItemA2aA3eId3IsoId2NameD;
                yield return ItemA2eA3dId2IsoId1NameC;
                yield return ItemA2dA3cId1IsoId5NameB;
                yield return ItemA2cA3bId5IsoId4NameA;
                yield return ItemA2bA3aId4IsoId3NameE;
                break;
            case Iso3166CountriesOrderingMethod.IdAsc:
                yield return ItemA2dA3cId1IsoId5NameB;
                yield return ItemA2eA3dId2IsoId1NameC;
                yield return ItemA2aA3eId3IsoId2NameD;
                yield return ItemA2bA3aId4IsoId3NameE;
                yield return ItemA2cA3bId5IsoId4NameA;
                break;
            case Iso3166CountriesOrderingMethod.IdDesc:
                yield return ItemA2cA3bId5IsoId4NameA;
                yield return ItemA2bA3aId4IsoId3NameE;
                yield return ItemA2aA3eId3IsoId2NameD;
                yield return ItemA2eA3dId2IsoId1NameC;
                yield return ItemA2dA3cId1IsoId5NameB;
                break;
            case Iso3166CountriesOrderingMethod.IsoIdAsc:
                yield return ItemA2eA3dId2IsoId1NameC;
                yield return ItemA2aA3eId3IsoId2NameD;
                yield return ItemA2bA3aId4IsoId3NameE;
                yield return ItemA2cA3bId5IsoId4NameA;
                yield return ItemA2dA3cId1IsoId5NameB;
                break;
            case Iso3166CountriesOrderingMethod.IsoIdDesc:
                yield return ItemA2dA3cId1IsoId5NameB;
                yield return ItemA2cA3bId5IsoId4NameA;
                yield return ItemA2bA3aId4IsoId3NameE;
                yield return ItemA2aA3eId3IsoId2NameD;
                yield return ItemA2eA3dId2IsoId1NameC;
                break;
            // NameAsc ====> skips to default below
            case Iso3166CountriesOrderingMethod.NameDesc:
                yield return ItemA2bA3aId4IsoId3NameE;
                yield return ItemA2aA3eId3IsoId2NameD;
                yield return ItemA2eA3dId2IsoId1NameC;
                yield return ItemA2dA3cId1IsoId5NameB;
                yield return ItemA2cA3bId5IsoId4NameA;
                break;
            default: // NameAsc: default ordering
                yield return ItemA2cA3bId5IsoId4NameA;
                yield return ItemA2dA3cId1IsoId5NameB;
                yield return ItemA2eA3dId2IsoId1NameC;
                yield return ItemA2aA3eId3IsoId2NameD;
                yield return ItemA2bA3aId4IsoId3NameE;
                break;
        }
    }
}
