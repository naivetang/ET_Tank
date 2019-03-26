using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.GMCreateTank)]
    public class GM_CreateTank : AEvent<long>
    {
        public override void Run(long id)
        {
            Tank tank = TankFactory.Create(id);

        }
    }
}
