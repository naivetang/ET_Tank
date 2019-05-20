using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    class BulletComponent : Component
    {
        private readonly Dictionary<long, Bullet> idBullets = new Dictionary<long, Bullet>();


        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            foreach (Bullet Bullet in this.idBullets.Values)
            {
                Bullet.Dispose();
            }

            this.idBullets.Clear();
        }

        public void Add(Bullet Bullet)
        {
            this.idBullets.Add(Bullet.Id, Bullet);
        }

        public Bullet Get(long id)
        {
            Bullet Bullet;
            this.idBullets.TryGetValue(id, out Bullet);
            return Bullet;
        }

        public void Remove(long id)
        {
            Bullet Bullet;
            this.idBullets.TryGetValue(id, out Bullet);
            this.idBullets.Remove(id);
            Bullet?.Dispose();
        }

        public void RemoveNoDispose(long id)
        {
            this.idBullets.Remove(id);
        }

        public int Count
        {
            get
            {
                return this.idBullets.Count;
            }
        }

        public Bullet[] GetAll()
        {
            return this.idBullets.Values.ToArray();
        }
    }
}
