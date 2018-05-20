using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parallelator.Loaders
{
    /// <summary>
    /// Thingy loader.
    /// </summary>
    /// <typeparam name="TThingy">Type of thingy.</typeparam>
    public interface IThingyLoader<TThingy>
    {
        /// <summary>
        /// Load thingy asynchronously.
        /// </summary>
        /// <param name="uris">URIs of thingies.</param>
        /// <returns>
        /// Enumerable of thingies.
        /// </returns>
        Task<IEnumerable<TThingy>> LoadAsync(IEnumerable<Uri> uris);
    }
}
