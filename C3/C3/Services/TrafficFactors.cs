namespace C3.Services
{
    public class TrafficFactors
    {
        private double[,] factors;
        public void Initialize(int size)
        {
            factors = new double[size, size];
            var rnd = new Random();
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    factors[i, j] = 0.9 + rnd.NextDouble() * 0.2; 
        }
        public void ApplyFactors(ref double[,] matrix)
        {
            int size = matrix.GetLength(0);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    matrix[i, j] *= factors[i, j];
        }
    }
}