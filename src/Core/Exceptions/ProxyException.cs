// <copyright file="RulesLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>
namespace Defra.Core.Exceptions;

public class ProxyException : Exception
{
    public ProxyException(string message)
        : base(message)
    {
    }
}