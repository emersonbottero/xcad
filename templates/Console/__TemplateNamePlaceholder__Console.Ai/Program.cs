﻿using __TemplateNamePlaceholder__Console.Base;
using System;
using Xarial.XCad.Inventor;
using Xarial.XCad.Inventor.Enums;

namespace __TemplateNamePlaceholder__Console.Sw
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var reader = new PropertiesReader(AiApplicationFactory.Create(AiVersion_e.Inventor2023), Console.Out)) 
            {
                foreach (var filePath in args) 
                {
                    reader.PrintProperties(filePath);
                }
            }
        }
    }
}
