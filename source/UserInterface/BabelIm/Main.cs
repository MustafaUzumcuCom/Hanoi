namespace BabelIm
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    public static class ChronosMain
    {
        [STAThread]
        public static void Main(params string[] args)
        {
            if (SingleInstance.InitializeAsFirstInstance("BabelIM"))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance.Cleanup();
            }
        }
    }
}