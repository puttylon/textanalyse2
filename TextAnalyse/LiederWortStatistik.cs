﻿using System;
namespace TextAnalyse
{
    public class LiederWortStatistik
    {
        public LiederWortStatistik()
        {
        }

        public string Wort
        {
            get;
            set;
        }
        public int InWievielenLiedergefunden
        {
            get;
            set;
        }
        public int WieOftInDenTextenGefunden
        {
            get;
            set;
        }

        public void Dump()
        {
            Console.WriteLine(Wort + ' ' + InWievielenLiedergefunden + ' ' + WieOftInDenTextenGefunden);
        }
    }
}
