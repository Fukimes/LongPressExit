# LongPressExit

这是一个面向 **.NET Framework 4.7.2 + MelonLoader** 的 MelonLoader mod，为不包含相关功能的原版 Dll 添加：

- 按住任意配置的 Select 按钮十秒退出游戏，并显示倒计时和进度条。
- 游戏进行中按配置的 TrackSkip 按键，结束当前游戏并 TrackSkip 。

## MelonPreferences 配置

首次启动会创建 `LongPressExit` 分类，并由 MelonLoader 自动保存配置。默认值如下：

```toml
[LongPressExit]
LongPressButton = "Select"
TrackSkipKey = "Space"
TrackSkipAlternateKey = "KeypadPlus"
```

`LongPressButton` 使用 `Manager.InputManager.ButtonSetting` 的枚举名称，可选 `Button01` 到 `Button08` 或 `Select`。两个 TrackSkip 配置使用 Unity `KeyCode` 的枚举名称，例如 `Space`、`KeypadPlus`、`F1`。

## 项目结构

- `patch/GameMainObject.cs`：长按退出和提示 UI。
- `patch/GameProcess.cs`：TrackSkip 流程结束补丁。
- `Preferences.cs`：MelonPreferences 配置定义、读取和按键解析。
- `Libs/`：本地编译依赖目录。

## 编译依赖

请在编译前手动将下面这些 DLL 放入项目根目录的 `Libs/` 文件夹。它们应来自目标游戏安装目录和对应的 MelonLoader 安装，版本必须与目标游戏匹配：

```text
0Harmony.dll
AMDaemon.NET.dll
Assembly-CSharp-firstpass.dll
Assembly-CSharp.dll
MelonLoader.dll
UnityEngine.dll
UnityEngine.CoreModule.dll
UnityEngine.IMGUIModule.dll
UnityEngine.InputModule.dll
UnityEngine.TextRenderingModule.dll
```

## 编译

1. 使用 Visual Studio 打开 `LongPressExit.sln`，先按上面的列表准备 `Libs/` 中的 DLL。
2. 选择 `Release | x64`，还原 NuGet 包 `Microsoft.NETFramework.ReferenceAssemblies.net472` 后生成。
3. 输出文件为 `bin\Release\net472\LongPressExit.dll`。
4. 将 DLL 放到游戏目录的 `Mods` 文件夹。
