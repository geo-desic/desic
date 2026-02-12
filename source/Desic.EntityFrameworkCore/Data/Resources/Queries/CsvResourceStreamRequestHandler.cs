using CsvHelper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Desic.EntityFrameworkCore.Data.Resources.Queries;

public class CsvResourceStreamRequestHandler<T>(ILogger<CsvResourceStreamRequestHandler<T>> logger) : IStreamRequestHandler<CsvResourceStreamRequest<T>, T>
{
    private readonly ILogger<CsvResourceStreamRequestHandler<T>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async IAsyncEnumerable<T> Handle(CsvResourceStreamRequest<T> request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        request.Encoding ??= System.Text.Encoding.UTF8;
        var culture = CultureInfo.InvariantCulture;
        var assembly = typeof(CsvResourceStreamRequestHandler<T>).Assembly;
        _logger.LogDebug("Creating stream for resource {resourceName} in {assemblyName}", request.ResourceName, assembly.FullName);
        using var stream = assembly.GetManifestResourceStream(request.ResourceName);
        if (stream == null)
        {
            _logger.LogError("Stream for resource {resourceName} is null", request.ResourceName);
            throw new ArgumentException("Invalid resource name", nameof(request));
        }
        _logger.LogDebug("Creating stream reader with encoding {encodingName} for previously created stream", request.Encoding.EncodingName);
        using var streamReader = new StreamReader(stream, request.Encoding);
        _logger.LogTrace("Creating csv reader with culture {cultureName} for previously created stream reader", culture.Name);
        using var csvReader = new CsvReader(streamReader, culture);
        if (request.ClassMapType != null)
        {
            _logger.LogDebug("Registering the class map type supplied in the request with the context of the csv reader");
            csvReader.Context.RegisterClassMap(request.ClassMapType);
        }
        csvReader.Read();
        csvReader.ReadHeader();
        while (csvReader.Read() && !cancellationToken.IsCancellationRequested)
        {
            yield return csvReader.GetRecord<T>();
        }
    }
}
