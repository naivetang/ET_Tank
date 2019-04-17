using Google.Protobuf.Collections;

namespace ETModel
{
    public partial class Room
    {
        public void Fill_G2C_RoomDetailInfo(ref G2C_RoomDetailInfo response)
        {
            if (response == null)
                response = new G2C_RoomDetailInfo();

            response.RoomId = this.Id;

            if (this.LeftCamp.Count > 0)
            {
                response.LeftCamp = new RepeatedField<RoomOnePeople>();

                foreach (RoomOnePeople value in this.LeftCamp.Values)
                {
                    RoomOnePeople one = new RoomOnePeople(value);

                    response.LeftCamp.Add(one);
                }
            }

            if (this.RightCamp.Count > 0)
            {
                response.RightCamp = new RepeatedField<RoomOnePeople>();

                foreach (RoomOnePeople value in this.RightCamp.Values)
                {
                    RoomOnePeople one = new RoomOnePeople(value);

                    response.RightCamp.Add(one);
                }
            }

            if (true)
            {
                response.RoomSimpleInfo = new RoomSimpleInfo();

                RoomSimpleInfo roomSimpleInfo = response.RoomSimpleInfo;

                roomSimpleInfo.RoomId = this.Id;

                roomSimpleInfo.PeopleNum = this.PeopleNum;

                roomSimpleInfo.MapId = this.MapTableId;

                roomSimpleInfo.BigModel = (int)this.BigModel;

                roomSimpleInfo.SmallModel = this.SmallMode;

                roomSimpleInfo.RoomName = this.RoomName;

                roomSimpleInfo.State = 1;

                roomSimpleInfo.SerialNumber = this.SerialNumber;

                roomSimpleInfo.RoomOwnerId = this.OwnerId;
            }
        }

        public void BroadcastRoomDetailInfo()
        {
            G2C_RoomDetailInfo message = new G2C_RoomDetailInfo();

            Fill_G2C_RoomDetailInfo(ref message);

            Player[] players = this.GetAll();

            foreach (Player player in players)
            {
                player.Session.Send(message);
            }
        }

        public void BroadCastStartGame()
        {
            G2C_StartGame message = new G2C_StartGame();

            message.RoomId = this.Id;

            Player[] players = this.GetAll();

            foreach (Player player in players)
            {
                player.Session.Send(message);
            }
        }
    }
}
