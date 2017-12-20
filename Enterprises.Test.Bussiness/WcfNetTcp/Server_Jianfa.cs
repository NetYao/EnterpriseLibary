namespace Enterprises.Test.Bussiness.ServiceClient
{
    public partial class WcfTcpTestService : IWcfTcpTestService
    {
        public int Jianfa(int a, int b)
        {
            return a - b;
        }
    }
}
