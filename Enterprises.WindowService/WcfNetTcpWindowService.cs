using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using Enterprises.Test.Bussiness.ServiceClient;

namespace Enterprises.WindowService
{
    public partial class WcfNetTcpWindowService : ServiceBase
    {
        internal static ServiceHost myServiceHost = null; 
        public WcfNetTcpWindowService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (myServiceHost != null)
            {
                myServiceHost.Close();
            }
            myServiceHost = new ServiceHost(typeof(WcfTcpTestService));
            myServiceHost.Open();
        }

        protected override void OnStop()
        {
            if (myServiceHost != null)
            {
                myServiceHost.Close();
                myServiceHost = null;
            }
        }

        
    }
}
