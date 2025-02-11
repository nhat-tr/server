﻿using Bit.Core.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Bit.Admin.Controllers;

public class InfoController : Controller
{
    [HttpGet("~/alive")]
    [HttpGet("~/now")]
    public DateTime GetAlive()
    {
        return DateTime.UtcNow;
    }

    [HttpGet("~/version")]
    public JsonResult GetVersion()
    {
        return Json(CoreHelpers.GetVersion());
    }
}
