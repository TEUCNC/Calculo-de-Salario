using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculoIRRFComINSS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Exemplo de salário bruto
            decimal salarioBruto = 3000.00m;

            // Criação do objeto funcionário
            var funcionario = new Funcionario(salarioBruto);

            // Instanciamos a calculadora de IRRF
            var calculadoraIRRF = new CalculadoraIRRF();

            // Calculamos o desconto de INSS (exemplo fixo)
            decimal inss = calculadoraIRRF.CalculadoraINSS.CalcularDesconto(salarioBruto);

            // Calculamos o salário base para IRRF (salário bruto - INSS)
            decimal salarioBase = salarioBruto - inss;

            // Calculamos o valor do IRRF
            decimal irrf = calculadoraIRRF.Calcular(funcionario);

            // Exibimos os resultados no console
            Console.WriteLine($"Salário Bruto: {salarioBruto:C}");
            Console.WriteLine($"Desconto INSS: {inss:C}");
            Console.WriteLine($"Salário Base: {salarioBase:C}");
            Console.WriteLine($"Desconto IRRF: {irrf:C}");
        }
    }

    // Representa o funcionário com salário bruto
    public class Funcionario
    {
        public decimal SalarioBruto { get; }

        public Funcionario(decimal salarioBruto)
        {
            SalarioBruto = salarioBruto;
        }
    }

    // Classe responsável por calcular o desconto do INSS
    public class CalculadoraINSS
    {
        public decimal CalcularDesconto(decimal salarioBruto)
        {
            // Valor fixo de desconto de INSS para fins de exemplo
            // Substitua por cálculo real, se necessário
            return 258.83m;
        }
    }

    // Representa uma faixa da tabela IRRF
    public class FaixaIRRF
    {
        public decimal LimiteMin { get; }
        public decimal LimiteMax { get; }
        public decimal Aliquota { get; }
        public decimal Deducao { get; }

        public FaixaIRRF(decimal limiteMin, decimal limiteMax, decimal aliquota, decimal deducao)
        {
            LimiteMin = limiteMin;
            LimiteMax = limiteMax;
            Aliquota = aliquota;
            Deducao = deducao;
        }

        // Verifica se o salário base está dentro da faixa
        public bool EstaNaFaixa(decimal salarioBase)
        {
            return salarioBase >= LimiteMin && salarioBase <= LimiteMax;
        }
    }

    // Contém a lista de faixas do IRRF
    public class TabelaIRRF
    {
        // Lista com as faixas de IRRF baseadas nas regras atuais
        private readonly List<FaixaIRRF> _faixas = new List<FaixaIRRF>
        {
            new FaixaIRRF(0.00m, 2112.00m, 0.0m, 0.0m),
            new FaixaIRRF(2112.01m, 2826.65m, 0.075m, 169.44m),
            new FaixaIRRF(2826.66m, 3751.05m, 0.15m, 381.44m),
            new FaixaIRRF(3751.06m, 4664.68m, 0.225m, 662.77m),
            new FaixaIRRF(4664.69m, decimal.MaxValue, 0.275m, 896.00m)
        };

        // Retorna a faixa correspondente ao salário base
        public FaixaIRRF ObterFaixa(decimal salarioBase)
        {
            return _faixas.First(f => f.EstaNaFaixa(salarioBase));
        }
    }

    // Calcula o IRRF com base no salário base e nas faixas da tabela
    public class CalculadoraIRRF
    {
        public CalculadoraINSS CalculadoraINSS { get; }
        private readonly TabelaIRRF _tabelaIRRF;

        public CalculadoraIRRF()
        {
            CalculadoraINSS = new CalculadoraINSS();
            _tabelaIRRF = new TabelaIRRF();
        }

        // Método principal que calcula o IRRF do funcionário
        public decimal Calcular(Funcionario funcionario)
        {
            // Calcula o desconto do INSS
            decimal inss = CalculadoraINSS.CalcularDesconto(funcionario.SalarioBruto);

            // Calcula o salário base
            decimal salarioBase = funcionario.SalarioBruto - inss;

            // Obtém a faixa correspondente ao salário base
            var faixa = _tabelaIRRF.ObterFaixa(salarioBase);

            // Calcula o valor do IRRF
            decimal irrf = salarioBase * faixa.Aliquota - faixa.Deducao;

            // Garante que o IRRF nunca seja negativo
            return Math.Max(irrf, 0);
        }
    }
}
