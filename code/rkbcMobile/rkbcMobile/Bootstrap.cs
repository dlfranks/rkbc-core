using Autofac;
using Autofac.Core;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using rkbcMobile.Repository;
using rkbcMobile.ViewModels;
using rkbcMobile.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace rkbcMobile
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<ItemsViewModel>().AsSelf();
            builder.RegisterType<ItemDetailViewModel>().AsSelf();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            IContainer container = builder.Build();

            AutofacServiceLocator asl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => asl);
        }
    }
}
