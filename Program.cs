using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using chenh_XunfeiTts.Tools;
using chenh_XunfeiTts;

namespace chenh_XunfeiTts
{
    public class Program
    {


        public static ConcurrentQueue<(string startTime, string warningMsg)> WarningMusicDics = new ConcurrentQueue<(string Timestamp, string warningMsg)>();

        private static SemaphoreSlim Semaphore = new SemaphoreSlim(100); // 限制最大并发写入任务  
        static async Task Main(string[] args)
        {
            //using (UdpClient udpClient = new UdpClient())
            //{
            //    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Loopback, 5001); // 目标地址和端口

            //    while (true)
            //    {
            //        string message = $"Hello, UDP! 时间: {DateTime.Now:HH:mm:ss}";
            //        byte[] data = Encoding.UTF8.GetBytes(message);

            //        udpClient.Send(data, data.Length, remoteEndPoint);
            //        Console.WriteLine($"已发送消息: {message}");

            //        await Task.Delay(500); // 每隔 5 秒发送一次
            //    }
            //}

            //string _appID = "368cce35";
            //string _apiKey = "443041888d5ef7a388414d0534c8c776"; ;
            //string _apiSecret = "NmU3MGVhZGRhNDUwYTg5YjVkNDA2ZGM4"; ;

            //string _abilitys = "e2e44feff"; ;
            //int _authType = 0;
            //string _resDir = "./res"; ;
            //string _workDir = "./"; ;
            //string _text = "如磁盘操作、文件存取、目录操作、进程管理、文件权限设定等。";
            //string fileName = "fileName";

            var list=new List<string>();
            list.Add("阿拉尔场站");
            //list.Add("3512");
            //list.Add("新源风电场");
            //list.Add("开");
            //list.Add("关");

            //foreach (var item in list)
            //{
                var model = new AikitBuildModel()
                {
                    appID = "368cce35",
                    apiKey = "443041888d5ef7a388414d0534c8c776",
                    apiSecret = "NmU3MGVhZGRhNDUwYTg5YjVkNDA2ZGM4",
                    ability = "e2e44feff",
                    authType = 0,
                    resDir = "./res",
                    workDir = "./",
                    text = list[0],
                    filePath = @"E:\tttt\yp\" + list[0] + ".wav"
                };
                var sdk = new AIKITSdk(model);
                sdk.CreatWavSource();
            //}
             

            //IntPtr abilitys = IntPtr.Zero; // 假设已经初始化

            //// 创建多个 TTS 任务
            //List<Task> tasks = new List<Task>();
            //for (int i = 0; i < 5; i++) // 启动 5 个线程
            //{
            //    tasks.Add(Task.Run(() =>
            //    {
                  

            //        // 设置回调
            //        XunfeiSDK.SetCallback(Callbacks.OnEvent);
            //        XunfeiSDK.SetOutputCallback(Callbacks.OnOutput);

            //        // 执行 TTS
            //        ttsProcessor.WriteToSourceFile("output.pcm");
            //    }));
            //}

            //// 等待所有任务完成
            //Task.WaitAll(tasks.ToArray());
            //Console.WriteLine("所有 TTS 任务完成");



            // 启动第一个线程，每隔5秒执行一次
            //var task1 = Task.Run(() => ExecuteEvery5Seconds());

            //// 启动第二个线程，每隔1秒执行一次
            //var task2 = Task.Run(() => ExecuteEvery1Second());

            //// 等待两个任务结束
            //await Task.WhenAll(task1, task2); 

            Console.ReadLine();
            Console.WriteLine("Hello, World!");
        }

        static async Task ExecuteEvery5Seconds()
        {
            while (true)
            {
                // 执行任务
                var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                WarningMusicDics.Enqueue((time, "当前时间"+time)); 
                // 等待 5 秒
                await Task.Delay(2000);
            }
        }

