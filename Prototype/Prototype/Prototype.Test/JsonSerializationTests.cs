using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Prototype.Test
{
    [TestClass]
    public class JsonSerializationTests
    {
        [TestMethod]
        public void CanSerializePrivateMembers()
        {
            GeographicLocation loc = new GeographicLocation();

            loc.Latitude.Should().NotBe(null);
            loc.Longitude.Should().NotBe(null);

            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(loc);

            serialized.Should().NotBeNullOrEmpty();

            var deserialized = JsonConvert.DeserializeObject<GeographicLocation>(serialized);

            deserialized.Should().NotBeNull();

            deserialized.ShouldBeEquivalentTo(loc);

            GeographicLocation loc2 = new GeographicLocation();

            const double expectLat = 33;
            const double expectLong = 22;

            loc2.Latitude = expectLat;
            loc2.Longitude = expectLong;

            serialized = Newtonsoft.Json.JsonConvert.SerializeObject(loc2);

            serialized.Should().NotBeNullOrEmpty();

            deserialized = JsonConvert.DeserializeObject<GeographicLocation>(serialized);

            deserialized.Should().NotBeNull();

            deserialized.ShouldBeEquivalentTo(loc2);
            deserialized.Latitude.Should().Be(expectLat);
            deserialized.Longitude.Should().Be(expectLong);
        }
    }
}
