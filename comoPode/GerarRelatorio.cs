using iTextSharp.text;
using iTextSharp.text.pdf;
using OxyPlot;
using System.Data.SqlClient;
using Font = iTextSharp.text.Font;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using Image = iTextSharp.text.Image;
using Element = iTextSharp.text.Element;

namespace comoPode
{
    public class RelatorioDespesas
    {
        public static void GerarRelatorio()
        {
            string connectionString = "Data Source=DESKTOP-7092F7U;Initial Catalog=SALARY_SYNC;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                decimal cotaMensal = ConsultarCotaMensal(connection);

                string queryPago = "SELECT Nome, Cargo, Salario, StatusPagamento FROM tbl_funcionario WHERE StatusPagamento = 'pago'";
                string queryNovo = "SELECT Nome, Cargo, Salario, StatusPagamento FROM tbl_funcionario WHERE StatusPagamento = 'novo'";

                decimal totalSalariosPagos = 0M;
                decimal totalSalariosNovos = 0M;
                decimal totalBeneficios = 0M;
                decimal gastosComunsTotais = 0M;

                using (SqlCommand commandPago = new SqlCommand(queryPago, connection))
                using (SqlDataReader readerPago = commandPago.ExecuteReader())
                {
                    while (readerPago.Read())
                    {
                        string nomeFuncionario = readerPago["Nome"].ToString();
                        string cargoFuncionario = readerPago["Cargo"].ToString();
                        decimal salario = Convert.ToDecimal(readerPago["Salario"]);
                        totalSalariosPagos += salario;
                        decimal beneficios = CalcularBeneficios(salario);
                        totalBeneficios += beneficios;
                        decimal gastosComuns = CalcularGastosComuns(salario);
                        decimal gastoTotal = salario + beneficios + gastosComuns;
                    }
                }

                using (SqlCommand commandNovo = new SqlCommand(queryNovo, connection))
                using (SqlDataReader readerNovo = commandNovo.ExecuteReader())
                {
                    while (readerNovo.Read())
                    {
                        string nomeFuncionario = readerNovo["Nome"].ToString();
                        string cargoFuncionario = readerNovo["Cargo"].ToString();
                        decimal salario = Convert.ToDecimal(readerNovo["Salario"]);
                        totalSalariosNovos += salario;
                    }
                }

                gastosComunsTotais = totalSalariosPagos + totalSalariosNovos + totalBeneficios;

                Document document = new Document();
                string downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\Relatorio.pdf";
                string dataHoraAtual = DateTime.Now.ToString("yyyyMMddHHmmss");
                string nomeArquivo = $"Relatorio_{dataHoraAtual}.pdf";

                // Combine o nome do arquivo com o caminho de download
                string pdfFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", nomeArquivo);

                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfFilePath, FileMode.Create));

                document.Open();

                Font font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Paragraph header = new Paragraph();
                header.Add(new Chunk("RX8Pay", font));
                header.Add(Chunk.NEWLINE);
                font = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                header.Add(new Chunk($"Data de Emissão: {DateTime.Now.ToString("dd/MM/yyyy")}", font));
                header.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                document.Add(Chunk.NEWLINE);

                document.Add(header);

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Paragraph title = new Paragraph("Relatório de Despesas", titleFont);
                title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                document.Add(title);
                document.Add(Chunk.NEWLINE);

                PdfPTable table = new PdfPTable(2);

