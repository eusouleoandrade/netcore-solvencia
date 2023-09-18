namespace TesteSolvencia
{
    class Program
    {
        public static void Main()
        {
            decimal limiteDoCliente = -5083.6M;

            bool habilitaEnvioDeOrdensPorLote = false;

            int limiteMaxDePapelPorEnvio = 50;

            var positions = GetPositions();

            Console.WriteLine();
            Console.WriteLine($"Limite do cliente: {limiteDoCliente}");
            Console.WriteLine();

            // Exibe todas as posições do cliente
            Console.WriteLine("Posições do cliente:");

            foreach (var position in positions)
                Console.WriteLine($"Instrument: {position.Instrument} - Quantity: {position.Quantity} - Value: {position.Value}");

            Console.WriteLine();
            Console.ReadKey();

            // Obtém as posições solvidas por BMF
            var positionsSolvedByBMF = GetPositionsSolvedByBMF();

            // Exibe os papéis já solvidos por BMF
            Console.WriteLine("Papéis já solvidos por BMF:");
            
            foreach (var position in positionsSolvedByBMF)
                Console.WriteLine($"Instrument: {position.Instrument}");

            Console.WriteLine();
            Console.ReadKey();

            // Filtro: filtra as posições para que possamos solver apenas as que já foram solvidas por BMF
            var positionsFilteredByBMFUsingAny = positions.Where(position => positionsSolvedByBMF.Any(bmfPosition => bmfPosition.Instrument == position.Instrument)).ToList();
            
            // Usando o filtro com Contains
            // var positionsFilteredByBMFUsingContains = positions.Where(position => positionsSolvedByBMF.Select(bmfPosition => bmfPosition.Instrument).Contains(position.Instrument)).ToList();

            // Filtro: Obtém as posições negativas
            // Ordenação: Efetua ordenação por valor do papel
            var negativePositions = positionsFilteredByBMFUsingAny.Where(positionsFilteredByBMF => positionsFilteredByBMF.Quantity < 0).OrderByDescending(o => o.Value).ToList();

            // Exibe todas as posições negativas do cliente
            Console.WriteLine("Posições negativas do cliente:");

            foreach (var position in negativePositions)
                Console.WriteLine($"Instrument: {position.Instrument} - Quantity: {position.Quantity} - Value: {position.Value}");

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine();
            Console.ReadKey();

            Console.WriteLine(">> INICIA A SOLVÊNCIA");
            Console.WriteLine();

            // Total de papéis enviados
            int quantidadeTotalDePapeisEnviados = 0;

            // Valor total de papéis enviadas
            decimal valorTotalDosPapeisEnviados = 0.0M;

            foreach (var position in negativePositions)
            {
                int quantidadeDePosicoesNegativas = position.Quantity;

                int numeroDaOrdemQueEstaSendoEnviada = 0;

                // Validação: Se o limte foir positivo não efetuar mais a solvência
                if (limiteDoCliente >= 0)
                    break;

                int quantidadeTotalPapeisParaEnviar = (int)Math.Min(Math.Abs(quantidadeDePosicoesNegativas), Math.Ceiling(Math.Abs(limiteDoCliente) / position.Value));

                if (habilitaEnvioDeOrdensPorLote)
                {
                    Console.WriteLine(position.Instrument);
                    Console.WriteLine();
                    Console.WriteLine($"Quantidade papéis {position.Instrument} antes da solvência: {position.Quantity}");
                    Console.WriteLine($"Quantidade de papéis {position.Instrument} que devem ser enviados: {quantidadeTotalPapeisParaEnviar}");
                    Console.WriteLine();

                    int quantidadeDePapeisEnviadosPorOrdem = 0;

                    while (quantidadeDePapeisEnviadosPorOrdem < quantidadeTotalPapeisParaEnviar)
                    {
                        int quantidadeDoLoteDePapeisParaEnviar = Math.Min(limiteMaxDePapelPorEnvio, quantidadeTotalPapeisParaEnviar - quantidadeDePapeisEnviadosPorOrdem);

                        decimal valorDoLoteDePapeisParaEnviar = quantidadeDoLoteDePapeisParaEnviar * position.Value;

                        numeroDaOrdemQueEstaSendoEnviada += 1;

                        Thread.Sleep(2000);
                        Console.WriteLine($"Enviando ordem {numeroDaOrdemQueEstaSendoEnviada} de compra de {quantidadeDoLoteDePapeisParaEnviar} ações {position.Instrument}.");
                        Console.WriteLine($"Cada ação {position.Instrument} vale R$ {position.Value}.");
                        Console.WriteLine($"Valor total R$ {valorDoLoteDePapeisParaEnviar}.");
                        Console.WriteLine();
                        Console.WriteLine("-----");
                        Console.WriteLine();

                        quantidadeTotalDePapeisEnviados += quantidadeDoLoteDePapeisParaEnviar;
                        limiteDoCliente += valorDoLoteDePapeisParaEnviar;
                        valorTotalDosPapeisEnviados += valorDoLoteDePapeisParaEnviar;

                        quantidadeDePapeisEnviadosPorOrdem += quantidadeDoLoteDePapeisParaEnviar;
                    }
                }
                else
                {
                    numeroDaOrdemQueEstaSendoEnviada += 1;

                    decimal valorTotalPapeis = quantidadeTotalPapeisParaEnviar * position.Value;

                    Console.WriteLine(position.Instrument);
                    Console.WriteLine();
                    Console.WriteLine($"Quantidade papéis {position.Instrument} antes da solvência: {position.Quantity}");
                    Console.WriteLine($"Quantidade de papéis {position.Instrument} que devem ser enviados: {quantidadeTotalPapeisParaEnviar}");
                    Console.WriteLine();

                    Thread.Sleep(2000);
                    Console.WriteLine($"Enviando ordem {numeroDaOrdemQueEstaSendoEnviada} de compra de {quantidadeTotalPapeisParaEnviar} ações {position.Instrument}.");
                    Console.WriteLine($"Cada ação {position.Instrument} vale R$ {position.Value}.");
                    Console.WriteLine($"Valor total R$ {valorTotalPapeis}.");
                    Console.WriteLine();
                    Console.WriteLine("-----");
                    Console.WriteLine();

                    quantidadeTotalDePapeisEnviados += quantidadeTotalPapeisParaEnviar;
                    limiteDoCliente += valorTotalPapeis;
                    valorTotalDosPapeisEnviados += valorTotalPapeis;
                }

                // Calcula a quantidade de ações que o papel fical após a solvência
                int quantidadeDePapeisDoClienteAposSolvencia = (Math.Abs(position.Quantity) == quantidadeTotalPapeisParaEnviar) ? quantidadeTotalPapeisParaEnviar : -(Math.Abs(position.Quantity) - quantidadeTotalPapeisParaEnviar);

                Console.WriteLine($"Quantidade do papel {position.Instrument} depois da solvência: {quantidadeDePapeisDoClienteAposSolvencia}");
                Console.WriteLine("Limite do cliente no momento: " + limiteDoCliente);
                Console.WriteLine();
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine();
            }

            Thread.Sleep(2000);
            Console.WriteLine((">> FINALIZA A SOLVÊNCIA").ToUpper());
            Console.WriteLine();
            Console.WriteLine($"Total de papéis enviados: {quantidadeTotalDePapeisEnviados}");
            Console.WriteLine($"Valor total enviado: {valorTotalDosPapeisEnviados}");
            Console.WriteLine($"Limite final cliente: {limiteDoCliente}");
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------");
        }

        private static IEnumerable<Position> GetPositions()
        {
            return new List<Position>
                {
                    new ("PETR4", -150, 33.89M),
                    new ("OIBR4", -300, 2.10M),
                    new ("TIMS3", 450, 14.87M),
                    new ("VALE3", -130, 69.54M)
                };
        }

        private static IEnumerable<Position> GetPositionsSolvedByBMF()
        {
            return new List<Position>{
                new ("VALE3", -130, 69.54M),
                new ("PETR4", -150, 33.89M)
            };
        }
    }

    public class Position
    {
        public string? Instrument { get; private set; }

        public int Quantity { get; private set; }

        public decimal Value { get; set; }

        public Position(string instrument, int quantity, decimal value)
        {
            Instrument = instrument;
            Quantity = quantity;
            Value = value;
        }
    }
}