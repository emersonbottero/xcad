﻿using NUnit.Framework;
using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SwDocumentManager;
using Xarial.XCad.SwDocumentManager.Documents;

namespace SolidWorksDocMgr.Tests.Integration
{
    public class DocumentWrapper : IDisposable
    {
        private readonly ISwDmApplication m_App;
        public ISwDmDocument Document { get; }

        private bool m_IsDisposed;

        internal DocumentWrapper(ISwDmApplication app, ISwDmDocument model)
        {
            m_App = app;
            Document = model;
            m_IsDisposed = false;
        }

        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                Document.Close();
                m_IsDisposed = true;
            }
        }
    }

    [TestFixture]
    [RequiresThread(System.Threading.ApartmentState.STA)]
    public abstract class IntegrationTests
    {
        private const string DATA_FOLDER = @"C:\Users\artem\OneDrive\xCAD\TestData";

        protected ISwDmApplication m_App;
        private ISwDMApplication m_SwDmApp;

        private List<IDisposable> m_Disposables;

        [OneTimeSetUp]
        public void Setup()
        {
            var dmKey = Environment.GetEnvironmentVariable("SW_DM_KEY", EnvironmentVariableTarget.Machine);

            m_App = SwDmApplicationFactory.Create(dmKey);
            m_SwDmApp = m_App.SwDocMgr;

            m_Disposables = new List<IDisposable>();
        }

        protected string GetFilePath(string name)
        {
            string filePath;

            if (Path.IsPathRooted(name))
            {
                filePath = name;
            }
            else
            {
                filePath = Path.Combine(DATA_FOLDER, name);
            }

            return filePath;
        }

        protected void UpdateSwReferences(string destPath, params string[] assmRelPaths)
        {
            Process prc;

            using (var app = SwApplicationFactory.Create(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2019,
                            Xarial.XCad.Enums.ApplicationState_e.Background
                            | Xarial.XCad.Enums.ApplicationState_e.Silent
                            | Xarial.XCad.Enums.ApplicationState_e.Safe))
            {
                prc = app.Process;

                foreach (var assmPath in assmRelPaths)
                {
                    using (var doc = (ISwDocument)app.Documents.Open(Path.Combine(destPath, assmPath)))
                    {
                        doc.Model.ForceRebuild3(false);
                        doc.Save();
                        var deps = (doc.Model.Extension.GetDependencies(false, false, false, false, false) as string[]).Where((item, index) => index % 2 != 0).ToArray();

                        if (!deps.All(d => d.Contains("^") || d.StartsWith(destPath, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            throw new Exception("Failed to setup source assemblies");
                        }
                    }
                }

                app.Close();
            }

            prc.Kill();
        }

        protected void CopyDirectory(string srcPath, string destPath)
        {
            foreach (var srcFile in Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories))
            {
                var relPath = srcFile.Substring(srcPath.Length + 1);
                var destFilePath = Path.Combine(destPath, relPath);
                var destDir = Path.GetDirectoryName(destFilePath);

                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                File.Copy(srcFile, destFilePath);
            }
        }

        protected DocumentWrapper OpenDataDocument(string name, bool readOnly = true)
        {
            var filePath = GetFilePath(name);

            var doc = (ISwDmDocument)m_App.Documents.Open(filePath, 
                readOnly 
                ? Xarial.XCad.Documents.Enums.DocumentState_e.ReadOnly 
                : Xarial.XCad.Documents.Enums.DocumentState_e.Default);

            if (doc != null)
            {
                var docWrapper = new DocumentWrapper(m_App, doc);
                m_Disposables.Add(docWrapper);
                return docWrapper;
            }
            else
            {
                throw new NullReferenceException($"Failed to open the the data document at '{filePath}'");
            }
        }
        
        [TearDown]
        public void TearDown()
        {
            foreach (var disp in m_Disposables)
            {
                try
                {
                    disp.Dispose();
                }
                catch
                {
                }
            }

            m_Disposables.Clear();

            while (m_App.Documents.Any())
            {
                m_App.Documents.First().Close();
            }
        }

        [OneTimeTearDown]
        public void FinalTearDown()
        {
        }
    }
}
