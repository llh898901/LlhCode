﻿using Dos.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMeng
{
    public class DB
    {
        public static readonly DbSession Context = new DbSession("UMeng");
    }
}
