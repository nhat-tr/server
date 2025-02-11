﻿using Microsoft.AspNetCore.DataProtection;

namespace Bit.Core.Tokens;

public class Token
{
    private readonly string _token;

    public Token(string token)
    {
        _token = token;
    }

    public Token WithPrefix(string prefix)
    {
        return new Token($"{prefix}{_token}");
    }

    public Token RemovePrefix(string expectedPrefix)
    {
        if (!_token.StartsWith(expectedPrefix))
        {
            throw new BadTokenException($"Expected prefix, {expectedPrefix}, was not present.");
        }

        return new Token(_token[expectedPrefix.Length..]);
    }

    public Token ProtectWith(IDataProtector dataProtector) =>
        new(dataProtector.Protect(ToString()));

    public Token UnprotectWith(IDataProtector dataProtector) =>
        new(dataProtector.Unprotect(ToString()));

    public override string ToString() => _token;
}
