using Nelder_Mead_method;

namespace SimplexUI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            var settings = new Settings(1, 100);

            static double function(double[] point)
            {
                return 2 * Math.Pow(point[0], 4) + Math.Pow(point[1], 4) - Math.Pow(point[0], 2) - 2 * Math.Pow(point[0], 2);
            }

            double[] x0 = [0.1f, 0.5f];
            double[] x1 = [x0[0] + 2, x0[1]];
            double[] x2 = [x0[0], x0[1] + 2];

            var simplex = new Simplex(settings, [x0, x1, x2], function);

            for (int r = 0; r < settings.MaxIterations; r++)
            {
                simplex.Iteration();
                Console.WriteLine(simplex.GetBest);
            }
        }
    }
}