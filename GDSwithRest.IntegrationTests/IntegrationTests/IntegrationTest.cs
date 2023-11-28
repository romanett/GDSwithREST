using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;

namespace GDSwithRest.Tests.IntegrationTests
{
    public class IntegrationTest : DockerComposeTestBase
    {
        [Fact]
        public async Task TestDatabaseMigration()
        {
            var index = await $"https://localhost:8081/".Wget();

            Assert.NotNull(index);
            Assert.Contains("opc.tcp", index);        
        }

        protected override ICompositeService Build()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(),
                (TemplateString) "docker-compose.yml");

            return new DockerComposeCompositeService(
                DockerHost,
                new DockerComposeConfig
                {
                    ComposeFilePath = new List<string> { file },
                    ForceRecreate = true,
                    RemoveOrphans = true,
                    StopOnDispose = true,
                    ComposeVersion = ComposeVersion.V2
                });
        }
    }
}
