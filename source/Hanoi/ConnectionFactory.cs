using Hanoi.Transports;

namespace Hanoi
{
    public class ConnectionFactory : IConnectionFactory
    {
        private static IConnectionFactory _default;
        public static IConnectionFactory Default
        {
            get { return _default ?? (_default = new ConnectionFactory()); }
        }

        public static void SetDefault(IConnectionFactory factory)
        {
            _default = factory;
        }

        public ITransport Create(ConnectionString connection)
        {
            if (connection.UseHttpBinding)
            {
                return new HttpTransport();
            }
            return new TcpTransport();
        }
    }

    public interface IConnectionFactory
    {
        ITransport Create(ConnectionString connection);
    }
}
