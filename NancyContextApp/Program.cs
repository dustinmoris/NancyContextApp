using System;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.TinyIoc;

namespace NancyContextApp
{
    public interface IDependency
    {
        NancyContextWrapper ContextWrapper { get; }
    }

    public class Dependency : IDependency
    {
        public Dependency(NancyContextWrapper contextWrapper)
        {
            ContextWrapper = contextWrapper;
        }

        public NancyContextWrapper ContextWrapper { get; }
    }

    public class NancyContextWrapper
    {
        public NancyContextWrapper(NancyContext context)
        {
            Context = context;
        }

        public readonly NancyContext Context;
    }

    public class IndexModule : NancyModule
    {
        public IndexModule(IDependency dependency)
        {
            Get["/"] = _ => "Hello World" + dependency.ContextWrapper.Context.Request.Query["test"];
        }
    }

    public class DefaultBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register((c, p) => new NancyContextWrapper(context));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new NancyHost(new Uri("http://localhost:8888/")))
            {
                host.Start();

                Console.ReadLine();
            }
        }
    }
}
