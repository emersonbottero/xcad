﻿//*********************************************************************
//xCAD
//Copyright(C) 2021 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.SolidWorks.Geometry
{
    public interface ISwVertex : ISwEntity, IXVertex
    {
        IVertex Vertex { get; }
    }

    internal class SwVertex : SwEntity, ISwVertex
    {
        public IVertex Vertex { get; }

        public Point Coordinate => new Point((double[])Vertex.GetPoint());

        public override ISwBody Body => SwObject.FromDispatch<ISwBody>(
            ((Vertex.GetEdges() as object[]).First() as IEdge).GetBody(), m_Doc);

        public override IEnumerable<ISwEntity> AdjacentEntities
        {
            get
            {
                foreach (IEdge edge in (Vertex.GetEdges() as object[]).ValueOrEmpty())
                {
                    yield return FromDispatch<SwEdge>(edge, m_Doc);
                }

                foreach (IFace2 face in (Vertex.GetAdjacentFaces() as object[]).ValueOrEmpty())
                {
                    yield return FromDispatch<SwFace>(face, m_Doc);
                }
            }
        }

        internal SwVertex(IVertex vertex, ISwDocument doc) : base((IEntity)vertex, doc)
        {
            Vertex = vertex;
        }
    }
}
