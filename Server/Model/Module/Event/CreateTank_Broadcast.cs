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
                TankInfo tankInfo = new TankInfo();
                tankInfo.TankId = t.Id;
                tankInfo.PX = t.Position.x;
                tankInfo.PY = t.Position.y;
                tankInfo.PZ = t.Position.z;
                tankInfo.RX = t.Rotation.x;
                tankInfo.RY = t.Rotation.y;
                tankInfo.RZ = t.Rotation.z;
                tankInfo.TurretRY = t.TurretRY;
                tankInfo.GunRX = t.GunRX;
                createTanks.Tanks.Add(tankInfo);
            }

            Log.Info("广播坦克");
        }
    }
}
