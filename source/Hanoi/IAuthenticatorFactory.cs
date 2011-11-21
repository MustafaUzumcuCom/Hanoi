using System.Collections.Generic;
using Hanoi.Authentication;

namespace Hanoi
{
    public interface IAuthenticatorFactory
    {
        Authenticator Create(IList<string> features, Connection connection);
    }
}