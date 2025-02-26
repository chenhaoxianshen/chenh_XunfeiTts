using chenhXunfeiTts;

namespace chenh_XunfeiTts
{
    public class Program
    {
        static void Main(string[] args)
        {
            string _appID = "368cce35";
            string _apiKey = "443041888d5ef7a388414d0534c8c776"; ;
            string _apiSecret = "NmU3MGVhZGRhNDUwYTg5YjVkNDA2ZGM4"; ;

            string _abilitys = "e2e44feff"; ;
            int _authType = 0;
            string _resDir = "./res"; ;
            string _workDir = "./"; ;
            string _text = "如磁盘操作、文件存取、目录操作、进程管理、文件权限设定等。";
            string fileName = "fileName";

            var model = new AikitBuildModel()
            {
                appID = "368cce35",
                apiKey = "443041888d5ef7a388414d0534c8c776",
                apiSecret = "NmU3MGVhZGRhNDUwYTg5YjVkNDA2ZGM4",
                ability = "e2e44feff",
                authType = 0,
                resDir = "./res",
                workDir = "./",
                text = "解放西，长沙人的弗洛里达",
                fileName = "xaaadas"

            }; 
            var sdk = new AIKITSdk(model);
            sdk.CreatWavSource();
             
            Console.ReadLine();
            Console.WriteLine("Hello, World!");
        }
    }
}
