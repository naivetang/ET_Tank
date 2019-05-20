using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;

namespace ETModel
{
    [ObjectSystem]
    public class BattleAwakeSystem: AwakeSystem<BattleComponent>
    {
        public override void Awake(BattleComponent self)
        {
            self.Awake();
        }
    }

    public class BattleComponent: Component
    {
        public static BattleComponent Instance { get; private set; }

        private readonly Dictionary<long, Battle> idBattles = new Dictionary<long, Battle>();

        public void Awake()
        {
            Instance = this;
        }

        public void Add(Battle battle)
        {
            this.idBattles.Add(battle.Id, battle);
        }

        public Battle Get(long id)
        {
            this.idBattles.TryGetValue(id, out Battle battle);
            return battle;
        }

        public void Remove(long id)
        {
            this.idBattles.Remove(id);
        }

        public int Count
        {
            get
            {
                return this.idBattles.Count;
            }
        }

        public Battle[] GetAll()
        {
            return this.idBattles.Values.ToArray();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            foreach (Battle battle in this.idBattles.Values)
            {
                battle.Dispose();
            }

            base.Dispose();
        }
    }
}