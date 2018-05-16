using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parallelator.Client.Loaders
{
    public interface IThingyLoader<TThingy>
    {
        Task<IEnumerable<TThingy>> LoadAsync(IEnumerable<Uri> uris);
    }
}
