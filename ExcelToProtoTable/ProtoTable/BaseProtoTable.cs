using ProtoBuf;
using ProtoTable.Model;
using System.Collections.Generic;

namespace ProtoTable
{
	[ProtoContract] 
	[ProtoInclude(2, typeof(Cars))]
	[ProtoInclude(3, typeof(Food))]
	public class BaseProtoTable
	{
		[ProtoMember(1)]
		public List<BaseProtoTableData> Datas { get; set; } = new List<BaseProtoTableData>();
	}
}
