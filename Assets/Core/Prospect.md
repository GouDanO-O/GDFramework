# 预览
## 游戏介绍

整个游戏由宇宙->世界->区域->副本->房间

    上述所有的结构,都基于ChunkData构成,ChunkData中又分为 
    
    ChunkDtoDef: 全存档通用对应固定数据,运行时无法在游戏中被更改(但是可以在编辑工坊中进行编辑)--使用Json来进行管理
    打包前,存储在Assets/Core/Res/Configs/ChunkData中作为默认剧本来游玩
    打包后,存储在Application.persistentDataPath中作为Mod剧本来游玩
    
    ChunkTemporaryData: 当前存档中对应临时数据,运行时,随着玩家的互动或时间的流逝而改变
    无论打包前后,统一基于ES3来进行存储管理,默认路径为Application.persistentDataPath
    
    故,当删除游戏所有数据后,Mod剧本和所有临时数据都会被清空(如无云存档)

**其中房间是最小可探索区块,房间里面由nXm个瓦片组成,大致可以参照Rimworld的地图,然后,实体(可互动单元的基类)是房间的重要组成部分
并且实体基于瓦片,实体又可细分为装饰物,物体,生物等等**

## 剧本编辑器

游戏内置的创意工坊,可以运行时创建剧本

## 战斗系统

大致设想的战斗系统如下

宇宙:

![image-20251114222822686](C:\Users\123\AppData\Roaming\Typora\typora-user-images\image-20251114222822686.png)

![image-20251114222449948](C:\Users\123\AppData\Roaming\Typora\typora-user-images\image-20251114222449948.png)

世界:



![image-20251114222213426](C:\Users\123\AppData\Roaming\Typora\typora-user-images\image-20251114222213426.png)

区域:

类似下面两者的结合体

![image-20251114222305259](C:\Users\123\AppData\Roaming\Typora\typora-user-images\image-20251114222305259.png)

![image-20251114222315614](C:\Users\123\AppData\Roaming\Typora\typora-user-images\image-20251114222315614.png)

副本:



![image-20251114220928414](C:\Users\123\AppData\Roaming\Typora\typora-user-images\image-20251114220928414.png)

房间:

https://store.steampowered.com/app/1534980/_/

可以认为大部分玩法,就对着他抄了

不过不会做的像他这么复杂

![image-20251114220030449](C:\Users\123\AppData\Roaming\Typora\typora-user-images\image-20251114220030449.png)

![image-20251114220953357](C:\Users\123\AppData\Roaming\Typora\typora-user-images\image-20251114220953357.png)


## 背包系统

## 对话和任务系统
