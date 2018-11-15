using Xunit;

namespace NUnitToXunit.Tests.SetupTearDownTests
{
    public class SetupTearDownTests
    {
        [Theory]
        [InlineData(nameof(SetupTearDownTests), "ToConstructor")]
        [InlineData(nameof(SetupTearDownTests), "ToDisposable")]
        [InlineData(nameof(SetupTearDownTests), "ToDisposableWithExistingBaseClass")]
        public void Convert(string testCategory, string testCase) =>
            SyntaxSnapshot.RunSnapshotTest(testCategory, testCase);
    }
}
