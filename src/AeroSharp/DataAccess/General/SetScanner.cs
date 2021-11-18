using System;
using System.Collections.Generic;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.Internal.Parsers;
using AeroSharp.Serialization;
using Aerospike.Client;

namespace AeroSharp.DataAccess.General
{
    /// <inheritdoc cref="ISetScanner"/>
    internal class SetScanner : ISetScanner
    {
        private readonly ISetScanOperator _operator;
        private readonly ISerializer _serializer;
        private readonly DataContext _dataContext;
        private readonly ScanConfiguration _scanConfiguration;

        internal SetScanner(
            ISetScanOperator @operator,
            ISerializer serializer,
            DataContext dataContext,
            ScanConfiguration scanConfiguration)
        {
            _operator = @operator;
            _serializer = serializer;
            _dataContext = dataContext;
            _scanConfiguration = scanConfiguration;
        }

        public ISetScanner Override(Func<ScanConfiguration, ScanConfiguration> configOverride)
        {
            var clone = new ScanConfiguration(_scanConfiguration);
            var overriddenConfig = configOverride(clone);
            return new SetScanner(_operator, _serializer, _dataContext, overriddenConfig);
        }

        public void ScanSet(Action<string> recordFoundOperation) => ScanSet(Array.Empty<string>(), (key, record) => recordFoundOperation(key?.userKey?.ToString()));

        public void ScanSet<T1>(Action<KeyValuePair<string, T1>> recordFoundOperation, string bin) => ScanSet(new[] { bin }, (key, record) => recordFoundOperation(MapRecordToTuple<T1>(key, record, bin)));

        public void ScanSet<T1, T2>(Action<(string Key, T1 Value1, T2 Value2)> recordFoundOperation, string bin1, string bin2) => ScanSet(new[] { bin1, bin2 }, (key, record) => recordFoundOperation(MapRecordToTuple<T1, T2>(key, record, bin1, bin2)));

        public void ScanSet<T1, T2, T3>(Action<(string Key, T1 Value1, T2 Value2, T3 Value3)> recordFoundOperation, string bin1, string bin2, string bin3) => ScanSet(new[] { bin1, bin2, bin3 }, (key, record) => recordFoundOperation(MapRecordToTuple<T1, T2, T3>(key, record,  bin1, bin2, bin3)));

        private KeyValuePair<string, T> MapRecordToTuple<T>(Key key, Record record, string bin)
        {
            var result = new KeyValuePair<string, T>(key?.userKey?.ToString(), BlobParser.Parse<T>(_serializer, record, bin));
            return result;
        }

        private (string Key, T1 Value1, T2 Value2) MapRecordToTuple<T1, T2>(Key key, Record record, string bin1, string bin2)
        {
            var value1 = BlobParser.Parse<T1>(_serializer, record, bin1);
            var value2 = BlobParser.Parse<T2>(_serializer, record, bin2);
            return (key?.userKey?.ToString(), value1, value2);
        }

        private (string Key, T1 Value1, T2 Value2, T3 Value3) MapRecordToTuple<T1, T2, T3>(Key key, Record record, string bin1, string bin2, string bin3)
        {
            var value1 = BlobParser.Parse<T1>(_serializer, record, bin1);
            var value2 = BlobParser.Parse<T2>(_serializer, record, bin2);
            var value3 = BlobParser.Parse<T3>(_serializer, record, bin3);
            return (key?.userKey?.ToString(), value1, value2, value3);
        }

        private void ScanSet(IEnumerable<string> bins, Action<Key, Record> operation) => _operator.ScanSet(bins, _dataContext, _scanConfiguration, operation);
    }

    /// <inheritdoc cref="ISetScanner{T}"/>
    internal class SetScanner<T> : ISetScanner<T>
    {
        private readonly ISetScanner _inner;
        private readonly ScanContext _context;

        internal SetScanner(
            ISetScanner inner,
            ScanContext context)
        {
            _inner = inner;
            _context = context;
        }

        public ISetScanner<T> Override(Func<ScanConfiguration, ScanConfiguration> configOverride)
        {
            var overridden = _inner.Override(configOverride);
            return new SetScanner<T>(overridden, _context);
        }

        public void ScanSet(Action<KeyValuePair<string, T>> recordFoundOperation) => _inner.ScanSet(recordFoundOperation, _context.Bins[0]);
    }

    /// <inheritdoc cref="ISetScanner{T1,T2}"/>
    internal class SetScanner<T1, T2> : ISetScanner<T1, T2>
    {
        private readonly ISetScanner _inner;
        private readonly ScanContext _context;

        internal SetScanner(
            ISetScanner inner,
            ScanContext context)
        {
            _inner = inner;
            _context = context;
        }

        public ISetScanner<T1, T2> Override(Func<ScanConfiguration, ScanConfiguration> configOverride)
        {
            var overridden = _inner.Override(configOverride);
            return new SetScanner<T1, T2>(overridden, _context);
        }

        public void ScanSet(Action<(string Key, T1 Value1, T2 Value2)> recordFoundOperation) => _inner.ScanSet(recordFoundOperation, _context.Bins[0], _context.Bins[1]);
    }

    /// <inheritdoc cref="ISetScanner{T1,T2,T3}"/>
    internal class SetScanner<T1, T2, T3> : ISetScanner<T1, T2, T3>
    {
        private readonly ISetScanner _inner;
        private readonly ScanContext _context;

        internal SetScanner(
            ISetScanner inner,
            ScanContext context)
        {
            _inner = inner;
            _context = context;
        }

        public ISetScanner<T1, T2, T3> Override(Func<ScanConfiguration, ScanConfiguration> configOverride)
        {
            var overridden = _inner.Override(configOverride);
            return new SetScanner<T1, T2, T3>(overridden, _context);
        }

        public void ScanSet(Action<(string Key, T1 Value1, T2 Value2, T3 Value3)> recordFoundOperation) => _inner.ScanSet(recordFoundOperation, _context.Bins[0], _context.Bins[1], _context.Bins[2]);
    }
}
