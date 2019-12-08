using PhotoSlideshow.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace PhotoSlideshow
{
    class Program
    {
        static void Main(string[] args)
        {
           

            Solution solution = new Solution();
            Random random = new Random();

            string[] files = Directory.GetFiles($"Samples", "*.txt");

            List<Slide> slides = new List<Slide>();
            Instance instance = Extensions.IO.ReadInput(files[2]);

            solution.RandomSolutionGenerate(instance.Photos); 
            solution.InterestFactor = solution.CalculateInterestFactor(solution.Slides);
            solution.HillClimbing(500);
            solution.OutputFileGenerate($"{Path.GetFileNameWithoutExtension(files[2])}_result_{DateTime.Now.Ticks}.txt");

            Console.WriteLine($"Number of slides are: { solution.Slides.Count() } , Interest Factor is: { solution.InterestFactor }");
            Console.ReadKey();
        }
    }
}
