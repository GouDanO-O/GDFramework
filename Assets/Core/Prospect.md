# 预览
## 游戏介绍

整个游戏由宇宙->世界->区域->副本->房间

    上述所有的结构,都基于ChunkData构成,ChunkData中又分为 

    ChunkDtoDef: 全存档通用对应固定数据,运行时无法在游戏中被更改(但是可以在编辑工坊中进行编辑)--使用Json来进行管理
    打包前,存储在Assets/Core/Res/Configs/ChunkData中作为默认剧本来游玩
    打包后,存储在Application.persistentDataPath中作为Mod剧本来游玩

    ChunkTemporaryData: 当前存档中对应临时数据,运行时,随着玩家的互动或时间的流逝而改变
    无论打包前后,统一基于ES3来进行存储管理,默认路径为Application.persistentDataPath
    
    故,当删除游戏所有数据后,Mod剧本和所有临时数据都会被情况(如无云存档)

**其中房间是最小可探索区块,房间里面由nXm个瓦片组成,大致可以参照Rimworld的地图,然后,实体(可互动单元的基类)是房间的重要组成部分
并且实体基于瓦片,实体又可细分为装饰物,物体,生物等等**


## 剧本编辑器



## 战斗系统


## 背包系统

## 对话系统

## 
