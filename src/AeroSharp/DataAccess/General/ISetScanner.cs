using System;
using System.Collections.Generic;
using AeroSharp.DataAccess.Configuration;

namespace AeroSharp.DataAccess.General
{
    /// <summary>
    /// Provides an interface for scanning a set and doing something about scanned records for arbitrary types and bins.
    /// </summary>
    public interface ISetScanner : IOverridable<ISetScanner, ScanConfiguration>
    {
        /// <summary>
        /// Synchronously scan a set and perform an operation on a found key.
        /// </summary>
        /// <param name="recordFoundOperation">The operation to perform.</param>
        void ScanSet(Action<string> recordFoundOperation);
        /// <summary>
        /// Synchronously scan a set and perform an operation on a found key-value pair.
        /// </summary>
        /// <typeparam name="T1">The type of the value retrieved in the bin.</typeparam>
        /// <param name="recordFoundOperation">The operation to perform.</param>
        /// <param name="bin">The bin to retrieve the value from.</param>
        void ScanSet<T1>(Action<KeyValuePair<string, T1>> recordFoundOperation, string bin);
        /// <summary>
        /// Synchronously scan a set and perform an operation on a found key value-set.
        /// </summary>
        /// <typeparam name="T1">The type of the value retrieved from bin1.</typeparam>
        /// <typeparam name="T2">The type of the value retrieved from bin2.</typeparam>
        /// <param name="recordFoundOperation">The operation to perform.</param>
        /// <param name="bin1">The bin to retrieve the first typed value from.</param>
        /// <param name="bin2">The bin to retrieve the second typed value from.</param>
        void ScanSet<T1, T2>(Action<(string Key, T1 Value1, T2 Value2)> recordFoundOperation, string bin1, string bin2);
        /// <summary>
        /// Synchronously scan a set and perform an operation on a found key value-set.
        /// </summary>
        /// <typeparam name="T1">The type of the value retrieved from bin1.</typeparam>
        /// <typeparam name="T2">The type of the value retrieved from bin2.</typeparam>
        /// <typeparam name="T3">The type of the value retrieved from bin3.</typeparam>
        /// <param name="recordFoundOperation">The operation to perform.</param>
        /// <param name="bin1">The bin to retrieve the first typed value from.</param>
        /// <param name="bin2">The bin to retrieve the second typed value from.</param>
        /// <param name="bin3">The bin to retrieve the third typed value from.</param>
        void ScanSet<T1, T2, T3>(Action<(string Key, T1 Value1, T2 Value2, T3 Value3)> recordFoundOperation, string bin1, string bin2, string bin3);
    }

    /// <summary>
    /// Provides an interface for scanning a set comprising of a single type in the default bin.
    /// </summary>
    /// <typeparam name="T">The type of the record in the set.</typeparam>
    public interface ISetScanner<T> : IOverridable<ISetScanner<T>, ScanConfiguration>
    {
        /// <summary>
        /// Synchronously scan a set and perform an operation on the the found key-value pair.
        /// </summary>
        /// <param name="recordFoundOperation">The operation to perform.</param>
        void ScanSet(Action<KeyValuePair<string, T>> recordFoundOperation);
    }

    /// <summary>
    /// Provides an interface for scanning a set comprising of two types in their corresponding default bins.
    /// </summary>
    /// <typeparam name="T1">The first type in the first default bin.</typeparam>
    /// <typeparam name="T2">The second type in the second default bin.</typeparam>
    public interface ISetScanner<T1, T2> : IOverridable<ISetScanner<T1, T2>, ScanConfiguration>
    {
        /// <summary>
        /// Synchronously scan a set and perform an operation on the found key value-set.
        /// </summary>
        /// <param name="recordFoundOperation">The operation to perform.</param>
        void ScanSet(Action<(string Key, T1 Value1, T2 Value2)> recordFoundOperation);
    }

    /// <summary>
    /// Provides an interface for scanning a set comprising of three types in their corresponding default bins.
    /// </summary>
    /// <typeparam name="T1">The first type in the first default bin.</typeparam>
    /// <typeparam name="T2">The second type in the second default bin.</typeparam>
    /// <typeparam name="T3">The third type in the third default bin.</typeparam>
    public interface ISetScanner<T1, T2, T3> : IOverridable<ISetScanner<T1, T2, T3>, ScanConfiguration>
    {
        /// <summary>
        /// Synchronously scan a set and perform an operation on the found key value-set.
        /// </summary>
        /// <param name="recordFoundOperation">The operation to perform.</param>
        void ScanSet(Action<(string Key, T1 Value1, T2 Value2, T3 Value3)> recordFoundOperation);
    }
}
