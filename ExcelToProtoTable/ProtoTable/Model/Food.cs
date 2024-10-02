using ProtoBuf;

namespace ProtoTable.Model
{
    [ProtoContract]
    public class Food : BaseProtoTable
    {
        [ProtoContract]
        public class d_Food : BaseProtoTableData
        {
            [ProtoMember(1)]
            public uint Id { get; set; }

            [ProtoMember(2)]
            public string Name { get; set; }

            [ProtoMember(3)]
            public uint Count { get; set; }
        }
    }
}
