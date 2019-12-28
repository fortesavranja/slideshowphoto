﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoSlideshow.Models
{
    public class Slide
    {
        public Slide(int id, List<Photo> photos)
        {
            Id = id;
            Photos = photos;
        }

        public int Id { get; set; }
        public List<Photo> Photos { get; set; }
        public List<string> Tags
        {
            get
            {
                return Photos.SelectMany(x => x.Tags).ToList();
            }
        }

        public bool IsValid
        {
            get
            {
                
                if (Photos.Count == 2)
                {
                    return Photos.All(x => x.Orientation == Orientation.V);
                }

                return true;
            }
        }
    }
}
