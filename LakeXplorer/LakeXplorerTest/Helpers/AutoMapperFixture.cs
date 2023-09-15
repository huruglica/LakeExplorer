using AutoMapper;
using LakeXplorer.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace LakeXplorerTest.Helpers
{
    public class AutoMapperFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public AutoMapperFixture()
        {
            var services = new ServiceCollection();

            var mapperConfiguration = new MapperConfiguration(
                mc => mc.AddProfile(new AutoMapperConfiguration()));

            IMapper mapper = mapperConfiguration.CreateMapper();

            services.AddSingleton(mapper);

            ServiceProvider = services.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
