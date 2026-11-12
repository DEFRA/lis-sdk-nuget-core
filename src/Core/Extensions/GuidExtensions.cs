// <copyright file="GuidExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Livestock.Sdk.Core.Extensions;

using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Guid extension functions.
/// </summary>
public static class GuidExtensions
{
    private const string Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
    private const int BitsPerCharacter = 5;
    private const char Separator = '-';

    extension(Guid source)
    {
        /// <summary>
        /// Generate a display hash for a guid.
        /// </summary>
        /// <param name="length">The length to generate. Default is 9.</param>
        /// <param name="charactersPerGroup">The number of characters per group. Default is 3.</param>
        /// <param name="crockfordReplacement">Replace characters that can be mistaken. True by default.</param>
        /// <returns>The string hash.</returns>
        public string CreateDisplayHash(int length = 9, int charactersPerGroup = 3, bool crockfordReplacement = true)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(source.ToString()));
            var bits = BinaryPrimitives.ReadUInt64BigEndian(hash);

            var id = new StringBuilder(length + 2);
            for (var index = 0; index < length; index++)
            {
                if (index > 0 && index % charactersPerGroup == 0)
                {
                    id.Append(Separator);
                }

                var shift = (sizeof(ulong) * 8) - (BitsPerCharacter * (index + 1));
                id.Append(Alphabet[(int)((bits >> shift) & 0b11111)]);
            }

            var result = id.ToString().Trim().ToUpperInvariant();
            return crockfordReplacement
                ? result
                    .Replace('O', '0')
                    .Replace('I', '1')
                    .Replace('L', '1')
                : result;
        }
    }
}
