using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Configuration;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.OrderedMapAccess;
using AeroSharp.DataAccess.OrderedMapAccess.Generators;
using AeroSharp.DataAccess.OrderedMapAccess.Parsers;
using AeroSharp.DataAccess.Validation;
using AeroSharp.Serialization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp;

/// <inheritdoc cref="IOrderedMapBuilder"/>
public sealed class OrderedMapBuilder : IDataContextBuilder<IOrderedMapBuilder>, IOrderedMapBuilder
{
    private static readonly ISet<Type> ValidKeyTypes = new HashSet<Type>
    {
        typeof(long), typeof(double), typeof(string), typeof(byte[]), typeof(bool)
    };

    private readonly IClientProvider _clientProvider;

    private readonly AbstractValidator<DataContext> _dataContextValidator;

    private readonly AbstractValidator<OrderedMapContext> _orderedMapContextValidator;

    private readonly AbstractValidator<WriteConfiguration> _writeConfigurationValidator;

    private readonly AbstractValidator<OrderedMapConfiguration> _orderedMapConfigurationValidator;

    private DataContext _dataContext;

    private ISerializer _serializer;

    private WriteConfiguration _writeConfiguration;

    private OrderedMapConfiguration _orderedMapConfiguration;

    internal OrderedMapBuilder(IClientProvider clientProvider)
    {
        _clientProvider = clientProvider;

        _orderedMapConfiguration = new OrderedMapConfiguration();
        _writeConfiguration = new WriteConfiguration();
        _dataContextValidator = new DataContextValidator();
        _orderedMapContextValidator = new OrderedMapContextValidator();
        _writeConfigurationValidator = new WriteConfigurationValidator();
        _orderedMapConfigurationValidator = new OrderedMapConfigurationValidator();
    }

    /// <summary>
    ///     Configures an <see cref="IOrderedMapBuilder"/>.
    /// </summary>
    /// <param name="clientProvider"> A <see cref="IClientProvider"/> instance. </param>
    /// <returns> An <see cref="IOrderedMapBuilder"/>. </returns>
    public static IDataContextBuilder<IOrderedMapBuilder> Configure(IClientProvider clientProvider) =>
        new OrderedMapBuilder(clientProvider);

    public IOrderedMapBuilder WithDataContext(DataContext dataContext)
    {
        _dataContext = dataContext;

        return this;
    }

    public IOrderedMap<TSubKey, TOrderKey, TValue> Build<TSubKey, TOrderKey, TValue>(string key) =>
        BuildOrderedMap<TSubKey, TOrderKey, TValue>(new OrderedMapContext(key));

    public IOrderedMap<TSubKey, TOrderKey, TValue> Build<TSubKey, TOrderKey, TValue>(string key, string dataBin, string indexBin) =>
        BuildOrderedMap<TSubKey, TOrderKey, TValue>(new OrderedMapContext(key, dataBin, indexBin));

    public IOrderedMapOperator<TSubKey, TOrderKey, TValue> Build<TSubKey, TOrderKey, TValue>() =>
        BuildOrderedMapOperator<TSubKey, TOrderKey, TValue>();

    public IOrderedMapBuilder WithOrderedMapConfiguration(OrderedMapConfiguration orderedMapConfiguration)
    {
        _orderedMapConfiguration = orderedMapConfiguration;

        return this;
    }

    public IOrderedMapBuilder WithWriteConfiguration(WriteConfiguration writeConfiguration)
    {
        _writeConfiguration = writeConfiguration;

        return this;
    }

    public IOrderedMapBuilder UseProtobufSerializer()
    {
        _serializer = new ProtobufSerializer();

        return this;
    }

    public IOrderedMapBuilder UseMessagePackSerializer()
    {
        _serializer = new MessagePackSerializer();

        return this;
    }

    public IOrderedMapBuilder UseMessagePackSerializerWithLz4Compression()
    {
        _serializer = new MessagePackSerializerWithCompression();

        return this;
    }

    public IOrderedMapBuilder WithSerializer(ISerializer serializer)
    {
        _serializer = serializer;

        return this;
    }

    private IOrderedMap<TSubKey, TOrderKey, TValue> BuildOrderedMap<TSubKey, TOrderKey, TValue>(OrderedMapContext orderedMapContext)
    {
        _orderedMapContextValidator.ValidateAndThrow(orderedMapContext);

        return new OrderedMap<TSubKey, TOrderKey, TValue>(
            BuildOrderedMapOperator<TSubKey, TOrderKey, TValue>(),
            orderedMapContext
        );
    }

    private IOrderedMapOperator<TSubKey, TOrderKey, TValue> BuildOrderedMapOperator<TSubKey, TOrderKey, TValue>()
    {
        ValidateKeyType<TSubKey>("sub-key");
        ValidateKeyType<TOrderKey>("order key");

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
        _orderedMapConfigurationValidator.ValidateAndThrow(_orderedMapConfiguration);

        var recordOperator = new RecordOperator(_clientProvider, _dataContext);
        var parser = BuildOrderedMapParser();
        var generator = BuildOrderedMapEntryGenerator();

        return new OrderedMapOperator<TSubKey, TOrderKey, TValue>(
            parser,
            generator,
            _orderedMapConfiguration,
            recordOperator,
            _writeConfiguration
        );
    }

    private static void ValidateKeyType<TKey>(string keyDescription)
    {
        var keyType = typeof(TKey);

        if (!ValidKeyTypes.Contains(keyType))
        {
            var validKeyNames = string.Join(", ", ValidKeyTypes.Select(type => $"{type}"));

            throw new UnsupportedKeyTypeException($"Ordered map {keyDescription}s must be one of type {validKeyNames}.");
        }
    }

    private IOrderedMapEntryGenerator BuildOrderedMapEntryGenerator() => _serializer is null
        ? new OrderedMapEntryGenerator()
        : new OrderedMapEntryGeneratorWithSerializer(_serializer);

    private IOrderedMapParser BuildOrderedMapParser() => _serializer is null
        ? new OrderedMapParser()
        : new OrderedMapParserWithSerializer(_serializer);
}
