using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parallelator.Client.Downloaders
{
    public interface IThingyLoader<TThingy>
    {
        Task<IEnumerable<TThingy>> LoadAsync(IEnumerable<Uri> uris);
    }
}
