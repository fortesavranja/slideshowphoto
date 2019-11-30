﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PhotoSlideshow.Models;

namespace PhotoSlideshow.Extensions
{
    public class IO
    {
        public static Instance ReadInput(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            Instance instance = new Instance();

            int i = 0;
            while (i < lines.Length)
              
            {
                if (i == 0)
                {
                    instance.NumberOfPhotos = int.Parse(lines[i]);
                }
                else
                {
                    string[] temp_params = lines[i].Split(' ');
                    Photo temp_photo = new Photo(i-1, (Orientation)Enum.Parse(typeof(Orientation), temp_params[0])
                        , new List<string>(temp_params.Skip(2)));

                    instance.Photos.Add(temp_photo);
                }
                i++;
            }


            return instance;
        }
    }
}
