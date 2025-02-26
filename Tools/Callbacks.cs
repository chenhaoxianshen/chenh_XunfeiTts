using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace chenh_XunfeiTts.Tools
{
    public static class Callbacks
    {
        private static bool ttsFinished = false;
        public static bool isFirstWrite = true; // 标记是否是首次写入

        public static string FAILName = "OutPut1";
        // 输出回调
        public static void OnOutput(IntPtr handle, IntPtr output)
        { 
            var outputData = Marshal.PtrToStructure<AIKIT_OutputData>(output);
            var node = Marshal.PtrToStructure<AIKIT_BaseData>(outputData.node); 
            Console.WriteLine($"OnOutput key: {node.key}");
            Console.WriteLine($"OnOutput status: {node.status}");

            if (node.value != IntPtr.Zero && node.len > 0)
            {
                byte[] pcmData = new byte[node.len];
                Marshal.Copy(node.value, pcmData, 0, node.len);

                // **首次写入时，如果文件存在则删除**
                if (isFirstWrite)
                {
                    if (File.Exists(FAILName+".pcm"))
                    {
                        File.Delete(FAILName+".pcm");
                        Console.WriteLine($"Deleted existing {FAILName}.pcm");
                    }
                    isFirstWrite = false; // 之后不再删除
                }

                // **追加写入 PCM 数据**
                using (FileStream fs = new FileStream(FAILName+".pcm", FileMode.Append, FileAccess.Write))
                {
                    fs.Write(pcmData, 0, pcmData.Length);
                }

                Console.WriteLine($"Appended {pcmData.Length} bytes to {FAILName}.pcm");
            }
        }

        // 事件回调
        public static void OnEvent(IntPtr handle, int eventType, IntPtr eventValue)
        {
            Console.WriteLine($"OnEvent: {eventType}");
            if (eventType == 2) // AIKIT_Event_End
            {
                ttsFinished = true;
            }
        }

        // 错误回调
        public static void OnError(IntPtr handle, int err, IntPtr desc)
        {
            string errorDesc = Marshal.PtrToStringAnsi(desc);
            Console.WriteLine($"OnError: {err}, {errorDesc}");
        }

        public static bool IsTtsFinished()
        {
            return ttsFinished;
        }
    }
}
