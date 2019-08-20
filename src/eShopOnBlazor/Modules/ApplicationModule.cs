using Autofac;
using eShopOnBlazor.Models;
using eShopOnBlazor.Models.Infrastructure;
using eShopOnBlazor.Services;

namespace eShopOnBlazor.Modules
{
    public class ApplicationModule : Module
    {
        private bool useMockData;

        public ApplicationModule(bool useMockData)
        {
            this.useMockData = useMockData;
        }
        protected override void Load(ContainerBuilder builder)
        {
            if (this.useMockData)
            {
                builder.RegisterType<CatalogServiceMock>()
                    .As<ICatalogService>()
                    .SingleInstance();
            }
            else
            {
                builder.RegisterType<CatalogService>()
                    .As<ICatalogService>()
                    .InstancePerLifetimeScope();
            }

            builder.RegisterType<CatalogDBContext>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CatalogDBInitializer>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CatalogItemHiLoGenerator>()
                .SingleInstance();
        }
    }
}