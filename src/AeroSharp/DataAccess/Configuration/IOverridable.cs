using System;

namespace AeroSharp.DataAccess.Configuration
{
    public interface IOverridable<TDataAccessObject, TConfiguration>
    {
        TDataAccessObject Override(Func<TConfiguration, TConfiguration> configOverride);
    }
}
