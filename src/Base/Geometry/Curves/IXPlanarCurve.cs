﻿using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry.Curves
{
    public interface IXPlanarCurve
    {
        Plane Plane { get; }
    }
}
