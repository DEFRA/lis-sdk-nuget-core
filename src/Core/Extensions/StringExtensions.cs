// <copyright file="StringExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Lis.Core.Extensions;

using System.Text;

/// <summary>
/// Common string extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// convert string to snake case.
    /// </summary>
    /// <param name="text">text to convert.</param>
    /// <returns>string converted to snake case.</returns>
    public static string ToSnakeCase(this string text)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(char.ToLowerInvariant(text[0]));
        for (int index = 1; index < text.Length; ++index)
        {
            var c = text[index];
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
}
