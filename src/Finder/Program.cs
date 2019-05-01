using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Finder
{
    class Program
    {
        private static bool F { get; set; } = true;

        public static void Main(string[] args)
        {
            Console.WriteLine("Olá, iniciando o Finder!");
            Console.WriteLine("Irei procurar um valor em arquivos CSV ou TXT. OK?");

            var pasta = "";
            while (string.IsNullOrWhiteSpace(pasta))
            {
                Console.WriteLine("Qual é a pasta?");
                pasta = Console.ReadLine();
            }

            while (F)
                Finder(pasta);
        }

        private static void Finder(string pasta)
        {
            try
            {
                F = false;
                Console.WriteLine("Qual é o valor procurado?");

                var valor = Console.ReadLine()?.ToLowerInvariant().Replace(Environment.NewLine, "") ?? "";
                var valores = new List<LinhaValue>();

                if ((valor.EndsWith(".txt") || valor.EndsWith(".csv")) && File.Exists(valor))
                {
                    using (var fileStream = new FileStream(valor, FileMode.Open))
                    using (var streamReader = new StreamReader(fileStream))
                    {
                        string linha = null;
                        do
                        {
                            linha = streamReader.ReadLine();
                            if (!string.IsNullOrWhiteSpace(linha))
                                valores.Add(new LinhaValue(linha));
                        }
                        while (linha != null);
                    }
                }


                var itens = Directory.GetFiles(pasta).ToList();


                Console.WriteLine($"Procurando no diretório: {pasta}");
                Console.WriteLine($"Existem {itens.Count} arquivos na sua pasta!");

                itens = itens.Where(c =>
                    c.ToLowerInvariant().Contains(".csv") || c.ToLowerInvariant().Contains(".txt")).ToList();

                Console.WriteLine($"Existem {itens.Count} arquivos CSV ou TXT!");

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var encontrado = false;
                foreach (var item in itens)
                {
                    try
                    {
                        var fileName = Path.GetFileName(item);

                        using (var fileStream = new FileStream(item, FileMode.Open))
                        using (var streamReader = new StreamReader(fileStream))
                        {
                            long numeroLinha = 1;
                            string linha;

                            do
                            {
                                linha = streamReader.ReadLine();
                                try
                                {
                                    if (valores.Any())
                                    {
                                        foreach (var val in valores)
                                        {
                                            if (linha?.Contains(val.Key) ?? false)
                                            {
                                                Console.WriteLine("");
                                                Console.WriteLine("Atenção!");
                                                Console.WriteLine($"Encontrei o valor: {val.Key}");
                                                Console.WriteLine($"Encontrei o valor no arquivo: {fileName}");
                                                Console.WriteLine($"Encontrei o valor na linha de Nº {numeroLinha}. Linha: {linha}. ");
                                                val.SetValue(fileName);
                                                Console.WriteLine("");
                                                encontrado = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrWhiteSpace(valor) && (linha?.Contains(valor) ?? false))
                                        {
                                            Console.WriteLine("");
                                            Console.WriteLine("Atenção!");
                                            Console.WriteLine($"Encontrei o valor no arquivo: {fileName}");
                                            Console.WriteLine($"Encontrei o valor na linha de Nº {numeroLinha}. Linha: {linha}. ");
                                            Console.WriteLine("");
                                            encontrado = true;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Não foi possível processar a linha de Nº {numeroLinha}. Linha: {linha}. Erro: {ex.Message}");
                                }

                                numeroLinha++;

                            } while (linha != null);
                        }
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Não foi possível ler o arquivo: {item}, Motivo: {ex.Message}");
                    }
                }

                if ((valor.EndsWith(".txt") || valor.EndsWith(".csv")) && File.Exists(valor) && valores.Any())
                {
                    var retorno = valor.Replace(".txt", $"_retorno_{DateTime.Now.Ticks}.txt");

                    Console.WriteLine($"Escrevendo o arquivo de retorno: {retorno}");

                    using (var fileStream = new FileStream(retorno, FileMode.CreateNew))
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        foreach (var item in valores)
                            streamWriter.WriteLine($"{item.Key} = {string.Join(" | ", item.Value.Distinct())}");
                        streamWriter.Close();
                    }

                    Console.WriteLine($"Retorno finalizado: {retorno}");
                }

                stopWatch.Stop();

                Console.WriteLine(encontrado ? "Valor encontrado!" : "Valor não encontrado!");
                Console.WriteLine($"Tempo total da pesquisa: {stopWatch.ElapsedMilliseconds}/milissegundos");
                Console.WriteLine("Fim!");
                Console.WriteLine("");
                Console.WriteLine("Deseja sair? Pressione ESC | Qualquer outra tecla para procurar novamente!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var procurar = Console.ReadKey();
            F = procurar.Key != ConsoleKey.Escape;
            Console.Clear();
        }
    }

    public class LinhaValue
    {
        public LinhaValue()
        {

        }

        public LinhaValue(string key)
        {
            Key = key;
        }

        public void SetValue(string value) => Value.Add(value);

        public string Key { get; set; }
        public List<string> Value { get; set; } = new List<string>();
    }
}
