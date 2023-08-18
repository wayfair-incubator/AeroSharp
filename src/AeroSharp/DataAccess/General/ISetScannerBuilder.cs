using AeroSharp.DataAccess.Configuration;

namespace AeroSharp.DataAccess.General
{
    /// <summary>
    /// Provides an interface for building a set scanner.
    /// </summary>
    public interface ISetScannerBuilder : ICompressorBuilder<ISetScannerBuilder>
    {
        /// <summary>
        /// Builds a <see cref="ISetScanner" /> to scan any set reading from any or no bins.
        /// </summary>
        /// <returns>A freshly-built <see cref="ISetScanner" />.</returns>
        ISetScanner Build();

        /// <summary>
        /// Builds a <see cref="ISetScanner{T}" /> to scan any set reading from the default first bin whose type is
        /// <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of the value inside the default bin.</typeparam>
        /// <returns>A freshly-built <see cref="ISetScanner{T}" />.</returns>
        ISetScanner<T> Build<T>();

        /// <summary>
        /// Builds a <see cref="ISetScanner{T}" /> to scan any set reading from the specified bin whose type is
        /// <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of the value inside the specified bin.</typeparam>
        /// <param name="bin">The name of the bin to read from.</param>
        /// <returns>A freshly-built <see cref="ISetScanner{T}" />.</returns>
        ISetScanner<T> Build<T>(string bin);

        /// <summary>
        /// Builds a <see cref="ISetScanner{T1,T2}" /> to scan any set reading from the default first two bins whose types are
        /// <typeparamref name="T1" /> and <typeparamref name="T2" /> respectively.
        /// </summary>
        /// <typeparam name="T1">The type of the value in the first default bin.</typeparam>
        /// <typeparam name="T2">The type of the value in the second default bin.</typeparam>
        /// <returns>A freshly-built <see cref="ISetScanner{T1,T2}" />.</returns>
        ISetScanner<T1, T2> Build<T1, T2>();

        /// <summary>
        /// Builds a <see cref="ISetScanner{T1,T2}" /> to scan any set reading from the specified bins whose types are
        /// <typeparamref name="T1" /> and <typeparamref name="T2" /> respectively.
        /// </summary>
        /// <typeparam name="T1">The type of the value inside the first specified bin.</typeparam>
        /// <typeparam name="T2">The type of the value inside the second specified bin.</typeparam>
        /// <param name="bin1">The name of the first bin to read from.</param>
        /// <param name="bin2">The name of the second bin to read from.</param>
        /// <returns>A freshly-built <see cref="ISetScanner{T1,T2}" />.</returns>
        ISetScanner<T1, T2> Build<T1, T2>(string bin1, string bin2);

        /// <summary>
        /// Builds a <see cref="ISetScanner{T1,T2,T3}" /> to scan any set reading from the default first three bins whose types are
        /// <typeparamref name="T1" />, <typeparamref name="T2" />, and <typeparamref name="T3" /> respectively.
        /// </summary>
        /// <typeparam name="T1">The type of the value in the first default bin.</typeparam>
        /// <typeparam name="T2">The type of the value in the second default bin.</typeparam>
        /// <typeparam name="T3">The type of the value in the third default bin.</typeparam>
        /// <returns>A freshly-built <see cref="ISetScanner{T1,T2,T3}" />.</returns>
        ISetScanner<T1, T2, T3> Build<T1, T2, T3>();

        /// <summary>
        /// Builds a <see cref="ISetScanner{T1,T2,T3}" /> to scan any set reading from the specified bins whose types are
        /// <typeparamref name="T1" />, <typeparamref name="T2" />, and <typeparamref name="T3" /> respectively.
        /// </summary>
        /// <typeparam name="T1">The type of the value in the first specified bin.</typeparam>
        /// <typeparam name="T2">The type of the value in the second specified bin.</typeparam>
        /// <typeparam name="T3">The type of the value in the third specified bin.</typeparam>
        /// <param name="bin1">The name of the first bin to read from.</param>
        /// <param name="bin2">The name of the second bin to read from.</param>
        /// <param name="bin3">The name of the third bin to read from.</param>
        /// <returns>A freshly-built <see cref="ISetScanner{T1,T2,T3}" />.</returns>
        ISetScanner<T1, T2, T3> Build<T1, T2, T3>(string bin1, string bin2, string bin3);

        /// <summary>
        /// Optional: Provide a <see cref="ScanConfiguration" /> with different settings than the default.
        /// </summary>
        /// <param name="scanConfiguration">The new settings.</param>
        /// <returns>The instance of this builder, with the new configuration loaded.</returns>
        ISetScannerBuilder WithScanConfiguration(ScanConfiguration scanConfiguration);
    }
}