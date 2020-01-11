using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PhotoSlideshow.Models
{
    public class Solution
    {
        public List<Slide> Slides { get; set; }
        public Solution()
        {
            this.Slides = new List<Slide>();
        }
        #region [Functions]

        public void HillClimbing(int numberOfIterations, SolutionSlide slide, Stopwatch stopwatch, int timeToRun)
        {
            
            Random random = new Random();
            List<int> randomNumbers = new List<int>();
            for (int i = 0; i < slide.Slides.Count(); i++)
            {
                randomNumbers.Add(i);
            }

            for (int i = 0; i < numberOfIterations && stopwatch.Elapsed.TotalMinutes < timeToRun; i++)
            {
                List<Slide> Solutiontmp = slide.Slides;
                int swapOrChange = random.Next(0, 9);
                List<int> slidesToSwap = Solutiontmp.Where(x => x.Photos.Count == 2).OrderBy(x => random.Next()).Select(x => x.Id).Take(2).ToList();

              
                if (swapOrChange < 3 && slidesToSwap.Count == 2)
                {
                    int firstSlidePhotoIndex = random.Next(0, 2);
                    int secondSlidePhotoIndex = random.Next(0, 2);

                    int firstSlideIndex = Solutiontmp.IndexOf(Solutiontmp.FirstOrDefault(x => x.Id == slidesToSwap.FirstOrDefault()));
                    int secondSlideIndex = Solutiontmp.IndexOf(Solutiontmp.FirstOrDefault(x => x.Id == slidesToSwap.LastOrDefault()));

                    List<Photo> firstSlidePhotos = new List<Photo>
                {
                    new Photo(Solutiontmp[firstSlideIndex].Photos.FirstOrDefault().Id, Orientation.V, new List<string>(Solutiontmp[firstSlideIndex].Photos.FirstOrDefault().Tags)),
                    new Photo(Solutiontmp[firstSlideIndex].Photos.LastOrDefault().Id, Orientation.V, new List<string>(Solutiontmp[firstSlideIndex].Photos.LastOrDefault().Tags))
                };
                    List<Photo> secondSlidePhotos = new List<Photo>
                {
                    new Photo(Solutiontmp[secondSlideIndex].Photos.FirstOrDefault().Id, Orientation.V, new List<string>(Solutiontmp[secondSlideIndex].Photos.FirstOrDefault().Tags)),
                    new Photo(Solutiontmp[secondSlideIndex].Photos.LastOrDefault().Id, Orientation.V, new List<string>(Solutiontmp[secondSlideIndex].Photos.LastOrDefault().Tags))
                };

                    Slide slideA = new Slide(Solutiontmp[firstSlideIndex].Id, firstSlidePhotos);
                    Slide slideB = new Slide(Solutiontmp[secondSlideIndex].Id, secondSlidePhotos);

                    slideA.Photos[firstSlidePhotoIndex] = Solutiontmp[secondSlideIndex].Photos[secondSlidePhotoIndex];
                    slideB.Photos[secondSlidePhotoIndex] = Solutiontmp[firstSlideIndex].Photos[firstSlidePhotoIndex];

                    Solutiontmp[firstSlideIndex] = slideA;
                    Solutiontmp[secondSlideIndex] = slideB;
                }

                else if (swapOrChange < 7)
                {
                    slidesToSwap = randomNumbers.OrderBy(x => random.Next()).Take(2).ToList();

                    Slide tempSlide = Solutiontmp[slidesToSwap.FirstOrDefault()];
                    Solutiontmp[slidesToSwap.FirstOrDefault()] = Solutiontmp[slidesToSwap.LastOrDefault()];
                    Solutiontmp[slidesToSwap.LastOrDefault()] = tempSlide;
                }
                else
                {
                    slidesToSwap = randomNumbers.OrderBy(x => random.Next()).Take(2).ToList();
                    Slide sld = Solutiontmp[slidesToSwap.FirstOrDefault()];
                    Solutiontmp.RemoveAt(slidesToSwap.FirstOrDefault());
                    Solutiontmp.Insert(slidesToSwap.LastOrDefault(), sld);
                }

                int currentInterestFactor = CalculateInterestFactor(Solutiontmp);
                if (currentInterestFactor > slide.InterestFactor)
                {
                    slide.Slides = Solutiontmp;
                    slide.InterestFactor = currentInterestFactor;
                }
            }
        }

        public SolutionSlide ScatterSearch(List<Photo> seeds, Stopwatch stopwatch, int timeToRun)
        {
            int initsize = 10;
            int t = 100;

            double temp = 50;
            double alpha = 0.999;
            double epsilon = 0.0001;

            SolutionSlide best = new SolutionSlide();
            List<SolutionSlide> P = new List<SolutionSlide>();
      


            for (int i = 0; i < initsize && temp < epsilon && stopwatch.Elapsed.TotalMinutes < timeToRun; i++)
            {
                P.Add(new SolutionSlide());
                RandomSolutionGenerate(seeds, P[i]);
                P[i].InterestFactor = CalculateInterestFactor(P[i].Slides);

                best.Slides = new List<Slide>(P.OrderByDescending(x => x.InterestFactor).FirstOrDefault().Slides);
                best.InterestFactor = P.OrderByDescending(x => x.InterestFactor).FirstOrDefault().InterestFactor;

                for (int j = 0; j < P.Count() && stopwatch.Elapsed.TotalMinutes < timeToRun; j++)
                {
                    int qualityS = P[j].InterestFactor;

                    HillClimbing(t, P[j], stopwatch, timeToRun);
                    P[j].InterestFactor = CalculateInterestFactor(P[j].Slides);

                    Random random = new Random();
                    double e = Math.Pow(Math.E, (P[j].InterestFactor - qualityS) / temp);

                    if (P[j].InterestFactor >= best.InterestFactor || random.NextDouble() < e / 2)
                    {
                        best = new SolutionSlide(P[j].Slides)
                        {
                            InterestFactor = P[j].InterestFactor
                        };
                    }
                }
                temp *= alpha;
            }
            return best;
        }

        public void RandomSolutionGenerate(List<Photo> photos, SolutionSlide slide)
        {
            int slideId = 0;
            Random random = new Random();
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
                
                slide.Slides.Add(new Slide(slideId, AddPhotos));
                slideId++;

            }
        }
        public void GenerateSolutionWithHeuristic(List<Photo> photos, Stopwatch stopwatch, int timeToRun, int divideNumber = 200)
        {
            Random random = new Random();

            int slideId = 0;
            int photosCount = photos.Count();

            int normalizedValue = (int)Math.Ceiling((decimal)photosCount / divideNumber);

            for (int i = 0; i < normalizedValue && stopwatch.Elapsed.TotalMinutes < timeToRun; i++)
            {
                List<Photo> tempPhotos = new List<Photo>(photos.Skip(i * divideNumber).Take(divideNumber));
           
                int tempPhotosCount = tempPhotos.Count();
                int iterationCount = 0;

                while (iterationCount  < tempPhotosCount && stopwatch.Elapsed.TotalMinutes < timeToRun)
                {
                    
                    Photo photo;
                   
                    if (iterationCount != 0 && i != 0)
                    {
                        photo = tempPhotos.OrderByDescending(x =>
                                            x.Tags.Where(t => !this.Slides.LastOrDefault().Tags.Contains(t)).Count() +
                                            x.Tags.Where(t => this.Slides.LastOrDefault().Tags.Contains(t)).Count() +
                                            this.Slides.LastOrDefault().Tags.Where(t => x.Tags.Contains(t)).Count())
                                        .FirstOrDefault();
                    }
                   
                    else
                    {
                        photo = tempPhotos.FirstOrDefault();
                    }

                    List<Photo> photosToAdd = new List<Photo>()
                    {
                        photo
                    };

                    if (photo.Orientation == Orientation.V)
                    {
                        Photo secondPhoto = tempPhotos
                            .Where(x => x.Id != photo.Id && x.Orientation.Equals(Orientation.V))
                            .OrderByDescending(x =>
                                x.Tags.Where(t => !photo.Tags.Contains(t)).Count() +
                                x.Tags.Where(t => photo.Tags.Contains(t)).Count() +
                                photo.Tags.Where(t => x.Tags.Contains(t)).Count())
                            .FirstOrDefault();
                        
                        if (secondPhoto != null)
                        {
                            photosToAdd.Add(secondPhoto);
                            tempPhotos.Remove(secondPhoto);

                            iterationCount++;
                        }
                    }
                    
                    this.Slides.Add(new Slide(slideId, photosToAdd));
                    tempPhotos.Remove(photo);

                    iterationCount++;
                    slideId++;
                }
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
