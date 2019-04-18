using ETModel;
namespace ETModel
{
	[Message(OuterOpcode.Actor_Test)]
	public partial class Actor_Test : IActorMessage {}

	[Message(OuterOpcode.C2M_TestRequest)]
	public partial class C2M_TestRequest : IActorLocationRequest {}

	[Message(OuterOpcode.M2C_TestResponse)]
	public partial class M2C_TestResponse : IActorLocationResponse {}

	[Message(OuterOpcode.Actor_TransferRequest)]
	public partial class Actor_TransferRequest : IActorLocationRequest {}

	[Message(OuterOpcode.Actor_TransferResponse)]
	public partial class Actor_TransferResponse : IActorLocationResponse {}

	[Message(OuterOpcode.C2G_EnterMap)]
	public partial class C2G_EnterMap : IRequest {}

	[Message(OuterOpcode.G2C_EnterMap)]
	public partial class G2C_EnterMap : IResponse {}

// 自己的unit id
// 所有的unit
	[Message(OuterOpcode.C2G_EnterBattle)]
	public partial class C2G_EnterBattle : IRequest {}

	[Message(OuterOpcode.G2C_EnterBattle)]
	public partial class G2C_EnterBattle : IResponse {}

// 自己的Tank id
// 所有的Tank
	[Message(OuterOpcode.UnitInfo)]
	public partial class UnitInfo {}

	[Message(OuterOpcode.TankFrameInfo)]
	public partial class TankFrameInfo {}

// 位置
// 朝向
	[Message(OuterOpcode.M2C_CreateUnits)]
	public partial class M2C_CreateUnits : IActorMessage {}

// 所有的unit
	[Message(OuterOpcode.TankInfoFirstEnter)]
	public partial class TankInfoFirstEnter {}

	[Message(OuterOpcode.B2C_CreateTanks)]
	public partial class B2C_CreateTanks : IActorMessage {}

// 所有的unit
	[Message(OuterOpcode.Frame_ClickMap)]
	public partial class Frame_ClickMap : IActorLocationMessage {}

	[Message(OuterOpcode.C2B_TankFrameInfo)]
	public partial class C2B_TankFrameInfo : IActorLocationMessage {}

	[Message(OuterOpcode.B2C_TankFrameInfos)]
	public partial class B2C_TankFrameInfos : IActorMessage {}

	[Message(OuterOpcode.M2C_PathfindingResult)]
	public partial class M2C_PathfindingResult : IActorMessage {}

	[Message(OuterOpcode.C2R_Ping)]
	public partial class C2R_Ping : IRequest {}

	[Message(OuterOpcode.R2C_Ping)]
	public partial class R2C_Ping : IResponse {}

	[Message(OuterOpcode.G2C_Test)]
	public partial class G2C_Test : IMessage {}

	[Message(OuterOpcode.C2M_Reload)]
	public partial class C2M_Reload : IRequest {}

	[Message(OuterOpcode.M2C_Reload)]
	public partial class M2C_Reload : IResponse {}

	[Message(OuterOpcode.C2B_Shoot)]
	public partial class C2B_Shoot : IActorLocationMessage {}

	[Message(OuterOpcode.B2C_Shoot)]
	public partial class B2C_Shoot : IActorMessage {}

	[Message(OuterOpcode.C2B_AttackTankRequest)]
	public partial class C2B_AttackTankRequest : IActorLocationRequest {}

// 源坦克id
// 目标坦克id
// 攻击力
	[Message(OuterOpcode.B2C_AttackTankResponse)]
	public partial class B2C_AttackTankResponse : IActorLocationResponse {}

// 攻击者
// 目标坦克id
// 当前血量
	[Message(OuterOpcode.B2C_AttackTank)]
	public partial class B2C_AttackTank : IActorMessage {}

// 源坦克id
// 目标坦克id
// 当前血量
	[Message(OuterOpcode.C2G_CreateRoom)]
	public partial class C2G_CreateRoom : IRequest {}

// 人数
// 地图
// 模式 1回合制，2时间制
// 回合制：多少个回合  时间制：多少分钟
// 房间名
	[Message(OuterOpcode.C2G_EnterRoom)]
	public partial class C2G_EnterRoom : IRequest {}

	[Message(OuterOpcode.RoomOnePeople)]
	public partial class RoomOnePeople {}

// 等级
// 名字
// false：未准备  true：准备
// 玩家id
// 阵营 1:左边 2：右边
	[Message(OuterOpcode.RoomSimpleInfo)]
	public partial class RoomSimpleInfo {}

//房间Id
//人数
//地图
//模式 1回合制，2时间制
//回合制：多少个回合  时间制：多少分钟
//房间名
//当前状态 1:准备中 2:游戏中
//房间序号
// 房主id
// 房间内的详细信息
	[Message(OuterOpcode.G2C_RoomDetailInfo)]
	public partial class G2C_RoomDetailInfo : IMessage {}

	[Message(OuterOpcode.G2C_UserBaseInfo)]
	public partial class G2C_UserBaseInfo : IMessage {}

// 等级
// 经验
// 名字
	[Message(OuterOpcode.G2C_Rooms)]
	public partial class G2C_Rooms : IMessage {}

	[Message(OuterOpcode.C2G_StartGame)]
	public partial class C2G_StartGame : IMessage {}

	[Message(OuterOpcode.G2C_StartGame)]
	public partial class G2C_StartGame : IMessage {}

	[Message(OuterOpcode.C2B_LoadAssetFinish)]
	public partial class C2B_LoadAssetFinish : IActorLocationMessage {}

}
namespace ETModel
{
	public static partial class OuterOpcode
	{
		 public const ushort Actor_Test = 101;
		 public const ushort C2M_TestRequest = 102;
		 public const ushort M2C_TestResponse = 103;
		 public const ushort Actor_TransferRequest = 104;
		 public const ushort Actor_TransferResponse = 105;
		 public const ushort C2G_EnterMap = 106;
		 public const ushort G2C_EnterMap = 107;
		 public const ushort C2G_EnterBattle = 108;
		 public const ushort G2C_EnterBattle = 109;
		 public const ushort UnitInfo = 110;
		 public const ushort TankFrameInfo = 111;
		 public const ushort M2C_CreateUnits = 112;
		 public const ushort TankInfoFirstEnter = 113;
		 public const ushort B2C_CreateTanks = 114;
		 public const ushort Frame_ClickMap = 115;
		 public const ushort C2B_TankFrameInfo = 116;
		 public const ushort B2C_TankFrameInfos = 117;
		 public const ushort M2C_PathfindingResult = 118;
		 public const ushort C2R_Ping = 119;
		 public const ushort R2C_Ping = 120;
		 public const ushort G2C_Test = 121;
		 public const ushort C2M_Reload = 122;
		 public const ushort M2C_Reload = 123;
		 public const ushort C2B_Shoot = 124;
		 public const ushort B2C_Shoot = 125;
		 public const ushort C2B_AttackTankRequest = 126;
		 public const ushort B2C_AttackTankResponse = 127;
		 public const ushort B2C_AttackTank = 128;
		 public const ushort C2G_CreateRoom = 129;
		 public const ushort C2G_EnterRoom = 130;
		 public const ushort RoomOnePeople = 131;
		 public const ushort RoomSimpleInfo = 132;
		 public const ushort G2C_RoomDetailInfo = 133;
		 public const ushort G2C_UserBaseInfo = 134;
		 public const ushort G2C_Rooms = 135;
		 public const ushort C2G_StartGame = 136;
		 public const ushort G2C_StartGame = 137;
		 public const ushort C2B_LoadAssetFinish = 138;
	}
}
