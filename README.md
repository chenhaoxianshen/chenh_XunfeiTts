# chenh_XunfeiTts

## 讯飞新版离线语音合成 Aikit 使用指南

### 使用步骤
1. 导入讯飞官网下载的语音模型（xiaoyan 或 小峰），拷贝到程序的执行目录下
2. 拷贝 `AEE_lib.dll` 到执行目录
3. 调用以下代码示例：

```csharp
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
