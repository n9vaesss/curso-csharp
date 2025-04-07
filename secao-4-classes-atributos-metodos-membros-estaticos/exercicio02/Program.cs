
using exercicio02;
using System.Globalization;

Funcionario f1 = new Funcionario();
Funcionario f2 = new Funcionario();

Console.WriteLine("Dados do primeiro funcionário: ");
f1.Nome = Console.ReadLine();
f1.Salario = double.Parse(Console.ReadLine(),CultureInfo.InvariantCulture);

Console.WriteLine("Dados do segundo funcionário: ");
f2.Nome = Console.ReadLine();
f2.Salario = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

double mediaSalarios = (f1.Salario + f2.Salario) / 2;

Console.WriteLine("Salário médio = " + mediaSalarios.ToString("F2", CultureInfo.InvariantCulture));
