// <copyright file="StringExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Livestock.Sdk.Core.Extensions;

using System.Diagnostics.CodeAnalysis;
using System.Text;

/// <summary>
/// Common string extensions.
/// </summary>
public static class StringExtensions
{
    extension(string? source)
    {
        /// <summary>
        /// convert string to snake case.
        /// </summary>
        /// <returns>string converted to snake case, or the input unchanged when it is null or empty.</returns>
        [return: NotNullIfNotNull(nameof(source))]
        public string? ToSnakeCase()
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(char.ToLowerInvariant(source[0]));
            for (int index = 1; index < source.Length; ++index)
            {
                var c = source[index];
                if (char.IsUpper(c))
                {
                    stringBuilder.Append('_');
                    stringBuilder.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Generate a display hash for a string based guid.
        /// </summary>
        /// <param name="length">The number of hash characters to generate, from 0 to 12. Default is 9.</param>
        /// <param name="charactersPerGroup">The number of characters per group. Must be positive. Default is 3.</param>
        /// <returns>The string hash.</returns>
        /// <exception cref="ArgumentNullException">The source string is null.</exception>
        /// <exception cref="FormatException">The source string is not a valid GUID.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is negative or greater than 12, or <paramref name="charactersPerGroup"/> is not positive.
        /// </exception>
        public string CreateDisplayHash(int length = 9, int charactersPerGroup = 3)
        {
            ArgumentNullException.ThrowIfNull(source);

            return Guid.TryParse(source, out var guid)
                ? guid.CreateDisplayHash(length, charactersPerGroup)
                : throw new FormatException("Invalid GUID");
        }
    }
}
