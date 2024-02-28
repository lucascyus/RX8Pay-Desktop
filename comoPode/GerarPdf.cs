using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace comoPode
{
    public class GerarPdf
    {
        public static void GerarPDFs()
        {
            string connectionString = "Data Source=DESKTOP-7092F7U;Initial Catalog=SALARY_SYNC;Integrated Security=True";
            string query = "SELECT Nome, Cargo, Salario FROM tbl_funcionario";
            string pdfFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PDFs");

            if (!Directory.Exists(pdfFolderPath))
            {
                Directory.CreateDirectory(pdfFolderPath);
            }

            List<string> pdfFiles = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nomeFuncionario = reader["Nome"].ToString();
                        string cargoFuncionario = reader["Cargo"].ToString();
                        decimal salario = Convert.ToDecimal(reader["Salario"]);

                        string pdfFilePath = Path.Combine(pdfFolderPath, $"{nomeFuncionario}_Demonstrativo.pdf");
                        pdfFiles.Add(pdfFilePath);

                        CriarPDF(nomeFuncionario, cargoFuncionario, salario, pdfFilePath);
                    }
                }
            }

            CompactarPDFs(pdfFiles);

            Console.WriteLine("PDFs exportados com sucesso!");
        }

        static void CriarPDF(string nomeFuncionario, string cargoFuncionario, decimal salario, string pdfFilePath)
        {
            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont fontTitulo = new XFont("Arial", 16, XFontStyle.Bold);
                XFont fontNormal = new XFont("Arial", 12);

                gfx.DrawString("Demonstrativo de Pagamento", fontTitulo, XBrushes.Black, new XRect(50, 50, page.Width.Point - 100, 20), XStringFormats.TopCenter);

                DrawText(gfx, fontNormal, $"{nomeFuncionario}", 50, 80, page.Width.Point - 100, XStringFormats.TopCenter);
                DrawText(gfx, fontNormal, $"{cargoFuncionario}", 50, 100, page.Width.Point - 100, XStringFormats.TopCenter);

                decimal dsr = salario * 0.05M;
                decimal valeTransporte = salario * 0.06M;
                decimal valeRefeicao = salario * 0.08M;
                decimal seguroDeVida = 150M;
                decimal inss = CalcularINSS(salario);
                decimal percentualFGTS = salario * 0.08M;
                decimal baseFGTS = salario;
                decimal baseCalculoIRRF = salario - inss;

                DrawTable(gfx, fontNormal, 50, 150, page.Width.Point - 100, new string[] { "Descrição", "Valor (R$)" },
                    new string[] { "DSR", dsr.ToString("F2") },
                    new string[] { "Vale Transporte", valeTransporte.ToString("F2") },
                    new string[] { "Vale Refeição", valeRefeicao.ToString("F2") },
                    new string[] { "Seguro de Vida", seguroDeVida.ToString("F2") },
                    new string[] { "INSS Folha", $"-{inss.ToString("F2")}" },
                    new string[] { "Salário Base", salario.ToString("F2") },
                    new string[] { "Salário Contribuinte INSS", inss.ToString("F2") },
                    new string[] { "Base Cálculo FGTS", baseFGTS.ToString("F2") },
                    new string[] { "FGTS do Mês", percentualFGTS.ToString("F2") },
                    new string[] { "Base Cálculo IRRF", baseCalculoIRRF.ToString("F2") });

                //DrawTable(gfx, fontNormal, 50, GetTableBottom(gfx, fontNormal, 50, 450, page.Width.Point - 100) + 20, page.Width.Point - 300, new string[] { },

                gfx.DrawString(" ", fontNormal, XBrushes.Black, new XRect(50, GetTableBottom(gfx, fontNormal, 50, 150, page.Width.Point - 100) + 40, page.Width.Point - 100, 20), XStringFormats.TopCenter);

                decimal totalBeneficios = dsr + valeTransporte + valeRefeicao + seguroDeVida;

                DrawText(gfx, fontNormal, $"Total de Benefícios: {totalBeneficios.ToString("F2")}", 50, GetTableBottom(gfx, fontNormal, 50, 320, page.Width.Point - 100) + 60, page.Width.Point - 100, XStringFormats.TopCenter);

                decimal salarioLiquido = salario - inss;
                DrawText(gfx, fontNormal, $"Salário Líquido: {salarioLiquido.ToString("F2")}", 50, GetTableBottom(gfx, fontNormal, 50, 340, page.Width.Point - 100) + 60, page.Width.Point - 100, XStringFormat.TopCenter);

                gfx.DrawString(" ", fontNormal, XBrushes.Black, new XRect(50, page.Height.Point - 60, page.Width.Point - 100, 20), XStringFormats.TopCenter);
                gfx.DrawString(" ", fontNormal, XBrushes.Black, new XRect(50, page.Height.Point - 40, page.Width.Point - 100, 20), XStringFormats.TopCenter);

                DrawText(gfx, fontNormal, "Assinatura do Funcionário: _______________________________", 50, GetTableBottom(gfx, fontNormal, 50, 380, page.Width.Point - 100) + 60, page.Width.Point - 100, XStringFormats.TopCenter);

                document.Save(pdfFilePath);
            }
        }

        static decimal CalcularINSS(decimal salario)
        {
            if (salario <= 1302.00M)
            {
                return salario * 0.075M;
            }
            else if (salario <= 2571.29M)
            {
                return salario * 0.09M;
            }
            else if (salario <= 3856.94M)
            {
                return salario * 0.12M;
            }
            else if (salario <= 7507.49M)
            {
                return salario * 0.14M;
            }

            return 0M;
        }

        static void DrawTable(XGraphics gfx, XFont font, double x, double y, double width, string[] headers, params string[][] rowValues)
        {
            double rowHeight = 20;
            double cellPadding = 0;

            double currentY = y;
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawString(headers[i], font, XBrushes.Black, new XRect(x + (width / headers.Length) * i, currentY, width / headers.Length, rowHeight), XStringFormats.TopCenter);
            }

            currentY += rowHeight;

            foreach (var rowValue in rowValues)
            {
                for (int i = 0; i < rowValue.Length; i++)
                {
                    gfx.DrawRectangle(XPens.Black, x + (width / headers.Length) * i, currentY, width / headers.Length, rowHeight);
                    gfx.DrawString(rowValue[i], font, XBrushes.Black, new XRect(x + (width / headers.Length) * i, currentY, width / headers.Length, rowHeight), XStringFormats.TopCenter);
                }
                currentY += rowHeight + cellPadding;
            }
        }

        static double GetTableBottom(XGraphics gfx, XFont font, double x, double y, double width)
        {
            return y + 20;
        }

        static void DrawText(XGraphics gfx, XFont font, string text, double x, double y, double width, XStringFormat format)
        {
            gfx.DrawString(text, font, XBrushes.Black, new XRect(x, y, width, font.Height), format);
        }

        static void CompactarPDFs(List<string> pdfFiles)
        {
            // Nome do arquivo ZIP
            string zipFileName = "funcionarios.zip";

            // Caminho para o arquivo ZIP
            string zipFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", zipFileName);

            int count = 1;

            // Garante que o nome do arquivo ZIP não existe
            while (File.Exists(zipFilePath))
            {
                zipFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"funcionarios_{count}.zip");
                count++;
            }

            // Criação do arquivo ZIP contendo os PDFs
            using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var pdfFile in pdfFiles)
                {
                    string fileName = Path.GetFileName(pdfFile);
                    zipArchive.CreateEntryFromFile(pdfFile, fileName);
                }
            }
        }
    }
}
