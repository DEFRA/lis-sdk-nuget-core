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
    private const int MaxLength = (sizeof(ulong) * 8) / BitsPerCharacter;
    private const char Separator = '-';

    extension(Guid source)
    {
        /// <summary>
        /// Generate a display hash for a guid.
        /// </summary>
        /// <param name="length">The number of hash characters to generate, from 0 to 12. Default is 9.</param>
        /// <param name="charactersPerGroup">The number of characters per group. Must be positive. Default is 3.</param>
        /// <returns>The string hash.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is negative or greater than 12, or <paramref name="charactersPerGroup"/> is not positive.
        /// </exception>
        public string CreateDisplayHash(int length = 9, int charactersPerGroup = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(length);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(length, MaxLength);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(charactersPerGroup);

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(source.ToString()));
            var bits = BinaryPrimitives.ReadUInt64BigEndian(hash);

            var id = new StringBuilder(length + (Math.Max(length - 1, 0) / charactersPerGroup));
            for (var index = 0; index < length; index++)
            {
                if (index > 0 && index % charactersPerGroup == 0)
                {
                    id.Append(Separator);
                }

                var shift = (sizeof(ulong) * 8) - (BitsPerCharacter * (index + 1));
                id.Append(Alphabet[(int)((bits >> shift) & 0b11111)]);
            }

            return id.ToString();
        }
    }
}
