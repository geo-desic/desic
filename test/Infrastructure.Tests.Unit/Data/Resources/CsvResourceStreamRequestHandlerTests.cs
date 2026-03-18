using AwesomeAssertions;
using Desic.Infrastructure.Data.Resources;
using Microsoft.Extensions.Logging.Testing;
using System.Text;

namespace Desic.Infrastructure.Tests.Unit.Data.Resources;

public class CsvResourceStreamRequestHandlerTests
{
    private readonly FakeLogger<CsvResourceStreamRequestHandler<TestResource>> _logger = new();

    private readonly Type ClassMapType = typeof(TestResourceClassMap);
    private const string ResourceFileNameUtf8 = "TestResourceUtf8.csv";
    private const string ResourceFileNameUtf16Le = "TestResourceUtf16Le.csv";
    private const string ResourcePrefix = "Desic.Infrastructure.Tests.Unit.Data.Resources";
    private const string ResourceNameUtf8 = $"{ResourcePrefix}.{ResourceFileNameUtf8}";
    private const string ResourceNameUtf16Le = $"{ResourcePrefix}.{ResourceFileNameUtf16Le}";
    private const int TotalRecordCount = 5;

    public class CsvResourceStreamRequestHandlerTests001 : CsvResourceStreamRequestHandlerTests
    {
        [Fact]
        public async Task Handle_NonExistantResource_ErrorLoggedAndExpectedException()
        {
            // arrange
            var handler = new CsvResourceStreamRequestHandler<TestResource>(logger: _logger);
            var request = new CsvResourceStreamRequest<TestResource>
            {
                Assembly = typeof(IAssemblyReference).Assembly,
                ClassMapType = ClassMapType,
                ResourceName = $"{ResourcePrefix}.NonExistantResource.csv",
            };
            var act = async () => { await foreach (var item in handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken)) { /* nothing */ } };

            // act / assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage(expectedWildcardPattern: "Invalid resource name*");
        }
    }

    public class CsvResourceStreamRequestHandlerTests002 : CsvResourceStreamRequestHandlerTests
    {
        [Fact]
        public async Task Handle_ValidRequestWithNoEncodingSpecified_Utf8EncodingUsedAndExpectedRecordsReturned()
        {
            // arrange
            var handler = new CsvResourceStreamRequestHandler<TestResource>(logger: _logger);
            var request = new CsvResourceStreamRequest<TestResource>
            {
                Assembly = typeof(IAssemblyReference).Assembly,
                ClassMapType = ClassMapType,
                ResourceName = ResourceNameUtf8,
            };
            var records = new List<TestResource>();

            // act
            await foreach (var item in handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken))
            {
                records.Add(item);
            }

            // assert
            records.Should().BeEquivalentTo(ExpectedRecords());
        }
    }

    public class CsvResourceStreamRequestHandlerTests003 : CsvResourceStreamRequestHandlerTests
    {
        [Fact]
        public async Task Handle_ValidRequestWithUtf16LeEncodingSpecified_ExpectedRecordsReturned()
        {
            // arrange
            var handler = new CsvResourceStreamRequestHandler<TestResource>(logger: _logger);
            var request = new CsvResourceStreamRequest<TestResource>
            {
                Assembly = typeof(IAssemblyReference).Assembly,
                ClassMapType = ClassMapType,
                Encoding = Encoding.Unicode, // this is the encoding for utf 16 little endian
                ResourceName = ResourceNameUtf16Le,
            };
            var records = new List<TestResource>();

            // act
            await foreach (var item in handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken))
            {
                records.Add(item);
            }

            // assert
            records.Should().BeEquivalentTo(ExpectedRecords());
        }
    }

    private static IEnumerable<TestResource> ExpectedRecords()
    {
        for (var i = 1; i <= TotalRecordCount; ++i)
        {
            yield return new TestResource { Id = i, Name = $"Name{i}", Value = $"Value{i}" };
        }
    }
}
