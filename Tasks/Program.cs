﻿using BulletinHub.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Current.Run();

            //TaskGenerator.GenerateTasks();
        }
    }
}