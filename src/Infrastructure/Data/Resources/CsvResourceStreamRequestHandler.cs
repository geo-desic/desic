using CsvHelper;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Desic.Infrastructure.Data.Resources;

public class CsvResourceStreamRequestHandler<T>(ILogger<CsvResourceStreamRequestHandler<T>> logger)
{
    private readonly ILogger<CsvResourceStreamRequestHandler<T>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async IAsyncEnumerable<T> Handle(CsvResourceStreamRequest<T> request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        request.Encoding ??= System.Text.Encoding.UTF8;
        var culture = CultureInfo.InvariantCulture;
        _logger.LogDebug("Creating stream for resource {ResourceName} in {AssemblyName}", request.ResourceName, request.Assembly.FullName);
        using var stream = request.Assembly.GetManifestResourceStream(request.ResourceName);
        if (stream == null)
        {
            _logger.LogError("Stream for resource {ResourceName} is null", request.ResourceName);
            throw new ArgumentException("Invalid resource name", nameof(request));
        }
        _logger.LogDebug("Creating stream reader with encoding {EncodingName} for previously created stream", request.Encoding.EncodingName);
        using var streamReader = new StreamReader(stream, request.Encoding);
        _logger.LogTrace("Creating csv reader with culture {CultureName} for previously created stream reader", culture.Name);
        using var csvReader = new CsvReader(streamReader, culture);
        if (request.ClassMapType != null)
        {
            _logger.LogDebug("Registering the class map type supplied in the request with the context of the csv reader");
            csvReader.Context.RegisterClassMap(request.ClassMapType);
        }
        await csvReader.ReadAsync();
        csvReader.ReadHeader();
        while (await csvReader.ReadAsync() && !cancellationToken.IsCancellationRequested)
        {
            yield return csvReader.GetRecord<T>();
        }
    }
}
