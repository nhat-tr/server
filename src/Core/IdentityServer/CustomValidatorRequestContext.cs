﻿using Bit.Core.Entities;
using Bit.Core.Models.Business;

namespace Bit.Core.IdentityServer;

public class CustomValidatorRequestContext
{
    public User User { get; set; }
    public bool KnownDevice { get; set; }
    public CaptchaResponse CaptchaResponse { get; set; }
}
