using System;

namespace Enterprises.Pay.AliPay
{
    public class AlipayConfig
    {
        

        /// <summary>
        /// APPID
        /// </summary>
        public static string AppId => "2016080600179658";
       
        // 商家私钥
        public static String MerchantPrivateKey => @"MIIEowIBAAKCAQEAnjkNn545pGA5nDEGKkIo2maNQZ928+6A6JdTwEVRcZBC0dEilI+3hHLb4HlOB+ipa1VTurqxPBWdsy+7QfonozjGcpo00JIVnZz6hOhIHTLhPS9eBupi1d4Oasd6nLC8Bij1BcPE369o1AoL9Vryguv5T/AIY/Tlgx3GEvHiceR8CcvTPmdeuA/Tr+tkzhJGw4Wmxg9p2ecCakHwnrBKcDlFQ7ohtL6xcz37nlosqQFO/ZgeB//OPuAC50UL1rQ0srQA+oK0xr3MNsAqRZ75ZWIiDGcAQkUed/2mv8spf+bcbu2FdKmVoDXKyoiAHM0WMXhAyvkTX/Lb1HdbsHDouQIDAQABAoIBAALCbfllsg2q2/ZcaIYeJ0Y/2aDWXiW8+CSWVxCYZOY9JN0x7491vjaiJjGRDApZRXMoxtAP531rxxdT8skgysYu/E8NX3Rvx8666SeeQXu44nLFu7cVKWofeFLmbb4D2xOkbTmPg4R/M63XCwRZdLdR1ERPpGXR5JrXzxePfmqMEZL0usfTyDW4FLdEG5c4Zam4/kIerZtj1kvmC0SaVURqesk2oNStmg65oi2ZZb7mweDHiU/pqFBe+ZHYlklHTG8Dvpl9BLAPaYPvBqVSQjs+mHF0TJKzF+uNkQS+Noi2Y4puNSGbV/Wr3en7w6vtxMPrfBOWgySMLuz59hPfovECgYEA0fWVD28w8RcoEtWQhnzEvEjMTCEPt5gCthyIHWqXrYcleZsUfoyqQ1qCRsHJgJ4XeSfxbhZ8c/7/1IT/RKhaUFf5LFn8reNXs/MH1gZn2gIWNtD7t4GWWJU8GVD+j8Vcd3Pal6edySbBbYbBfUkN1hwdaWpMV0y768DLn09O0iUCgYEAwOsotrG/Cvf3HCfjhf/Hob7jdxBLrxtE5nSe6TtaRAy+3Wvbms0M380ZkoqvuhX/1X2JjOc3mWc65zNtZDct1YCKif4BZWMKXH+fj/g80nPoxOv3fArkYzF8N5UL7CrEkgoi7dewvT2xYKhpsSVl9DoHv57OnWCiyUs9wOVXNgUCgYAKdPC3lBIb78tPJNPN3ujtd7K1F/BwdGGuMD2XOXfUbKaxJ8gdhWP1dsyGOaCPh1Aj+JlNolEQdeLH8tfD056r4bXHP7QV9PypObFqN2d0tCXnFQF0Yj/aqZQUrrP+9RVl3Z9FblOtcQCxM8TvOHQ39a6BUYn7zxaM50084fZLAQKBgGWZs4h8FgQd0ZuXemEw8x9BHCSxhVqEPwYr9yEYLBLYr5CHZFGjmhsntkcSGTgkq+bwxYdaolJ9Jm1rDTqRQOdHWi8QggW2YleSsyMsdkPT3YwQRfei+OMwxZ20NmI7p+jNw9WFGcSggBFKwNcVvhsgkMIfTeteYH0ozi4P2p1NAoGBAMiUjSJo6p858rvR+HZQOL/DHTSLcoxZQI6qkHDiXFCv13OiCMtYV/cnfdj6aDyJ01+TXB7OMdZjO1ykPmgiYS5qEuhMSTlA86qx8jYYaPOTcp8oFy6O/Ws4IFs1nc+sJnMtwSjQ4iUuRMQTrqFX9PTaPdiytN4XALbDlFEm4tFM";

