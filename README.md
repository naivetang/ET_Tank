# 多人在线坦克对战
是一款 PC 端多人合作对战的 FPS 游戏，玩家通过选择不同类型的坦克、装配不同的炮弹来提升自己的战力和机动性。玩家在游戏前创建游戏房间后，在所有玩家准备之后房主即可开始游戏对局。房间类型分为回合制和时间制两类，回合制对局中，两方阵营中先将对方阵营所有玩家击败的一方获胜，时间制对局中，在规定时间内击败敌军数量多的一方获胜。每辆坦克由以下基础属性组成：火力、防护、机动性。炮弹由以下属性基础组成：破甲、伤害、溅射范围、有效射程。在游戏过程中，玩家可以通过触发和完成特殊事件来获得额外奖励，特殊事件主要包括：时间、伤害、击杀、助攻、胜负。（部分功能未完成，具体完成度可参看[演示视频](https://youtu.be/QMRTo6WiYh4)）
<br>[![Watch the video](https://i9.ytimg.com/vi/QMRTo6WiYh4/mq2.jpg?sqp=CKTq7_kF&rs=AOn4CLDhVCEVDDjmWBrS3ljYJqK_6MhD0Q)](https://youtu.be/QMRTo6WiYh4)

# 开发工具
+ FairyGUI：是一款国内独立团队开发并开源Github的UI框架，相比NGUI与UGUI，FairyGUI有自己独立的UI编辑器，美术人员在UI编辑器内开发界面。效率上FairyGUI对DrawCall优化使用了特有的FairyBatching技术，更加高效而且容易控制。
+ Protobuf：是Google定义的高效的数据交换标准，支持互联网上大多流行开发语言，对于同一套协议，客户端服务器不同开发语言，也能互相序列化和反序列化通信数据。pb在序列化数据之前还会对数据进行优化，比如用户定义一个int型变量传输前赋值大于-127小于128，Protobuf检查需要传输的int型变量只有一个字节有效，在C#环境下将int型切割为Byte型数据，在C++环境下将int型切割为Char型数据（4字节优化为1字节大小）。
+ MongoDB：非关系型数据库，它的特点有高性能、易部署、易使用，数据存储也十分方便，不需要建表建字段，支持对象存储，文件存储格式为Json的扩展Bson
+ ILRuntime：提供一个由纯C#实现，高效、方便且可靠的IL运行时，在此运行时下可以无缝访问C#工程现成代码。将项目分为数据层和热更层，将数据放在数据层，逻辑放在热更层，项目运行入口在数据层，数据层通过IL运行时加载热更层。
# 客户端设计
+ ECS框架设计
    + E是Entity，所有实体类必须继承Entity才能挂载Component，C是Component，是挂载在实体类上的组件，这些组件可以是移动组件、摄像机组件等，挂载了移动组件的Entity可以通过键盘控制移动，挂载了摄像机组件的Entity摄像机会一直跟随着此Entity。S是System，是Component的驱动程序，Component的接口执行入口由System提供。Entity可以挂载Component，所以应当有Component的容器，包含的方法应当有挂载组件的泛型方法AddComponent和移除组件的泛型方法RemoveComponent。
    + Component主要存储数据，逻辑驱动在System，所以没有多余的方法体，只有获取所挂载的Entity。
    + System作为Component的事件驱动系统，至少应该有AwakeSystem，StartSystem，UpdateSystem，FixedUpdateSystem，LateUpdate，DestorySystem，DeserializeSystem。
+ 资源加载
    ResourceComponent是管理所有AssetBundle的资源管理类，包括了加载AssetBundle、获取AssetBundle中的Asset，卸载AssetBundle等方法，其中加载AssetBundle又有两种加载方式，同步加载方法会让玩家等待资源加载完成，异步加载会开启一个协程加载资源，不阻塞主进程。
+ 事件系统（发布订阅模式）
    基于发布订阅模式的事件系统是完全无耦合中间系统，将数据和逻辑分开，数据变更时抛出事件，订阅本事件的逻辑被驱动。新增的逻辑只需要在EventSystem中注册对应的事件，数据变化就会驱动此逻辑。
+ UI系统
    + Backaground层一般是层级最低的UI，为一些全屏背景图
    + Home为主界面，包括登陆界面、大厅界面、房间界面
    + FixedView一般为固定UI，比如战斗场景自己的血量条、技能面板等
    + Dialog界面一般为弹出界面，大小不能覆盖全屏幕，能看见Dialog下面的Home界面但是无法点击，只有关闭Dialog后才能再跟Home界面交互
    + Tip界面层级较高，一般不可交互，是系统给用户的一些提示消息，自动弹出自动关闭
    + Loading界面层级最高，在场景过渡时候使用

# 服务器设计

# 参考
+ [ET框架](https://github.com/egametang/ET) Unity3D Client And C# Server Framework
+ [FariyGUI](https://github.com/fairygui/FairyGUI-unity) UI中间件