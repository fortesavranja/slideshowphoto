using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhotoSlideshow.Models
{
    public class Solution
    {
        #region [Functions]

        public void HillClimbing(int numberOfIterations, SolutionSlide slide)
        {
            Random random = new Random();
            List<int> randomNo = new List<int>();
            for (int i = 0; i < slide.Slides.Count(); i++)
            {
                randomNo.Add(i);
            }

            for (int i = 0; i < numberOfIterations; i++)
            {
                List<Slide> Solutiontmp = slide.Slides;
                List<int> SwapSlides = randomNo.OrderBy(x => random.Next()).Take(2).ToList();

                Slide tempSlide = Solutiontmp[SwapSlides.FirstOrDefault()];
                Solutiontmp[SwapSlides.FirstOrDefault()] = Solutiontmp[SwapSlides.LastOrDefault()];
                Solutiontmp[SwapSlides.LastOrDefault()] = tempSlide;

                int currentInterestFactor = CalculateInterestFactor(Solutiontmp);
                if (currentInterestFactor > slide.InterestFactor)
                {
                    slide.Slides = Solutiontmp;
                    slide.InterestFactor = currentInterestFactor;
                }
            }
        }

        public SolutionSlide ScatterSearch(List<Photo> seeds)
        {
            int initsize = 10;
            int t = 500;

            SolutionSlide best = new SolutionSlide();
            List<SolutionSlide> P = new List<SolutionSlide>();

            for (int i = 0; i < initsize; i++)
            {
                P.Add(new SolutionSlide());
                RandomSolutionGenerate(seeds, P[i]);
                P[i].InterestFactor = CalculateInterestFactor(P[i].Slides);

                best.Slides = new List<Slide>(P.OrderByDescending(x => x.InterestFactor).FirstOrDefault().Slides);
                best.InterestFactor = P.OrderByDescending(x => x.InterestFactor).FirstOrDefault().InterestFactor;

                for (int j = 0; j < P.Count(); j++)
                {
                    HillClimbing(t, P[j]);
                    P[j].InterestFactor = CalculateInterestFactor(P[j].Slides);
                    if (P[j].InterestFactor >= best.InterestFactor)
                    {
                        best = new SolutionSlide(P[j].Slides)
                        {
                            InterestFactor = P[j].InterestFactor
                        };
                    }
                }
            }

            return best;
        }

        public void RandomSolutionGenerate(List<Photo> photos, SolutionSlide slide)
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
                slide.Slides.Add(new Slide(AddPhotos));
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

        public void OutputFileGenerate(string filename, SolutionSlide slide)
        {
            using (StreamWriter file = new StreamWriter(new FileStream(filename, FileMode.CreateNew)))
            {
                file.WriteLine(slide.Slides.Count);
                foreach (Slide item in slide.Slides)
                {
                    file.WriteLine($"{string.Join(" ", item.Photos.Select(x => x.Id).ToList())}");
                }
            }
        }

        #endregion
    }
}
