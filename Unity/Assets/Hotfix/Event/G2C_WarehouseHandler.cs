using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    class G2C_WarehouseHandler:AMHandler<G2C_Warehouse>
    {
        protected override void Run(ETModel.Session session, G2C_Warehouse message)
        {
            WarehouseViewComponent.Data = message;
        }
    }
}
