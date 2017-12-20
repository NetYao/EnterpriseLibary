using System.ServiceModel;

namespace Enterprises.Test.Bussiness.ServiceClient
{
    /// <summary>
    /// 测试服务接口
    /// </summary>
    [ServiceContract]
   public interface IWcfTcpTestService
    {
        /// <summary>
        /// 加法运算
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [OperationContract]
        int Jiafa(int a,int b);

        /// <summary>
        /// 减法
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [OperationContract]
        int Jianfa(int a ,int b);
    }
}
