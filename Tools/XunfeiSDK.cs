using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace chenh_XunfeiTts.Tools
{
    public static class XunfeiSDK
    {
        // 初始化 SDK
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AIKIT_Init(AIKIT_InitParam param);

        // 卸载 SDK
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AIKIT_UnInit();

        // 注册回调函数
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AIKIT_RegisterCallback(AIKIT_Callbacks cbs);

        // 启动 TTS
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AIKIT_Start(string ability, IntPtr param, IntPtr userContext, ref IntPtr handle);

        // 写入数据
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AIKIT_Write(IntPtr handle, IntPtr data);

        // 结束 TTS
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AIKIT_End(IntPtr handle);

        // 创建参数构建器
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AIKITBuilder_Create(BuilderType type);

        // 添加字符串型参数
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AIKITBuilder_AddString(IntPtr handle, string key, string value, int len);

        // 添加字符串型参数
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AIKITBuilder_AddInt(IntPtr handle, string key, int value, int len);

        // 添加输入数据
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AIKITBuilder_AddBuf(IntPtr handle, ref BuilderData data);

        // 构建输入参数
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AIKITBuilder_BuildParam(IntPtr handle);

        /// <summary>
        /// 通过AIKITBuilderHandle对象将刚才设置好的参数转成>>>AIKITBuilder_BuildData>>>对象
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("AEE_lib", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr AIKITBuilder_BuildData(IntPtr handle);

        // 销毁输入构造器
        [DllImport("AEE_lib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AIKITBuilder_Destroy(IntPtr handle);
    }

    // 定义结构体和枚举
    [StructLayout(LayoutKind.Sequential)]
    public struct AIKIT_HANDLE
    {
        public IntPtr usrContext;
        public string abilityID;
        public IntPtr handleID;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AIKIT_OutputData
    {
        public IntPtr node; // AIKIT_BaseData*
        public int count;
        public int totalLen;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AIKIT_Callbacks
    {
        public AIKIT_OnOutput outputCB;
        public AIKIT_OnEvent eventCB;
        public AIKIT_OnError errorCB;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AIKIT_BizParam
    {
        public IntPtr next;
        public IntPtr key;
        public IntPtr value;
        public IntPtr reserved;
        public int len;
        public int type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BuilderData
    {
        public int type;                 // 数据类型
        public string name;              // 数据段名
        public IntPtr data;              // 数据段实体
        public int len;                  // 数据段长度
        public int status;               // 数据段状态
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AIKITBuilderContext
    {
        public IntPtr builderInst;       // 构造器内部类句柄
        public BuilderType type;        // 构造器类型
    }

    public enum BuilderType
    {
        BUILDER_TYPE_PARAM,             // 参数输入
        BUILDER_TYPE_DATA               // 数据输入
    }

    public enum BuilderDataType
    {
        DATA_TYPE_TEXT,                 // 文本
        DATA_TYPE_AUDIO,                // 音频
        DATA_TYPE_IMAGE,                // 图片
        DATA_TYPE_VIDEO                 // 视频
    }


    [StructLayout(LayoutKind.Sequential)]

    public class AIKIT_InitParam
    {
        public int AuthType;         // 授权方式，0=设备级授权，1=应用级授权  
        public string AppID;         // 应用id  
        public string ApiKey;        // 应用key  
        public string ApiSecret;     // 应用secret  
        public string WorkDir;       // sdk工作目录，需可读可写权限  
        public string ResDir;        // 只读资源存放目录，需可读权限  
        public string LicenseFile;   // 离线激活方式的授权文件存放路径，为空时需联网进行首次在线激活  
        public string BatchID;       // 授权批次  
        public string UDID;          // 用户自定义设备标识  
        public string CfgFile;       // 配置文件路径，包括文件名  

    }
    [StructLayout(LayoutKind.Sequential)]

    public class AIKIT_BaseParam
    {
        public IntPtr next; // 链表指针，使用IntPtr代替void*指针
        public IntPtr key; // 指针，使用IntPtr代替const char*
        public IntPtr value; // 指针，使用IntPtr代替void*
        public IntPtr reserved; // 预留字段，使用IntPtr代替void*
        public int len; // 数据长度
        public int type; // 变量类型
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AIKIT_BaseData
    {
        public IntPtr next; // 链表指针，使用IntPtr代替void*指针
        public IntPtr desc; // 指针，使用IntPtr代替const char*
        public string key;
        public IntPtr value; // 指针，使用IntPtr代替void*
        public IntPtr reserved; // 预留字段，使用IntPtr代替void*
        public int len; // 数据长度
        public int type; // 变量类型
        public int status; // 变量类型
        public int from; // 变量类型
    }

    [StructLayout(LayoutKind.Sequential)]
    public class AIKITBuilderHandle
    {
        public IntPtr builderInst;
        public BuilderType type;
    }
    // 定义委托
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AIKIT_OnOutput(IntPtr handle, IntPtr output);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AIKIT_OnEvent(IntPtr handle, int eventType, IntPtr eventValue);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AIKIT_OnError(IntPtr handle, int err, IntPtr desc);
}
