using ProtoBuf;
using ProtoTable.Model;

namespace ProtoTable
{
	[ProtoContract]
	[ProtoInclude(2, typeof(Cars.d_Cars))]
	[ProtoInclude(3, typeof(Food.d_Food))]
	public class BaseProtoTableData
	{
	}
}
