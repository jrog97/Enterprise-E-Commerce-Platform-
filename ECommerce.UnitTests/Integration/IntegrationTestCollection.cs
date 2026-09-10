using Xunit;

namespace ECommerce.UnitTests.Integration;

[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection
    : ICollectionFixture<IntegrationTestFactory>
{
}