                table.AddCell(new PdfPCell(new Phrase("Valor da cota mensal:", FontFactory.GetFont(FontFactory.HELVETICA, 12))));
                table.AddCell(new PdfPCell(new Phrase(cotaMensal.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 12))));

                table.AddCell(new PdfPCell(new Phrase("Total gasto com salários (pago):", FontFactory.GetFont(FontFactory.HELVETICA, 12))));
                table.AddCell(new PdfPCell(new Phrase(totalSalariosPagos.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 12))));

                table.AddCell(new PdfPCell(new Phrase("Total gasto com salários (novo):", FontFactory.GetFont(FontFactory.HELVETICA, 12))));
                table.AddCell(new PdfPCell(new Phrase(totalSalariosNovos.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 12))));

                table.AddCell(new PdfPCell(new Phrase("Total gasto com benefícios (pago):", FontFactory.GetFont(FontFactory.HELVETICA, 12))));
                table.AddCell(new PdfPCell(new Phrase(totalBeneficios.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 12))));

                table.AddCell(new PdfPCell(new Phrase("Total gastos/descontos da empresa:", FontFactory.GetFont(FontFactory.HELVETICA, 12))));
                table.AddCell(new PdfPCell(new Phrase(gastosComunsTotais.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 12))));

                table.AddCell(new PdfPCell(new Phrase("Valor restante da cota mensal:", FontFactory.GetFont(FontFactory.HELVETICA, 12))));
                table.AddCell(new PdfPCell(new Phrase((cotaMensal - gastosComunsTotais).ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 12))));

                document.Add(table);

                document.Add(new Chunk("Previsão de Gastos", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)));
                document.Add(Chunk.NEWLINE);


                var plotModel = new PlotModel();
                var series = new ColumnSeries
                {
                    ItemsSource = new[]
                    {
                        new ColumnItem((double)totalSalariosPagos, 0) { Color = OxyColor.Parse("#0000ff") },
                        new ColumnItem((double)totalSalariosNovos, 1) { Color = OxyColor.Parse("#00ff00") },
                        new ColumnItem((double)(totalSalariosPagos + totalSalariosNovos), 2) { Color = OxyColor.Parse("#ff0000") }
                    },
                    LabelPlacement = LabelPlacement.Inside,
                    LabelFormatString = "{0:N2}",
                };

                plotModel.Series.Add(series);

                // Aumentar a resolução da imagem exportada para obter uma melhor qualidade
                var pngExporter = new PngExporter { Width = 600, Height = 400 }; // Ajuste os valores conforme necessário para obter alta definição
                string imagePath = Path.GetTempFileName() + ".png";
                pngExporter.ExportToFile(plotModel, imagePath);

                // Redimensionar a imagem ao adicioná-la ao documento PDF
                var chartImage = Image.GetInstance(imagePath);
                chartImage.ScaleToFit(300f, 200f); // Redimensionar a imagem para o tamanho desejado no PDF
                chartImage.Alignment = Element.ALIGN_CENTER;
                document.Add(chartImage);


                // Excluir o arquivo de imagem temporário
                File.Delete(imagePath);

                Paragraph legenda = new Paragraph();
                legenda.Alignment = Element.ALIGN_CENTER;

                Font legendaFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

                // Adicione os textos da legenda com cores e fontes especificadas
                legenda.Add(new Chunk("Legenda: ", legendaFont));
                legenda.Add(new Chunk("(Azul) Salários Pagos - ", new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8))));
                legenda.Add(new Chunk("(Verde) Salários Novos - ", new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8))));
                legenda.Add(new Chunk("(Vermelho) Gastos Totais", new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8))));

                // Adicione a legenda ao documento
                document.Add(legenda);

                // Calcule o aumento em porcentagem do gasto futuro da cota mensal em relação à cota mensal atual
                decimal gastoFuturoCotaMensal = totalSalariosPagos + totalSalariosNovos + totalBeneficios + gastosComunsTotais;
                decimal restanteOrcamento = cotaMensal - totalSalariosPagos - totalBeneficios - gastosComunsTotais;
                decimal porcentagemSalariosNovos = (totalSalariosNovos / restanteOrcamento) * 100;
                document.Add(Chunk.NEWLINE);

                document.Add(new Chunk($"É esperado um aumento de {porcentagemSalariosNovos:N2}% em relação aos gastos anteriores.", FontFactory.GetFont(FontFactory.HELVETICA, 12)));
                document.Add(Chunk.NEWLINE);
                document.Add(Chunk.NEWLINE);


                document.Add(new Chunk("Distribuição de Gastos", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)));

                var plotModel1 = new PlotModel();
                var pieSeries = new PieSeries
                {
                    Stroke = OxyColors.White,
                    StrokeThickness = 2.0,
                    InsideLabelPosition = 0.9,
                    AngleSpan = 360,
                    FontSize = 14,
                };


                // Consulte o banco de dados para obter a distribuição de cargos e salários
                using (SqlCommand command = new SqlCommand("SELECT Cargo, SUM(Salario) AS TotalSalario FROM tbl_funcionario GROUP BY Cargo", connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string cargo = reader["Cargo"].ToString();
                        decimal totalSalario = Convert.ToDecimal(reader["TotalSalario"]);

                        pieSeries.Slices.Add(new PieSlice(cargo, (double)totalSalario));
                    }
                }

                plotModel1.Series.Add(pieSeries);

                // Salvar o gráfico como uma imagem temporária
                var pngExporter1 = new PngExporter { Width = 1200, Height = 800 }; // Ajuste os valores conforme necessário para obter alta definição
                string imagePath1 = Path.GetTempFileName() + ".png";
                pngExporter1.ExportToFile(plotModel1, imagePath1);

                // Redimensionar a imagem ao adicioná-la ao documento PDF
                var chartImage1 = Image.GetInstance(imagePath1);
                chartImage1.ScaleToFit(400f, 300f); // Redimensionar a imagem para o tamanho desejado no PDF
                chartImage1.Alignment = Element.ALIGN_CENTER;
                document.Add(chartImage1);

                document.Close();
            }
        }

        private static decimal ConsultarCotaMensal(SqlConnection connection) //obter cota mensal
        {
            string cotaQuery = "SELECT Valor FROM cotamensal";
            using (SqlCommand cotaCommand = new SqlCommand(cotaQuery, connection))
            {
                object result = cotaCommand.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToDecimal(result);
                }
                return 0M;
            }
        }

        private static decimal CalcularBeneficios(decimal salario)
        {
            // Implemente a lógica para calcular os benefícios aqui
            decimal dsr = salario * 0.05M;
            decimal valeTransporte = salario * 0.06M;
            decimal valeRefeicao = salario * 0.08M;
            decimal seguroDeVida = 150M;
            return dsr + valeTransporte + valeRefeicao + seguroDeVida;
        }

        private static decimal CalcularGastosComuns(decimal salario)
        {
            // Implemente a lógica para calcular os gastos/descontos comuns da empresa aqui
            // Exemplo: Impostos, FGTS, etc.
            decimal percentualFGTS = salario * 0.08m;
            return percentualFGTS;
        }


    }
}
