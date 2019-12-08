using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhotoSlideshow.Models
{
    public class Solution
    {
        public List<Slide> Slides { get; set; }
        public int InterestFactor { get; set; } = int.MinValue;

        public Solution()
        {
            this.Slides = new List<Slide>();
        }

        public Solution(List<Slide> Slides)
        {
            this.Slides = Slides;
        }

        #region [Functions]

        public void HillClimbing(int numberOfIterations)
        {
            Random random = new Random();
            List<int> randomNo = new List<int>();
            for (int i = 0; i < this.Slides.Count(); i++)
            {
                randomNo.Add(i);
            }

            for (int i = 0; i < numberOfIterations; i++)
            {
                List<Slide> Solutiontmp = this.Slides;
                List<int> SwapSlides = randomNo.OrderBy(x => random.Next()).Take(2).ToList();

                Slide tempSlide = Solutiontmp[SwapSlides.FirstOrDefault()];
                Solutiontmp[SwapSlides.FirstOrDefault()] = Solutiontmp[SwapSlides.LastOrDefault()];
                Solutiontmp[SwapSlides.LastOrDefault()] = tempSlide;

                int currentInterestFactor = CalculateInterestFactor(Solutiontmp);
                if (currentInterestFactor > this.InterestFactor)
                {
                    this.Slides = Solutiontmp;
                    this.InterestFactor = currentInterestFactor;
                }
            }
        }

        public void RandomSolutionGenerate(List<Photo> photos)
        {
            List<int> SkipPhotos = new List<int>();
            for (int i = 0; i < photos.Count; i++)
            {
                if (SkipPhotos.Any(x => x == photos[i].Id))
                {
                    continue;
                }

                List<Photo> AddPhotos = new List<Photo>()
                {
                    photos[i]
                };

                if (photos[i].Orientation == Orientation.V)
                {
                   
                    Photo secondphoto = photos.Skip(i + 1).Where(x => x.Orientation.Equals(Orientation.V) && !SkipPhotos.Contains(x.Id)).FirstOrDefault();

                    if (secondphoto != null)
                    {
                        AddPhotos.Add(secondphoto);
                        SkipPhotos.Add(secondphoto.Id);
                    }
                }
                this.Slides.Add(new Slide(AddPhotos));
            }
        }

        public int CalculateInterestFactor(List<Slide> slides)
        {
            int interestFactor = 0; 
            for (int i = 0; i < slides.Count - 1; i++)
            {
                int commonTags = CalculateCommonSlideTags(slides[i], slides[i + 1]);
                int slideAnotB = CalculateDifferenteSlideTags(slides[i], slides[i + 1]);
                int slideBnotA = CalculateDifferenteSlideTags(slides[i + 1], slides[i]);
                interestFactor += Math.Min(commonTags, Math.Min(slideAnotB, slideBnotA));
            }
            return interestFactor;
        }

        public int CalculateCommonSlideTags(Slide slideA, Slide slideB)
        {
            return slideA.Tags.Where(x => slideB.Tags.Contains(x)).Count();
        }

        public int CalculateDifferenteSlideTags(Slide slideA, Slide slideB)
        {
            return slideA.Tags.Where(x => !slideB.Tags.Contains(x)).Count();
        }

        public void OutputFileGenerate(string filename)
        {
            using (StreamWriter file = new StreamWriter(new FileStream(filename, FileMode.CreateNew)))
            {
                file.WriteLine(this.Slides.Count);
                foreach (Slide slide in this.Slides)
                {
                    file.WriteLine($"{string.Join(" ", slide.Photos.Select(x => x.Id).ToList())}");
                }
            }
        }

        #endregion
    }
}
