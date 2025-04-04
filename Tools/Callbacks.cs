using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chenh_XunfeiTts.Tools
{
    public static class Callbacks
    {
        public static bool ttsFinished = false;
        public static bool isFirstWrite = true; // 标记是否是首次写入

        public static string FAILName = "OutPut1";

        public static string FilePath = "";
        // 输出回调
        private static readonly object fileLock = new object();

        // 创建 ThreadLocal 实例，每个线程都有独立的 TtsFinished 值，初始值为 false
        public static ThreadLocal<bool> TtsFinished = new ThreadLocal<bool>(() => false);

        public static void OnOutput(IntPtr handle, IntPtr output)
        {

            try
            {
                var outputData = Marshal.PtrToStructure<AIKIT_OutputData>(output);
                var node = Marshal.PtrToStructure<AIKIT_BaseData>(outputData.node);
                Console.WriteLine($"OnOutput key: {node.key}");
                Console.WriteLine($"OnOutput status: {node.status}");

                if (node.value != IntPtr.Zero && node.len > 0)
                {
                    byte[] pcmData = new byte[node.len];
                    Marshal.Copy(node.value, pcmData, 0, node.len);

                    var filePath =Path.Combine(Directory.GetCurrentDirectory(),FAILName.Trim()+".pcm");
                    if (!string.IsNullOrEmpty(FilePath))
                    {
                        if (!Directory.Exists(FilePath))
                        {
                            Directory.CreateDirectory(FilePath);
                        }
                        filePath= Path.Combine(FilePath, FAILName.Trim() + ".pcm");
                    }
                    // **首次写入时，如果文件存在则删除**
                    if (isFirstWrite)
                    { 
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            Console.WriteLine($"Deleted existing {filePath}");
                        }
                        isFirstWrite = false; // 之后不再删除
                    }

                    // **使用 lock 确保只有一个线程能写入文件**
                    lock (fileLock)
                    {
                        // **追加写入 PCM 数据**
                        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                        {
                            fs.Write(pcmData, 0, pcmData.Length);
                        }

                        Console.WriteLine($"Appended {pcmData.Length} bytes to {Path.GetFileName(filePath)}");
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
           
        } 

        // 事件回调
        public static void OnEvent(IntPtr handle, int eventType, IntPtr eventValue)
        {
            Console.WriteLine($"OnEvent: {eventType}");
            if (eventType == 2) // AIKIT_Event_End
            { 
                ttsFinished = true;
                TtsFinished.Value = true;
            }
        }

        // 错误回调
        public static void OnError(IntPtr handle, int err, IntPtr desc)
        {
            string errorDesc = Marshal.PtrToStringAnsi(desc);
            Console.WriteLine($"OnError: {err}, {errorDesc}");
        }
         
    }
}
