﻿//*********************************************************************
//xCAD
//Copyright(C) 2022 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Xarial.XCad.Exceptions;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Sketch;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Features;
using Xarial.XCad.UI;

namespace Xarial.XCad.SolidWorks.Sketch
{
    public interface ISwSketchPicture : IXSketchPicture, ISwSketchEntity, ISwFeature
    {
        /// <summary>
        /// Pointer to the sketch picture
        /// </summary>
        ISketchPicture SketchPicture { get; }
    }

    internal class SwSketchPicture : SwFeature, ISwSketchPicture
    {
        public ISketchPicture SketchPicture { get; private set; }

        internal SwSketchPicture(IFeature feat, SwDocument doc, SwApplication app, bool created) : base(feat, doc, app, created)
        {
            if (feat != null)
            {
                SketchPicture = feat.GetSpecificFeature2() as ISketchPicture;
            }
        }

        public override object Dispatch => SketchPicture;

        public IXImage Image 
        {
            get 
            {
                if (IsCommitted)
                {
                    throw new NotSupportedException();
                }
                else 
                {
                    return m_Creator.CachedProperties.Get<IXImage>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    throw new CommitedElementReadOnlyParameterException();
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        public Rect2D Boundary
        {
            get
            {
                if (IsCommitted)
                {
                    double width = -1;
                    double height = -1;
                    double x = -1;
                    double y = -1;

                    SketchPicture.GetSize(ref width, ref height);
                    SketchPicture.GetOrigin(ref x, ref y);
                    var angle = SketchPicture.Angle;

                    var transform = TransformMatrix.CreateFromRotationAroundAxis(new Vector(0, 0, 1), angle, new Point(0, 0, 0));

                    var dirX = new Vector(1, 0, 0).Transform(transform);
                    var dirY = new Vector(0, 1, 0).Transform(transform);

                    return new Rect2D(width, height, new Point(x + width / 2, y + height / 2, 0), dirX, dirY);
                }
                else
                {
                    return m_Creator.CachedProperties.Get<Rect2D>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    throw new CommitedElementReadOnlyParameterException();
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        public IXSketchBase OwnerSketch => throw new NotImplementedException();
        public IXSketchBlockInstance OwnerBlock => throw new NotImplementedException();

        protected override IFeature InsertFeature(CancellationToken cancellationToken)
        {
            if (Image == null) 
            {
                throw new Exception("Image is not specified");
            }

            if (Boundary == null) 
            {
                throw new Exception("Boundary of the image is not specified");
            }

            var tempFileName = "";

            try
            {
                tempFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");

                File.WriteAllBytes(tempFileName, Image.Buffer);

                var pict = OwnerDocument.Model.SketchManager.InsertSketchPicture(tempFileName);

                if (pict != null)
                {
                    var orig = new Point(Boundary.CenterPoint.X - Boundary.Width / 2, Boundary.CenterPoint.Y - Boundary.Height / 2, 0);

                    pict.SetOrigin(orig.X, orig.Y);
                    pict.SetSize(Boundary.Width, Boundary.Height, false);

                    var angle = Boundary.AxisX.GetAngle(new Vector(1, 0, 0));

                    //picture PMPage stays open after inserting the picture
                    const int swCommands_PmOK = -2;
                    OwnerApplication.Sw.RunCommand(swCommands_PmOK, "");

                    SketchPicture = pict;

                    return pict.GetFeature();
                }
                else
                {
                    throw new Exception("Failed to insert picture");
                }
            }
            finally
            {
                if (File.Exists(tempFileName))
                {
                    try
                    {
                        File.Delete(tempFileName);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
