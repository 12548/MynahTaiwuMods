return {
    Title = "~Sqlite框架~",
    BackendPlugins = { "SqliteFrameworkBackend.dll" },
    FrontendPlugins = { "SqliteFrameworkFrontend.dll" },
    Author = "myna12548",
    GameVersion="99.99.99.99",
    Source = 1,
    FileId = 2995720038,
    --Cover = "cover.png",
    Description = [[给存档绑定一个Sqlite3数据库。
    
    注意：由于dll加载限制，本mod将会拷贝一个e_sqlite3.dll到游戏目录下，卸载本mod后可自行删除此文件。
    ]],
    DefaultSettings = {
    },
    Dependencies = { 2995707635 },
}
