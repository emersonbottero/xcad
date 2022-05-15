﻿//*********************************************************************
//xCAD
//Copyright(C) 2022 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Geometry.Curves
{
    public interface ISwPoint : ISwObject, IXPoint
    {
    }

    internal class SwPoint : SwObject, ISwPoint
    {
        internal SwPoint(object disp, ISwDocument doc, ISwApplication app) : base(disp, doc, app)
        {
        }

        public Point Coordinate { get; set; }

        public bool IsCommitted => true;

        public void Commit(CancellationToken cancellationToken)
        {
        }
    }
}
