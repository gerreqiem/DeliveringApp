using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace C3.Services
{
    public class VectorInfluence
    {
        private double[,] factors;
        public void Initialize(int size)
        {
            factors = new double[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    factors[i, j] = 1.0; 
        }
        public void SetFactor(int from, int to, double factor)
        {
            factors[from, to] = factor;
            factors[to, from] = factor; 
        }
        public void ApplyFactors(ref double[,] matrix)
        {
            int n = matrix.GetLength(0);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    matrix[i, j] *= factors[i, j];
        }
    }
}