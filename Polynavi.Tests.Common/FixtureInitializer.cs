using AutoFixture;
using System.Linq;

namespace Polynavi.Tests.Common
{
    public static class FixtureInitializer
    {
        public static Fixture InitializeFixture()
        {
            var fixture = new Fixture();

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}
