using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Graph
{
	public interface IAssetsProvider
	{
		Stream Open(string asset);
	}
}
