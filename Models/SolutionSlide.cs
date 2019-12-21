using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoSlideshow.Models
{
    public class SolutionSlide
    {
        public List<Slide> Slides { get; set; }
        public int InterestFactor { get; set; } = int.MinValue;

        public SolutionSlide()
        {
            this.Slides = new List<Slide>();
        }

        public SolutionSlide(List<Slide> Slides)
        {
            this.Slides = Slides;
        }

    }
}
