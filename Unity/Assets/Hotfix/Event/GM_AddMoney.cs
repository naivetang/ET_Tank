using System;
using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.GMAddGold)]
    public class GM_AddMoney:AEvent<object>
    {
        public override void Run(object a)
        {
            Send_C2G_AddGold(a);
        }

        private void Send_C2G_AddGold(object add)
        {
            C2G_AddGold msg = new C2G_AddGold();

            msg.Add = Convert.ToInt32(add.ToString());

            ETModel.SessionComponent.Instance.Session.Send(msg);
        }
    }
}
