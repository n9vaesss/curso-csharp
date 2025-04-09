using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exercicio3
{
    class Retangulo
    {
        public double Largura;
        public double Altura;

        public double Area()
        {
            return Altura * Largura;
        }

        public double Perimetro()
        {
            return 2 * (Altura + Largura);
        }

        public double Diagonal()
        {
            return Math.Sqrt(Math.Pow(Altura, 2) + Math.Pow(Largura, 2));
        }

        public override string ToString()
        {
            return "Area = " + Area() + "\n" +
                "Perímetro = " + Perimetro() + "\n" +
                "Diagonal = " + Diagonal();
        }
    }
}
