﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispetcher.Common.Tasks
{
    public interface ITask
    {
        void Start();
        void Stop();
    }
}
