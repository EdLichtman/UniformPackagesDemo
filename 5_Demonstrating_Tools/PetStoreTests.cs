using Demo.Tools;
using NUnit.Framework;
using Refit;
using Assert = NUnit.Framework.Assert;

namespace _5_Demonstrating_Tools
{
    [TestFixture]
    internal class PetStoreTests
    {
        [Test]
        public async Task CanFindOutHowMuchIsThatDoggieInTheWindow()
        {
            var client = RestService.For<ISwaggerPetstore>("https://petstore.swagger.io/v2");
            var availablePets = await client.FindPetsByStatus(new[] { Anonymous.Available });
            var theOneWithTheWagglyTail = availablePets.FirstOrDefault(x => x.Name.Equals("doggie", StringComparison.OrdinalIgnoreCase));
            Assert.That(theOneWithTheWagglyTail, Is.Not.Null);
        }

        [Test]
        public async Task MrSnuffleupagusIsNotForSale()
        {
            var client = RestService.For<ISwaggerPetstore>("https://petstore.swagger.io/v2");
            var availablePets = await client.FindPetsByStatus(new[] { Anonymous.Available });
            var bigBirdsFriend = availablePets.FirstOrDefault(x => x.Name.Equals("MrSnuffleupagus", StringComparison.OrdinalIgnoreCase));
            Assert.That(bigBirdsFriend, Is.Null);
        }
    }
}
