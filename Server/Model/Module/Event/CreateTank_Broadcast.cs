namespace ETModel
{
    [Event(EventIdType.CreateTanked)]
    public class CreateTank_Broadcast : AEvent
    {
        public override void Run()
        {
            B2C_CreateTanks createTanks = new B2C_CreateTanks();

            Tank[] tanks = Game.Scene.GetComponent<TankComponent>().GetAll();

            foreach (Tank t in tanks)
            {
                TankFrameInfo tankFrameInfo = new TankFrameInfo();
                tankFrameInfo.TankId = t.Id;
                tankFrameInfo.PX = t.PX;
                tankFrameInfo.PY = t.PY;
                tankFrameInfo.PZ = t.PZ;
                tankFrameInfo.RX = t.RX;
                tankFrameInfo.RY = t.RY;
                tankFrameInfo.RZ = t.RZ;
                tankFrameInfo.TurretRY = t.TurretRY;
                tankFrameInfo.GunRX = t.GunRX;
                //createTanks.Tanks.Add(tankFrameInfo);


            }

            Log.Info("广播坦克");
        }
    }
}
