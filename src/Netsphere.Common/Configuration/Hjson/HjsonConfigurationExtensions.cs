using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Netsphere.Common.Configuration.Hjson
{
    /// <summary>
    /// Extension methods for adding <see cref="HjsonConfigurationProvider" />.
    /// </summary>
    public static class HjsonConfigurationExtensions
    {
        /// <summary>
        /// Adds the HJSON configuration provider at <paramref name="path" /> to <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" /> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="P:Microsoft.Extensions.Configuration.IConfigurationBuilder.Properties" /> of <paramref name="builder" />.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddHjsonFile(this IConfigurationBuilder builder, string path)
        {
            return builder.AddHjsonFile(null, path, false, false);
        }

        /// <summary>
        /// Adds the HJSON configuration provider at <paramref name="path" /> to <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" /> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="P:Microsoft.Extensions.Configuration.IConfigurationBuilder.Properties" /> of <paramref name="builder" />.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddHjsonFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return builder.AddHjsonFile(null, path, optional, false);
        }

        /// <summary>
        /// Adds the HJSON configuration provider at <paramref name="path" /> to <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" /> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="P:Microsoft.Extensions.Configuration.IConfigurationBuilder.Properties" /> of <paramref name="builder" />.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddHjsonFile(this IConfigurationBuilder builder, string path, bool optional,
            bool reloadOnChange)
        {
            return builder.AddHjsonFile(null, path, optional, reloadOnChange);
        }

        /// <summary>
        /// Adds a HJSON configuration source to <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" /> to add to.</param>
        /// <param name="provider">The <see cref="T:Microsoft.Extensions.FileProviders.IFileProvider" /> to use to access the file.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="P:Microsoft.Extensions.Configuration.IConfigurationBuilder.Properties" /> of <paramref name="builder" />.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddHjsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path,
            bool optional, bool reloadOnChange)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException(nameof(path));

            var configurationSource = new HjsonConfigurationSource
            {
                FileProvider = provider,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };
            configurationSource.ResolveFileProvider();
            builder.Add(configurationSource);
            return builder;
        }
    }
}
