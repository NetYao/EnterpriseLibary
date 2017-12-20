using System;
using System.Configuration;
using System.ServiceModel;
using Enterprises.Framework.Utility;

namespace Enterprises.Test.Bussiness.ServiceClient
{
    class ServiceAgentConfig
    {
        internal static System.ServiceModel.Channels.Binding Binding
        {
            get
            {
                NetTcpBinding binding = new NetTcpBinding();
                binding.CloseTimeout = new TimeSpan(0, 1, 0);
                binding.OpenTimeout = new TimeSpan(0, 1, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                binding.SendTimeout = new TimeSpan(0, 1, 0);
                binding.TransactionFlow = false;
                binding.TransferMode = TransferMode.Buffered;
                binding.TransactionProtocol = TransactionProtocol.OleTransactions;
                binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                binding.ListenBacklog = 10;
                binding.MaxBufferPoolSize = 524288;
                binding.MaxBufferSize = 65536;
                binding.MaxConnections = 600;// 10;
                binding.MaxReceivedMessageSize = 65536;
                binding.ReaderQuotas.MaxDepth = 32;
                binding.ReaderQuotas.MaxStringContentLength = 8192;
                binding.ReaderQuotas.MaxArrayLength = 1638400;//原始大小16384
                binding.ReaderQuotas.MaxBytesPerRead = 4096;
                binding.ReaderQuotas.MaxNameTableCharCount = 16384;
                binding.ReliableSession.Ordered = true;
                binding.ReliableSession.InactivityTimeout = new TimeSpan(0, 0, 10);//new TimeSpan(0, 10, 0);
                binding.ReliableSession.Enabled = false;
                binding.Security.Mode = SecurityMode.None;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Message.ClientCredentialType = MessageCredentialType.None;

                return binding;
            }
        }
    }

    /// <summary>
    /// 测试wcf 在宿主在window service下面 net:tcp 调用.
    /// </summary>
    public class ServiceAgent : ClientBase<IWcfTcpTestService>, IWcfTcpTestService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ServiceAgent Create()
        {
            string _endpointName = "";
            try
            {
                _endpointName = ConfigurationManager.AppSettings["WcfClient:WorkflowServiceEndpoint"];
                if (!string.IsNullOrEmpty(_endpointName))
                {
                    return new ServiceAgent(_endpointName);
                }
            }
            catch (Exception x)
            {
                LogHelper.WriteLog("ServiceAgent构造失败，终结点：" + _endpointName);
            }

            string _endpointUrl = "";
            try
            {
                _endpointUrl = ConfigurationManager.AppSettings["WcfClient:WorkflowServiceEndpointAddress"];
                if (!string.IsNullOrEmpty(_endpointUrl))
                {
                    return new ServiceAgent(ServiceAgentConfig.Binding, new EndpointAddress(_endpointUrl));
                }
            }
            catch (Exception x2)
            {
                LogHelper.WriteLog("ServiceAgent构造失败，终结点地址：" + _endpointUrl);
                throw  x2;
            }

            return null;
        }

        internal ServiceAgent(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        internal ServiceAgent(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        internal ServiceAgent(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        internal ServiceAgent(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }


        public int Jiafa(int a, int b)
        {
            return Channel.Jiafa(a,b);
        }

        public int Jianfa(int a, int b)
        {
            return Channel.Jianfa(a, b);
        }
    }

}
