

using exemplo02;
using System.Globalization;

Produto produto = new Produto();

Console.WriteLine("Entre com os dados do produto:");

Console.Write("Nome: ");
produto.Nome = Console.ReadLine();

Console.Write("Preço: ");
produto.Preco = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

Console.Write("Quantidade no estoque: ");
produto.Quantidade = int.Parse(Console.ReadLine());

Console.WriteLine("Dados do produto: " + produto);

Console.Write("Digite o número de produtos a ser adicionado ao estoque: ");
int quantidade = int.Parse(Console.ReadLine());

produto.AdicionarProdutos(quantidade);

Console.WriteLine("Dados do produto: " + produto);

Console.Write("Digite o número de produtos a ser removido ao estoque: ");
quantidade = int.Parse(Console.ReadLine());

produto.RemoverProdutos(quantidade);

Console.WriteLine("Dados do produto: " + produto);




