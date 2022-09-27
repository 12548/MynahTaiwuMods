return {
	["Title"] = "12548演示Mod",
	["FrontendPlugins"] = {
		[1] = "DemoMod.dll",
		},
	["Author"] = "myna12548",
	["Description"] = "用来演示和测试一些Mod功能",
	["DefaultSettings"] = {
		[1] = {
			["Key"] = "a",
			["SettingType"] = "Toggle",
			["DisplayName"] = "布尔A",
			["Description"] = "这是一个简单的布尔值",
			["DefaultValue"] = true,
			},
		[2] = {
			["Key"] = "b",
			["SettingType"] = "InputField",
			["DisplayName"] = "字符串B",
			["Description"] = "",
			["DefaultValue"] = "asdf",
			},
		[3] = {
			["Key"] = "c",
			["SettingType"] = "Slider",
			["DisplayName"] = "滑动C",
			["Description"] = "",
			["DefaultValue"] = 0,
			["MinValue"] = 3,
			["MaxValue"] = 30,
			["StepSize"] = 1,
			},
		[4] = {
			["Key"] = "d",
			["SettingType"] = "Dropdown",
			["DisplayName"] = "下拉d",
			["Description"] = "",
			["DefaultValue"] = 0,
			["Options"] = {
				[1] = "选项J",
				[2] = "选项L",
				},
			},
		},
	}
