return {
    Title = "~制作物品预览 v0.8~",
    ["BackendPlugins"] = { "MakeItemPreviewBackend.dll" },
    ["FrontendPlugins"] = {
        "MakeItemPreviewFrontend.dll" },
    Author = "myna12548",
    Description = [[为制作物品增加浮窗预览。注意：预览仅限制造，不适用于精制、淬毒等其他操作；
    
预览物品为基础物品，不含属性加成。

0.7预览使用方法：先按ctrl固定原版预览浮窗，然后把鼠标放在原版浮窗的预览物品上，再按一下ctrl出mod预览浮窗。
目前略显复杂，正在考虑如何简化

0.5更新：修正部分已知问题
0.6更新：适配最新版mod依赖系统
0.7更新：适配新版更新，改变预览模式（看上面的使用方法）
0.8更新：适配新版更新
]],
    Source = 1,
    FileId = 2875353985,
    DefaultSettings = {
    },
    Dependencies = { 2878665107 }
}
