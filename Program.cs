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

            //SolutionSlide slide = new SolutionSlide();
            //solution.RandomSolutionGenerate(instance.Photos, slide);
            //slide.InterestFactor = solution.CalculateInterestFactor(slide.Slides);
            //solution.HillClimbing(5000, slide);
            //solution.OutputFileGenerate($"{Path.GetFileNameWithoutExtension(files[2])}_result_{DateTime.Now.Ticks}.txt", slide);
            //Console.WriteLine($"Number of slides are: { slide.Slides.Count() } , Interest Factor is: { slide.InterestFactor }");


            SolutionSlide bestSolution = solution.ScatterSearch(instance.Photos);
            solution.OutputFileGenerate($"{Path.GetFileNameWithoutExtension(files[2])}_result_{DateTime.Now.Ticks}.txt", bestSolution);
            Console.WriteLine($"Number of slides are: { bestSolution.Slides.Count() } , Interest Factor is: { bestSolution.InterestFactor }");


            Console.ReadKey();
        }
    }
}
