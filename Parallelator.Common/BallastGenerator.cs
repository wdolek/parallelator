using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Parallelator.Common
{
    public static class BallastGenerator
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public static DummyBallast CreateBallast()
        {
            using (Stream s = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Parallelator.Common.Resources.ballast.json"))
            using (var sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
                return Serializer.Deserialize<DummyBallast>(reader);
        }
    }
}