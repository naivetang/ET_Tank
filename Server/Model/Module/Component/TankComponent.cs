using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ETModel
{
    public class TankComponent : Component
    {
        [BsonElement]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        private readonly Dictionary<long, Tank> idTanks = new Dictionary<long, Tank>();

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            this.ClearTanks();
        }

        public void ClearTanks()
        {
            foreach (Tank Tank in this.idTanks.Values)
            {
                Tank.Dispose();
            }
            this.idTanks.Clear();
        }

        public void Add(Tank Tank)
        {
            this.idTanks.Add(Tank.Id, Tank);
        }

        public Tank Get(long id)
        {
            this.idTanks.TryGetValue(id, out Tank Tank);
            return Tank;
        }

        public void Remove(long id)
        {
            Tank Tank;
            this.idTanks.TryGetValue(id, out Tank);
            this.idTanks.Remove(id);
            Tank?.Dispose();
        }

        public void RemoveNoDispose(long id)
        {
            this.idTanks.Remove(id);
        }

        public int Count
        {
            get
            {
                return this.idTanks.Count;
            }
        }

        public Tank[] GetAll()
        {
            return this.idTanks.Values.ToArray();
        }
    }
}