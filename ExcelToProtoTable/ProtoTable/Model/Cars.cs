using ProtoBuf;

namespace ProtoTable.Model
{
    [ProtoContract]
    public class Cars : BaseProtoTable
    {
        [ProtoContract]
        public class d_Cars : BaseProtoTableData
        {
            [ProtoMember(1)]
            public uint Id { get; set; }

            [ProtoMember(2)]
            public string Name { get; set; }

            [ProtoMember(3)]
            public string Description { get; set; }

            [ProtoMember(4)]
            public uint Count { get; set; }
        }
    }
}
