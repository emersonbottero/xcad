﻿//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.SwDocumentManager.Geometry
{
    public interface ISwDmBody : IXBody, ISwDmObject
    {
    }

    internal abstract class SwDmBody : SwDmSelObject, ISwDmBody
    {
        public string Name => throw new NotSupportedException();

        public bool Visible { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public Color? Color { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public IEnumerable<IXFace> Faces => throw new NotSupportedException();

        public SwDmBody() : base(null)
        {
        }

        public IXBody Add(IXBody other) => throw new NotSupportedException();
        public IXBody[] Common(IXBody other) => throw new NotSupportedException();
        public IXBody[] Substract(IXBody other) => throw new NotSupportedException();
    }
}
