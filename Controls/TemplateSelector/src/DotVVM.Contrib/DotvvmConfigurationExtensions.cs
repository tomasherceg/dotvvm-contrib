﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;

namespace DotVVM.Contrib
{
    public static class DotvvmConfigurationExtensions
    {
        public static void AddContribTemplateSelectorConfiguration(this DotvvmConfiguration config)
        {
            // register tag prefix
            config.Markup.Controls.Add(new DotvvmControlConfiguration()
            {
                Assembly = "DotVVM.Contrib.TemplateSelector",
                Namespace = "DotVVM.Contrib",
                TagPrefix = "dc"
            });
        }

    }
}
