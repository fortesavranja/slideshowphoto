using PhotoSlideshow.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Diagnostics;

namespace PhotoSlideshow
{
    class Program
    {
        static void Main(string[] args)
        {
            Solution solution = new Solution();
            Random random = new Random();
            Stopwatch stopwatch = new Stopwatch();
            int timeToRun = 1;


            string[] files = Directory.GetFiles($"Samples", "*.txt");

            List<Slide> slides = new List<Slide>();
            Instance instance = Extensions.IO.ReadInput(files[4]);

            stopwatch.Start();

            //SolutionSlide slide = new SolutionSlide();
            //solution.GenerateSolution(instance.Photos, slide);
            //slide.InterestFactor = solution.CalculateInterestFactor(slide.Slides);
            //solution.HillClimbing(5000, slide);
            //solution.OutputFileGenerate($"{Path.GetFileNameWithoutExtension(files[2])}_result_{DateTime.Now.Ticks}.txt", slide);
            //Console.WriteLine($"Number of slides are: { slide.Slides.Count() } , Interest Factor is: { slide.InterestFactor }");

            solution.GenerateSolutionWithHeuristic(instance.Photos.OrderBy(x => x.Orientation).ThenBy(x => random.Next()).ToList(), stopwatch, timeToRun, 3000);
            SolutionSlide bestSolution = solution.ScatterSearch(instance.Photos, stopwatch, timeToRun);
            if (bestSolution.InterestFactor < 0)
            {
                bestSolution.InterestFactor = solution.CalculateInterestFactor(solution.Slides);
            }

            solution.OutputFileGenerate($"{Path.GetFileNameWithoutExtension(files[4])}_result_{DateTime.Now.Ticks}.txt", bestSolution);
            Console.WriteLine($"Number of slides are: { bestSolution.Slides.Count() } , Interest Factor is: { bestSolution.InterestFactor }");

            Console.ReadKey();
            stopwatch.Stop();
        }
    }
}
