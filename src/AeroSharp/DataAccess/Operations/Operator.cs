using AeroSharp.Connection;
using AeroSharp.DataAccess.Internal;
using AeroSharp.Serialization;

namespace AeroSharp.DataAccess.Operations
{
    internal class Operator : IOperator
    {
        private DataContext _dataAccessConfiguration;
        private WriteConfiguration _writeConfiguration;
        private ISerializer _serializer;
        private IClientProvider _clientProvider;

        internal Operator(
            DataContext dataAccessConfiguration,
            WriteConfiguration writeConfiguration,
            ISerializer serializer,
            IClientProvider clientProvider)
        {
            _dataAccessConfiguration = dataAccessConfiguration;
            _writeConfiguration = writeConfiguration;
            _serializer = serializer;
            _clientProvider = clientProvider;
        }

        public IOperationBuilder Key(string key)
        {
            var recordOperator = new RecordOperator(_clientProvider, _dataAccessConfiguration);
            return new OperationBuilder(_serializer, recordOperator, _writeConfiguration, key);
        }
    }
}
