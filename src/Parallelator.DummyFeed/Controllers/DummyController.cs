using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Parallelator.Common;

namespace Parallelator.DummyFeed.Controllers
{
    [Route("api/dummy")]
    public class DummyController : Controller
    {
        private readonly DummyBallast _ballast;

        public DummyController()
        {
            _ballast = BallastGenerator.CreateBallast();
        }

        [HttpGet("{delay:range(0,2000)}/{total:min(1)}/{current:min(0)}")]
        public async Task<DummyData> GetAsync(int delay, int total, int current)
        {
            await Task.Delay(delay);

            return new DummyData(delay, total, current)
            {
                Ballast = _ballast
            };
        }
    }
}