        static async Task ExecuteEvery1Second()
        {
            while (true)
            {
                // 执行任务
                GenerateQueueList();

                // 等待 1 秒
                await Task.Delay(1000);
            }
        }


        public static async void GenerateQueueList()
        {
            try
            {
                List<Task> tasks = new List<Task>();
                foreach (var entry in WarningMusicDics.ToArray())
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        await Semaphore.WaitAsync();
                        try
                        {
                            var guid = Guid.NewGuid();
                            var fileName = guid + ".wav";
                            string audioFilePath = System.IO.Directory.GetCurrentDirectory() + "//Resources//" + fileName;
                            if (!File.Exists(audioFilePath))
                            {
                                GenerateWavFile(entry.warningMsg, audioFilePath);
                                WarningMusicDics.TryDequeue(out var item);
                            }
                        }
                        finally
                        {
                            Semaphore.Release();
                        }
                    }));
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                //Log.WriteLog($"生成音频文件错误 {ex.Message}");
            }
        }


        /// <summary>
        /// 生成音频文件
        /// </summary>
        /// <param name="text"> 生成文本 </param>
        /// <param name="filePath">音频存放路径</param>
        public static void GenerateWavFile(string text, string filePath)
        {
            //if (PlatformInfo.IsLinux)
            //{
            //    try
            //    {
            //        InvokeCommand(text, filePath);
            //    }
            //    catch (Exception ex)
            //    {

            //        Log.WriteLog("生成Linux音频报错" + ex.Message);
            //    }
            //}
            //else if (PlatformInfo.IsWindows)
            //{

                try
                {
                    InvokeTtsWinExec(text, filePath);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("生成windows音频报错" + ex.Message);
                }
            //}
        }

        private static string _abbID = "e5760ffa";
        private static string _apiKey = "bedff5362d92df582b4ff6d529880c45";
        private static string _apiSecret = "ODZkNmVhYzlmOGY3OTgwMDMyYjE3MmQ3";
        /// <summary>
        /// 把语音模型文件
        /// </summary>
        /// <param name="text"></param>
        /// <param name="targetPath"></param>
        public static void InvokeTtsWinExec(string text, string targetPath)
        {
            try
            {
                var model = new AikitBuildModel()
                {
                    appID = _abbID,
                    apiKey = _apiKey,
                    apiSecret = _apiSecret,
                    ability = "e2e44feff",
                    authType = 0,
                    resDir = "./res",
                    workDir = "./",
                    text = text,
                    filePath = targetPath
                };
                var sdk = new AIKITSdk(model);
                sdk.CreatWavSource();
            }
            catch (Exception ex)
            {

                Console.WriteLine("生成windows音频报错" + ex.Message);
            }

        }

        public static void InvokeCommand(string text, string targetPath)
        {
            // Linux 相关逻辑  
            // 获取系统驱动器的根目录，例如 "C:\" 或 "D:\"
            string audioFilePath = targetPath;

            //linux编译后程序安装目录
            var execPath = "/root/桌面/test/scada";
            // Sox命令行参数
            var command = $"./aikit_test   {text} {audioFilePath} ";

            Console.WriteLine(command);
            var loopMax = 1;
            for (int i = 0; i < loopMax; i++)
            {
                // 启动Sox进程
                var soxProcess = new Process();
                soxProcess.StartInfo.FileName = "/bin/bash"; // 确保bash shell存在于此路径
                soxProcess.StartInfo.Arguments = $"-c \"{command}\"";
                soxProcess.StartInfo.UseShellExecute = false;
                soxProcess.StartInfo.WorkingDirectory = execPath;
                soxProcess.StartInfo.RedirectStandardOutput = true;
                soxProcess.StartInfo.RedirectStandardError = true;
                soxProcess.StartInfo.CreateNoWindow = true;

                try
                {
                    //等待进程结束  
                    soxProcess.Start();
                    if (!soxProcess.WaitForExit(5000)) // 最多等 5 秒
                    {
                        soxProcess.Kill();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
