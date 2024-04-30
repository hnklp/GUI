using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.Generator
{
    class Program
    {
        static unsafe void Main()
        {
            int number = 0;
            int* p = &number;

            while (*p < 50)
            {
                (*p)++;
            }
        }
    }
}
