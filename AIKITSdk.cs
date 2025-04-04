using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using chenh_XunfeiTts.Tools;
using chenh_XunfeiTts;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace chenh_XunfeiTts
{
    public class AIKITSdk
    {
        private static string _appID;
        private static string _apiKey;
        private static string _apiSecret;
                    
        private static string _abilitys;
        private static int _authType = 0;
        private static string _resDir;
        private static string _workDir;
        private static string _text;
        private static string _filePath;
        private static string _vcn="xiaoyan"; 

        public  bool ttsFinished = false;
        public AIKITSdk(AikitBuildModel model) {


            try
            {
                _appID = model.appID;
                _apiKey = model.apiKey;
                _apiSecret = model.apiSecret;
                _abilitys = model.ability;
                _authType = model.authType;
                _resDir = model.resDir;
                _workDir = model.workDir;
                _text = model.text;
                _vcn = model.vcn;
                _filePath = model.filePath;
                Callbacks.FAILName = Path.GetFileNameWithoutExtension(_filePath);
                Callbacks.FilePath = Path.GetDirectoryName(_filePath); 
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message); ;
            }

        }

        public  void CreatWavSource()
        {
            // 设置回调
            AIKIT_Callbacks cbs = new AIKIT_Callbacks
            {
                outputCB = Callbacks.OnOutput,
                eventCB = Callbacks.OnEvent,
                errorCB = Callbacks.OnError
            };

            // 注册回调
            int ret = XunfeiSDK.AIKIT_RegisterCallback(cbs);
            if (ret != 0)
            {
                Console.WriteLine("AIKIT_RegisterCallback failed: " + ret);
                return;
            }
            IntPtr _akikt_handle = IntPtr.Zero; 

            string _ability = _abilitys;
            AIKIT_InitParam paraminit = new AIKIT_InitParam();
            paraminit.AppID = _appID;
            paraminit.ApiSecret = _apiSecret;
            paraminit.ApiKey = _apiKey;
            paraminit.AuthType = _authType;
            paraminit.ResDir = _resDir;
            paraminit.WorkDir = _workDir;

            // 初始化 SDK 
            int result = XunfeiSDK.AIKIT_Init(paraminit);
            if (ret != 0)
            {
                Console.WriteLine("AIKIT_Init failed: " + ret);
                return;
            }
            var pcmPath = Path.Combine(Callbacks.FilePath, Callbacks.FAILName)+".pcm";
            var wavPath = _filePath;

            // 测试 TTS
            WriteToSourceFile( pcmPath);
            //// 卸载 SDK
            XunfeiSDK.AIKIT_UnInit();


            ConvertPcmToWav(pcmPath,wavPath);




            //if (File.Exists(pcmPath)) { 
            //    File.Delete(pcmPath);
            //}  
        }

        public void WriteToSourceFile(string pcmPath)
        {
            Callbacks.ttsFinished = false; 
            Callbacks.isFirstWrite = true;
            string text = _text;
            IntPtr handle = IntPtr.Zero;

            // 创建参数构建器
            IntPtr paramBuilder = XunfeiSDK.AIKITBuilder_Create(BuilderType.BUILDER_TYPE_PARAM);
            if (paramBuilder == IntPtr.Zero)
            {
                Console.WriteLine("AIKITBuilder_Create failed");
                return;
            }

            // 添加参数 
             string role = string.IsNullOrEmpty(_vcn)?"xiaoyan":_vcn; // xiaofeng xiaoyan
            XunfeiSDK.AIKITBuilder_AddString(paramBuilder, "vcn", role, role.Length); // 必选参数，发音人
            //XunfeiSDK.AIKITBuilder_AddString(paramBuilder, "vcnModel", role, role.Length); // 必选参数，发音人模型
            XunfeiSDK.AIKITBuilder_AddInt(paramBuilder, "language", 1, 1); // 必选参数，1是中文
            XunfeiSDK.AIKITBuilder_AddString(paramBuilder, "textEncoding", "UTF-8", "UTF-8".Length); // 可选参数，文本编码格式

            // 构建参数
            IntPtr param = XunfeiSDK.AIKITBuilder_BuildParam(paramBuilder);
            if (param == IntPtr.Zero)
            {
                Console.WriteLine("AIKITBuilder_BuildParam failed");
                return;
            }

            // 启动 TTS
            int ret = XunfeiSDK.AIKIT_Start(_abilitys, param, IntPtr.Zero, ref handle);
            Console.WriteLine("AIKIT_Start: " + ret);
            if (ret != 0)
            {
                return;
            }

            // 创建数据构建器
            IntPtr dataBuilder = XunfeiSDK.AIKITBuilder_Create(BuilderType.BUILDER_TYPE_DATA);
            if (dataBuilder == IntPtr.Zero)
            {
                Console.WriteLine("AIKITBuilder_Create failed");
                return;
            }

            // 添加数据
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            BuilderData builderData = new BuilderData
            {
                type = (int)BuilderDataType.DATA_TYPE_TEXT,
                name = "text",
                data = Marshal.AllocHGlobal(textBytes.Length),
                len = textBytes.Length,
                status = 1 // 数据状态
            };

            // 将文本数据复制到非托管内存
            Marshal.Copy(textBytes, 0, builderData.data, textBytes.Length);

            // 添加数据到构建器
            ret = XunfeiSDK.AIKITBuilder_AddBuf(dataBuilder, ref builderData);
            if (ret != 0)
            {
                Console.WriteLine("AIKITBuilder_AddBuf failed: " + ret);
                Marshal.FreeHGlobal(builderData.data); // 释放内存
                return;
            }

            // 构建数据
            IntPtr data = XunfeiSDK.AIKITBuilder_BuildData(dataBuilder);
            if (data == IntPtr.Zero)
            {
                Console.WriteLine("AIKITBuilder_BuildData failed");
                Marshal.FreeHGlobal(builderData.data); // 释放内存
                return;
            }

            // 写入数据
            ret = XunfeiSDK.AIKIT_Write(handle, data);
            Console.WriteLine("AIKIT_Write: " + ret);
            if (ret != 0)
            {
                Console.WriteLine("AIKIT_Write failed: " + ret);
                Marshal.FreeHGlobal(builderData.data); // 释放内存
                return;
            }

            // 等待 TTS 完成
            while (!Callbacks.ttsFinished)
            {
                System.Threading.Thread.Sleep(50);
            }

            // 结束 TTS
            ret = XunfeiSDK.AIKIT_End(handle);
            Console.WriteLine("AIKIT_End: " + ret);

            // 销毁构建器
            XunfeiSDK.AIKITBuilder_Destroy(paramBuilder);
            XunfeiSDK.AIKITBuilder_Destroy(dataBuilder);
        }

        public  void ConvertPcmToWav(string pcmFile, string wavFile, int sampleRate = 16000, int channels = 1, int bitsPerSample = 16)
        {
            if (File.Exists(pcmFile))
            {
                using (FileStream pcmStream = new FileStream(pcmFile, FileMode.Open, FileAccess.Read))
                using (FileStream wavStream = new FileStream(wavFile, FileMode.Create, FileAccess.Write))
                {
                    byte[] pcmData = new byte[pcmStream.Length];
                    pcmStream.Read(pcmData, 0, pcmData.Length);

                    // 计算 WAV 头部信息
                    int byteRate = sampleRate * channels * (bitsPerSample / 8);
                    int subChunk2Size = pcmData.Length;
                    int chunkSize = 36 + subChunk2Size;

                    using (BinaryWriter writer = new BinaryWriter(wavStream))
                    {
                        // 写入 WAV 头部
                        writer.Write(new char[] { 'R', 'I', 'F', 'F' }); // ChunkID
                        writer.Write(chunkSize); // ChunkSize
                        writer.Write(new char[] { 'W', 'A', 'V', 'E' }); // Format

                        // fmt 子块
                        writer.Write(new char[] { 'f', 'm', 't', ' ' }); // Subchunk1ID
                        writer.Write(16); // Subchunk1Size (PCM = 16)
                        writer.Write((short)1); // AudioFormat (PCM = 1)
                        writer.Write((short)channels); // NumChannels
                        writer.Write(sampleRate); // SampleRate
                        writer.Write(byteRate); // ByteRate
                        writer.Write((short)(channels * (bitsPerSample / 8))); // BlockAlign
                        writer.Write((short)bitsPerSample); // BitsPerSample

                        // data 子块
                        writer.Write(new char[] { 'd', 'a', 't', 'a' }); // Subchunk2ID
                        writer.Write(subChunk2Size); // Subchunk2Size
                        writer.Write(pcmData); // PCM 数据
                    }
                } 
                Console.WriteLine("PCM 转换为 WAV 完成: " + wavFile);
            }
        }
    }
}
