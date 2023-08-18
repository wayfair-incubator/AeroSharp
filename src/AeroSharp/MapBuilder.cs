using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Configuration;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.DataAccess.MapAccess.Generators;
using AeroSharp.DataAccess.MapAccess.Parsers;
using AeroSharp.DataAccess.Validation;
using AeroSharp.Serialization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp;

/// <inheritdoc cref="IMapBuilder"/>
public sealed class MapBuilder : IDataContextBuilder<IMapBuilder>, IMapBuilder
{
    private static readonly ISet<Type> ValidKeyTypes = new HashSet<Type>
    {
        typeof(long), typeof(double), typeof(string), typeof(byte[]), typeof(bool)
    };

    private readonly IClientProvider _clientProvider;

    private readonly IMapBinParser _mapBinParser;

    private readonly AbstractValidator<DataContext> _dataContextValidator;

    private readonly AbstractValidator<MapContext> _mapContextValidator;

    private readonly AbstractValidator<WriteConfiguration> _writeConfigurationValidator;

    private readonly AbstractValidator<MapConfiguration> _mapConfigurationValidator;

    private DataContext _dataContext;

    private ISerializer _serializer;

    private WriteConfiguration _writeConfiguration;

    private MapConfiguration _mapConfiguration;

    internal MapBuilder(IClientProvider clientProvider)
    {
        _clientProvider = clientProvider;

        _mapBinParser = new MapBinParser();
        _mapConfiguration = new MapConfiguration();
        _writeConfiguration = new WriteConfiguration();
        _dataContextValidator = new DataContextValidator();
        _mapContextValidator = new MapContextValidator();
        _writeConfigurationValidator = new WriteConfigurationValidator();
        _mapConfigurationValidator = new MapConfigurationValidator();
    }

    /// <summary>
    ///     Configures an <see cref="IMapBuilder"/>.
    /// </summary>
    /// <param name="clientProvider"> A <see cref="IClientProvider"/> instance. </param>
    /// <returns> An <see cref="IMapBuilder"/>. </returns>
    public static IDataContextBuilder<IMapBuilder> Configure(IClientProvider clientProvider) =>
        new MapBuilder(clientProvider);

    public IMapBuilder WithDataContext(DataContext dataContext)
    {
        _dataContext = dataContext;

        return this;
    }

    public IMap<TKey, TValue> Build<TKey, TValue>(string key) =>
        BuildMap<TKey, TValue>(new MapContext(key));

    public IMap<TKey, TValue> Build<TKey, TValue>(string key, string bin) =>
        BuildMap<TKey, TValue>(new MapContext(key, bin));

    public IMapOperator<TKey, TValue> Build<TKey, TValue>() =>
        BuildMapOperator<TKey, TValue>();

    public IMapBuilder WithMapConfiguration(MapConfiguration mapConfiguration)
    {
        _mapConfiguration = mapConfiguration;

        return this;
    }

    public IMapBuilder WithWriteConfiguration(WriteConfiguration writeConfiguration)
    {
        _writeConfiguration = writeConfiguration;

        return this;
    }

    public IMapBuilder UseProtobufSerializer()
    {
        _serializer = new ProtobufSerializer();

        return this;
    }

    public IMapBuilder UseMessagePackSerializer()
    {
        _serializer = new MessagePackSerializer();

        return this;
    }

    public IMapBuilder UseMessagePackSerializerWithLz4Compression()
    {
        _serializer = new MessagePackSerializerWithCompression();

        return this;
    }

    public IMapBuilder WithSerializer(ISerializer serializer)
    {
        _serializer = serializer;

        return this;
    }

    private IMap<TKey, TValue> BuildMap<TKey, TValue>(MapContext mapContext)
    {
        _mapContextValidator.ValidateAndThrow(mapContext);

        return new Map<TKey, TValue>(BuildMapOperator<TKey, TValue>(), mapContext);
    }

    private IMapOperator<TKey, TValue> BuildMapOperator<TKey, TValue>()
    {
        var keyType = typeof(TKey);

        if (!ValidKeyTypes.Contains(keyType))
        {
            var validKeyNames = string.Join(", ", ValidKeyTypes.Select(type => $"{type}"));

            throw new UnsupportedKeyTypeException($"Map keys must be one of type {validKeyNames}.");
        }

        if (_dataContext is null)
        {
            throw new ArgumentNullException(nameof(_dataContext));
        }

        if (_clientProvider is null)
        {
            throw new ArgumentNullException(nameof(_clientProvider));
        }

        _dataContextValidator.ValidateAndThrow(_dataContext);
        _writeConfigurationValidator.ValidateAndThrow(_writeConfiguration);
        _mapConfigurationValidator.ValidateAndThrow(_mapConfiguration);

        var recordOperator = new RecordOperator(_clientProvider, _dataContext);
        var mapParser = BuildMapParser();
        var mapEntryGenerator = BuildMapEntryGenerator();

        return new MapOperator<TKey, TValue>(
            mapParser,
            mapEntryGenerator,
            _mapConfiguration,
            recordOperator,
            _writeConfiguration);
    }

    private IMapParser BuildMapParser() => _serializer is null
        ? new MapParser(_mapBinParser)
        : new MapParserWithSerializer(_mapBinParser, _serializer);

    private IMapEntryGenerator BuildMapEntryGenerator() => _serializer is null
        ? new MapEntryGenerator()
        : new MapEntryGeneratorWithSerializer(_serializer);
}
