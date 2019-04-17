namespace ETModel
{
    [ObjectSystem]
    public class TankGateComponentAwakeSystem : AwakeSystem<TankGateComponent, long>
    {
        public override void Awake(TankGateComponent self, long a)
        {
            self.Awake(a);
        }
    }

    public class TankGateComponent : Component, ISerializeToEntity
    {
        public long GateSessionActorId;

        public bool IsDisconnect;
        
        public void Awake(long gateSessionId)
        {
            this.GateSessionActorId = gateSessionId;
        }
    }
}