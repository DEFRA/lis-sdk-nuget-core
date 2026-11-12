// <copyright file="StringExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Livestock.Sdk.Core.Extensions;

using System.Text;

/// <summary>
/// Common string extensions.
/// </summary>
public static class StringExtensions
{
    extension(string source)
    {
        /// <summary>
        /// convert string to snake case.
        /// </summary>
        /// <returns>string converted to snake case.</returns>
        public string ToSnakeCase()
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
        /// <param name="length">The length to generate. Default is 9.</param>
        /// <param name="charactersPerGroup">The number of characters per group. Default is 3.</param>
        /// <param name="crockfordReplacement">Replace characters that can be mistaken. True by default.</param>
        /// <returns>The string hash.</returns>
        public string CreateDisplayHash(int length = 9, int charactersPerGroup = 3, bool crockfordReplacement = true)
        {
            return !Guid.TryParse(source, out var guid)
                ? throw new InvalidCastException("Invalid GUID")
                : guid.CreateDisplayHash(length, charactersPerGroup, crockfordReplacement);
        }
    }
}
