using ETModel;
namespace ETHotfix
{
	[Message(HotfixOpcode.C2R_Regist)]
	public partial class C2R_Regist : IRequest {}

	[Message(HotfixOpcode.R2C_Regist)]
	public partial class R2C_Regist : IResponse {}

	[Message(HotfixOpcode.C2R_Login)]
	public partial class C2R_Login : IRequest {}

	[Message(HotfixOpcode.R2C_Login)]
	public partial class R2C_Login : IResponse {}

// 网关地址 因为网关服务器跟游戏服务器在同一台服务器，所以地址一样
// 网关验证
	[Message(HotfixOpcode.C2G_LoginGate)]
	public partial class C2G_LoginGate : IRequest {}

// R2C_Login中的网关验证key
	[Message(HotfixOpcode.G2C_LoginGate)]
	public partial class G2C_LoginGate : IResponse {}

// 服务器ComponentFactory.Create的Player的Id
	[Message(HotfixOpcode.G2C_TestHotfixMessage)]
	public partial class G2C_TestHotfixMessage : IMessage {}

	[Message(HotfixOpcode.C2M_TestActorRequest)]
	public partial class C2M_TestActorRequest : IActorLocationRequest {}

	[Message(HotfixOpcode.M2C_TestActorResponse)]
	public partial class M2C_TestActorResponse : IActorLocationResponse {}

	[Message(HotfixOpcode.PlayerInfo)]
	public partial class PlayerInfo : IMessage {}

	[Message(HotfixOpcode.C2G_PlayerInfo)]
	public partial class C2G_PlayerInfo : IRequest {}

	[Message(HotfixOpcode.G2C_PlayerInfo)]
	public partial class G2C_PlayerInfo : IResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixOpcode
	{
		 public const ushort C2R_Regist = 10001;
		 public const ushort R2C_Regist = 10002;
		 public const ushort C2R_Login = 10003;
		 public const ushort R2C_Login = 10004;
		 public const ushort C2G_LoginGate = 10005;
		 public const ushort G2C_LoginGate = 10006;
		 public const ushort G2C_TestHotfixMessage = 10007;
		 public const ushort C2M_TestActorRequest = 10008;
		 public const ushort M2C_TestActorResponse = 10009;
		 public const ushort PlayerInfo = 10010;
		 public const ushort C2G_PlayerInfo = 10011;
		 public const ushort G2C_PlayerInfo = 10012;
	}
}
