using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace Enterprises.Framework.WindowsServer
{
    [RunInstaller(true)]
    public partial class WindowServiceInstaller : System.Configuration.Install.Installer
    {
        public WindowServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
