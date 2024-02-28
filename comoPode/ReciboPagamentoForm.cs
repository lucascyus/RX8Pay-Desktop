using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using Font = iTextSharp.text.Font;

namespace comoPode
{
    public partial class Relatorio : Form
    {
        public Relatorio()
        {
            InitializeComponent();
        }

        private void btnExportarRecibo_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=DESKTOP-7092F7U;Initial Catalog=SALARY_SYNC;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            // Consulte o banco de dados para obter os dados dos funcionários
            string query = "SELECT Nome, Salario, Cargo FROM tbl_funcionario WHERE StatusPagamento = 'Pago'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            // Para cada funcionário, gere um relatório em PDF
            while (reader.Read())
            {
                string nomeFuncionario = reader["Nome"].ToString();
                decimal salarioBase = Convert.ToDecimal(reader["Salario"]);
                string cargoFuncionario = reader["Cargo"].ToString();

                // Crie um documento PDF
                Document doc = new Document();
                string path = $"C:\\Users\\fshlu\\{nomeFuncionario}_recibo.pdf";

                PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                doc.Open();

                // Defina a fonte e tamanho da fonte para o título
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(bf, 16);

                // Defina a fonte e tamanho da fonte para os detalhes
                Font detailsFont = new Font(bf, 12);

                // Crie um parágrafo para o título
                Paragraph title = new Paragraph("Recibo de Pagamento", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                doc.Add(title);

                // Adicione um espaço em branco após o título
                doc.Add(new Paragraph("\n"));

                // Crie uma tabela para os detalhes do recibo
                PdfPTable table = new PdfPTable(2);
                table.DefaultCell.Border = 0;

                // Adicione linhas com os detalhes do funcionário
                AdicionarLinhaRecibo(table, "Nome do Funcionário:", nomeFuncionario, detailsFont);
                AdicionarLinhaRecibo(table, "Cargo:", cargoFuncionario, detailsFont);
                AdicionarLinhaRecibo(table, "Salário Base:", salarioBase.ToString("C"), detailsFont);

                // Calcule o salário líquido
                
                decimal dsr = CalcularDSR(salarioBase);
                AdicionarLinhaRecibo(table, "DSR:", dsr.ToString("C"), detailsFont);

                decimal valeTransporte = CalcularValeTransporte(salarioBase);
                AdicionarLinhaRecibo(table, "Vale Transporte:", valeTransporte.ToString("C"), detailsFont);

                decimal refeicao = CalcularRefeicao();
                AdicionarLinhaRecibo(table, "Refeição:", refeicao.ToString("C"), detailsFont);

                decimal planoSaude = CalcularPlanoSaude();
                AdicionarLinhaRecibo(table, "Plano de Saúde:", planoSaude.ToString("C"), detailsFont);

                decimal inss = CalcularINSS();
                AdicionarLinhaRecibo(table, "INSS:", inss.ToString("C"), detailsFont);

                decimal salarioLiquido = CalcularSalarioLiquido(salarioBase);
                AdicionarLinhaRecibo(table, "Salário Líquido:", salarioLiquido.ToString("C"), detailsFont);


                // Adicione mais detalhes e cálculos aqui

                doc.Add(table);

                // Adicione espaço em branco antes da assinatura e data
                doc.Add(new Paragraph("\n"));

                // Adicione linhas para assinatura e data
                Paragraph linhaAssinatura = new Paragraph("_____________________________");
                linhaAssinatura.Alignment = Element.ALIGN_CENTER;
                doc.Add(linhaAssinatura);

                Paragraph linhaData = new Paragraph(DateTime.Now.ToString("dd/MM/yyyy"));
                linhaData.Alignment = Element.ALIGN_CENTER;
                doc.Add(linhaData);

                // Feche o documento e salve-o
                doc.Close();
            }

            // Feche a conexão com o banco de dados
            connection.Close();

            // Exiba uma mensagem de confirmação
            MessageBox.Show("Recibos exportados com sucesso!");
        }

        private void AdicionarLinhaRecibo(PdfPTable table, string descricao, string valor, Font fonte)
        {
            PdfPCell cellDescricao = new PdfPCell(new Phrase(descricao, fonte));
            PdfPCell cellValor = new PdfPCell(new Phrase(valor, fonte));

            cellDescricao.Border = 0;
            cellValor.Border = 0;

            table.AddCell(cellDescricao);
            table.AddCell(cellValor);
        }

        private decimal CalcularSalarioLiquido(decimal salarioBase)
        {
            decimal dsr = CalcularDSR(salarioBase);
            decimal valeTransporte = CalcularValeTransporte(salarioBase);
            decimal refeicao = CalcularRefeicao();
            decimal planoSaude = CalcularPlanoSaude();
            decimal inss = CalcularINSS();

            // Calcula o salário líquido
            decimal salarioLiquido = salarioBase + dsr + valeTransporte + refeicao + planoSaude + inss;

            return salarioLiquido;
        }

        // Funções para calcular benefícios e descontos
        private decimal CalcularDSR(decimal salarioBase)
        {
            return salarioBase * 0.1m;
        }

        private decimal CalcularValeTransporte(decimal salarioBase)
        {
            return salarioBase * 0.06m;
        }

        private decimal CalcularRefeicao()
        {
            return 500.00m;
        }

        private decimal CalcularPlanoSaude()
        {
            return -250.00m; // Valor negativo representa um desconto.
        }

        private decimal CalcularINSS()
        {
            return -500.00m; // Valor negativo representa um desconto.
        }
    }
}
