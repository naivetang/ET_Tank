namespace ETModel
{
    // 一个数值可能会多种情况影响，比如速度,加个buff可能增加速度绝对值100，也有些buff增加10%速度，所以一个值可以由5个值进行控制其最终结果
    // final = (((base + add) * (100 + pct) / 100) + finalAdd) * (100 + finalPct) / 100;
    public enum 
            NumericType
    {
		Max = 10000,

        // 最终显示的值
		Speed = 1000,
        // 基础值
		SpeedBase = Speed * 10 + 1,
        // 增加的绝对值
	    SpeedAdd = Speed * 10 + 2,
        // 增加的百分比
	    SpeedPct = Speed * 10 + 3,
        // 最终增加绝对值
	    SpeedFinalAdd = Speed * 10 + 4,
        // 最终增加百分比
	    SpeedFinalPct = Speed * 10 + 5,

	    Hp = 1001,
	    HpBase = Hp * 10 + 1,

	    MaxHp = 1002,
	    MaxHpBase = MaxHp * 10 + 1,
	    MaxHpAdd = MaxHp * 10 + 2,
	    MaxHpPct = MaxHp * 10 + 3,
	    MaxHpFinalAdd = MaxHp * 10 + 4,
		MaxHpFinalPct = MaxHp * 10 + 5,

        Atk = 1003,
        AtkBase = Atk * 10 + 1,
        AtkAdd = Atk * 10 + 2,
	}
}
