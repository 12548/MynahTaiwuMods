MynahModConfigGenerator.MSBuild
-------------------------------

[MynahModConfigGenerator](../MynahModConfigGenerator)的NuGet包形式，方便快速引入太吾绘卷插件项目。

在项目中，通过添加生成任务`GenerateTaiwuConfigTask`，可以实现基于代码的太吾绘卷mod配置信息生成，根据annotation自动向Config.lua注入配置信息，具体配置信息参考[MynahBaseModBase.cs](../MynahBaseModBase/MynahBaseModBase.cs)。

## 使用方式

1. 在项目中引入依赖。
```xml
  <ItemGroup>
    <PackageReference Include="TaiwuConfigGenerator" Version="1.*" PrivateAssets="all" />
  </ItemGroup>
```

2. 配置项目发布，将Config.lua和生成的plugin dll拷贝到发布路径，例如：（其中GameDir需替换为你的游戏安装路径）
```xml
  <PropertyGroup>
    <OutputDir>.\Mod\</OutputDir>
  </PropertyGroup>

  <Target Name="CopyPlugin" AfterTargets="Build">
    <Copy SourceFiles="Config.lua" DestinationFolder="$(OutputDir)" />
    <Copy SourceFiles="$(TargetDir)$(ProjectName).dll" DestinationFolder="$(OutputDir)Plugins" />
    <Copy SourceFiles="$(TargetDir)$(ProjectName).pdb" DestinationFolder="$(OutputDir)Plugins" />
  </Target>
```

3. 通过`GenerateTaiwuConfigTask`任务注入配置描述信息，例如：
```xml
  <Target Name="GenerateConfig" AfterTargets="Build">
    <GenerateTaiwuConfigTask PluginDir="$(OutputDir)" />
  </Target>
```

## GenerateTaiwuConfigTask参数定义

`GenerateTaiwuConfigTask`包含如下参数：

|     参数     |  描述                                                                         |
|-------------|------------------------------------------------------------------------------|
|  PluginDir  |插件发布目录，假设路径下包含Config.lua文件和Plugins文件夹，自动读取Plugins中的dll生成配置|
|ConfigLuaPath|`未指定PluginDir时` 指定待注入配置信息的基础Config.lua路径                           |
|  TargetDll  |`未指定PluginDir时` 指定插件dll所在路径                                            |

## 编译

1. 克隆代码库。
```shell
git clone https://github.com/12548/MynahTaiwuMods.git
cd MynahTaiwuMods
```

2. 编译NuGet包，设置环境变量`TAIWU_PATH`为你的游戏安装路径。

假设你处于项目根目录，以cmd为例，其中路径为游戏路径示例，需要替换为你的游戏安装路径。
```shell
set TAIWU_PATH=E:\SteamLibrary\steamapps\common\The Scroll Of Taiwu\
dotnet build ./MynahModConfigGenerator.MSBuild --configuration Release
```

3. 输出包应位于`./bin/Release`路径下。

## 本地使用

1. 将NuGet包所在路径加入NuGet源即可引入进行测试。

此处以VS2022为例，Visual Studio中菜单栏选择 `工具-NuGet包管理器-程序包管理器设置` ，进入 `程序包源` 配置项，点击绿色 `+` 号添加源，`名称` 可以任意设置，`源` 选择生成的NuGet包所在路径。

2. 由于VS会对NuGet包进行缓存，如果本地开发时重新生成了新的NuGet包，需要删除包缓存后重启VS才会生效，一般位于 `<盘符>:\Users\<用户名>\.nuget\packages\`（可以通过win+r或路径栏输入`%appdata%/../../.nuget/packages/mynahmodconfiggenerator.msbuild`跳转）。
