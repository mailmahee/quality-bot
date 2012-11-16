namespace QualityBotDaemon
{
    using Ninject;
    using QualityBot;

    internal static class Kernel
    {
        private static IKernel _kernel;

        static Kernel()
        {
            _kernel = new StandardKernel();
            _kernel.Bind<IJobsDb>().To<MongoJobsDb>();
            _kernel.Bind<IService>().To<Service>();
        }

        public static T Get<T>()
        {
            return _kernel.Get<T>();
        }
    }
}