        // 支付宝公钥,查看地址：https://openhome.alipay.com/platform/keyManage.htm 对应APPID下的支付宝公钥。非应用公腰
        public static String AlipayPublicKey => "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAr+8dQU1nZWXKY4ZPGrm2wmCx9buKL3jm2D49wXTwa4AdW1J04hhwzsf9of2rArGpCWKwrkx80683ZuW2a4fyTb33wQ4YaBwkpMnXsqxESKfPjzXhbZdO91USS+8Ks2UsPVEu4BmhqbQYnOzCowRLh5bMMoko9pKhDrNw5iYQG0ccp2wQ3cSIFw9MbXIa/RjJOOwWDybSVFbx+RFmPcw3Tp6nfVDESpMDKBS+WxQn0jaoJ1gwJ1FLzE/tMHyyWH6axejcEXf3+QqKC756CvjOpyJqt1OQzg5Er56+7oBOxT5vHX06nkvz4egEErMrmuT6Td7F4R7VDedBZqIYAchkjQIDAQAB";
        

        // 服务器异步通知页面路径  需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
        public static String NotifyUrl => "https://www.gingergo.cn/api/users/alipay/sign?";

        // 页面跳转同步通知页面路径 需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
        public static String ReturnUrl => "http://iot.hinets.net/Bussinessers/AliPay/Return.aspx";

        // 签名方式
        public static String SignType => "RSA2";



        // 支付宝网关
        public static String GatewayUrl => "https://openapi.alipaydev.com/gateway.do";//"https://openapi.alipay.com/gateway.do";
        /// <summary>
        /// 商户UID
        /// </summary>
        public static String SellerId=> "2088102170188194";
        /// <summary>
        /// 编码
        /// </summary>
        public static String Charset => "utf-8";

        /// <summary>
        /// 合作伙伴身份（PID）
        /// </summary>
        public static string Pid => "2088721334198064";

        /// <summary>
        /// MD5密钥
        /// </summary>
        public static string Md5Key => "rfsk2jtubdr60bzddmjd5qmgojjv1mip";

        /// <summary>
        /// 合作伙伴支付宝公钥
        /// </summary>
        public static string AliPayHzhbPublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";

        /// <summary>
        /// 合作伙伴商家宝密钥
        /// </summary>
        public static string AliPayHzhbPrivateKey = "MIICXQIBAAKBgQDULI7synf4wrb//J9ihgvT45PG7s4X7sWw2pwcPqy6RxHRHM0jtaaCCiTg0Imlo9MC9aXJJMXYCz9AcdrUgX8/zv5ZyFTIb2KAEW8TU7txND29ToUT5vTEeEF6e55CQdEo9JY6o7Cbj9N4uWamIyAd1u1M8/6tGsoNCbs82aFnZwIDAQABAoGABcq1mBcq0TqXfkNqImlgV0dmGE8ahyK4eMxu6IJ5ephIDzpHC0uBIRmfyhdHOqNPNkx4WxZK5EviTSMOgzCJxapqGqpV12ou2FccsQQDDpCxawWAPhEDSM3WJMtE7XMC9a9jWVwQ18F53hkEpwNnJ5IbinDKxJ87+T8buDTz/JkCQQDsti1UOaHP0XMDZ58zt0GtL6e0NsXKTaAXYaqjHcir8/S4XVnCUgk8tSpJw4M1qomV9DYR2elblBHxm4Hq44GTAkEA5XaGiqSPOTccg++uaGmzpD/HoGvYTosfEQilt1fgMcftEQCI0QcFnXCBQHN7ETWeYEuT5vqijFOIdklThUB3XQJBAJBWfclv4wUvuCwJUYBdbETIXEB9bZnwP4BCY6RXEvgXJ9ALzCG7mDpWVdTozOc6d+7iHN5BvBPFdDOZGV07INUCQQCt3N3ahQ+EYa64hU4YDScxZ6YhdjWCZcVXSb8OqYXVElAdFtOT1gc6ILm+lQGFq6PiAMIDPJsXSl/WiH7BM3R9AkBoSmsdwXPpAGY6rXPhxipaQ5xVhUo9vsFna1tRQh9Sn6U1uZzTO/arVg/d6TqlDvGspSyYpZregTN1ikTYD3N+";

    }

    public enum AliPaySignType
    {
        MD5,
        RSA2,
        RSA,
        DSA
    }

}
