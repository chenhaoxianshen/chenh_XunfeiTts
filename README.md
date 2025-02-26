# chenh_XunfeiTts
讯飞新版离线语音合成
使用，导入讯飞官网下载的语音模型，xiaoyan或者小峰，拷贝到程序的执行目录下，拷贝AEE_lib.dll到执行目录，调用
     var model = new AikitBuildModel()
     {
         appID = "xxxxx",
         apiKey = "xxxxx",
         apiSecret = "xxxxxxx",
         ability = "e2e44feff",
         authType = 0,
         resDir = "./res",
         workDir = "./",
         text = "解放西，长沙人的佛罗里达",
         fileName = "文件名" 
     };
     var sdk = new AIKITSdk(model);
     sdk.CreatWavSource();
