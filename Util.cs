using System;
using System.Collections.Generic;
using System.Linq;

namespace InterpretadorProtocoloMatrix
{
    #region Enumeráveis (int)
    public enum TipoLinha
    {
        AmostraEnvio = 10,
        Paciente = 11,
        Atributo = 12,
        DadosExtendidos = 13,
        ResultadoAnterior = 14,
        AtributosAmostraAnterior = 15,
        AmostraRecebimento = 20,
        ResultadoRecebimento = 21,
        FimLote = 22,
        FlagRecebimento = 23,
        Repeticoes = 24,
        DadosGrafico = 25,
        DadosAdicionaisGrafico = 26,
        ArquivoGrafico = 27
    } 
    #endregion

    #region Enumeráveis (string)
    /// <summary>
    /// Ordem de comando para amostras.
    /// </summary>
    public class Ordem
    {
        private Ordem(string valor)
        {
            Valor = valor;
        }

        public string Valor { get; set; }

        /// <summary>
        /// Ordem vazia. Enviar em branco fará com que o MatrixConnect entenda tratar-se de uma adição de exames/atualização dos dados da amostra.
        /// </summary>
        public static Ordem Vazia { get { return new Ordem(" "); } }

        /// <summary>
        /// Reiniciar os exames da amostra anteriormente enviada.
        /// </summary>
        public static Ordem Reiniciar { get { return new Ordem("R"); } }

        /// <summary>
        /// Excluir exames da amostra.
        /// </summary>
        public static Ordem Excluir { get { return new Ordem("E"); } }

        /// <summary>
        /// Excluir exames da amostra que não tenham resultados.
        /// </summary>
        public static Ordem ExcluirSemResultados { get { return new Ordem("S"); } }

        /// <summary>
        /// Cancelar.
        /// </summary>
        public static Ordem Cancelar { get { return new Ordem("C"); } }
    }

    /// <summary>
    /// Mnemônico da informação adicional mandado pelo instrumento. Atualmente, existem dois tipos de dados extras.
    /// </summary>
    public class TipoDadoAdicional
    {
        private TipoDadoAdicional(string valor)
        {
            Valor = Valor;
        }

        public string Valor { get; set; }

        /// <summary>
        /// Representam os pontos de mínimo no gráfico de eletroforese.
        /// </summary>
        public static TipoDadoAdicional MIN { get { return new TipoDadoAdicional("MIN"); } }

        /// <summary>
        /// Representam pontos de "threshold" (ponto inicial) que dividem o gráfico LMNE em áreas distintas.
        /// </summary>
        public static TipoDadoAdicional TSH { get { return new TipoDadoAdicional("TSH"); } }
    } 
    #endregion

    /// <summary>
    /// Retorno de importações e exportações do MatrixConnect.
    /// </summary>
    public class RetornoMatrixConnect
    {
        public bool bitSucesso { get; set; }
        
        public string Mensagem { get; set; }
    }

    public static class Util
    {
        public const string MatrixRuler = "1...5...10...15...20...25...30...35...40...45...50...55...60...65...70...75...80...85...90...95..100..105..110..115..120..125..130..135..140..145..150..155..160..165..170..175..180..185..190..195..200..205..210..215..220..225..230..235..240..245..250";

        public static List<string> LoadAlfabeto()
        {
            List<string> lAlfabeto = new List<string>();

            for (char c = 'A'; c <= 'Z'; c++)
                lAlfabeto.Add(c.ToString());

            return lAlfabeto;
        }

        public static string CalcularModulo256(string linha)
        {
            int iASCII = 0;

            foreach (char c in linha)
                iASCII =+ Convert.ToInt32(c);

            return (iASCII % 256).ToString("X");
        }

        public static string NormalizeProperty(this string s, int length, bool alignRight = false)
        {
            //STRING VAZIA OU NULA
            if (string.IsNullOrEmpty(s))
            {
                if (alignRight)
                    return string.Empty.PadLeft(length, ' ');

                else return string.Empty.PadRight(length, ' ');
            }

            //STRING MAIOR DO QUE O LIMITE ESTIPULADO
            else if (s.Length > length)
                return s.Substring(0, length);

            //COMPRIMENTO DE STRING DENTRO DO ESTIPULADO
            else
            {
                if (alignRight)
                    return s.PadLeft(length, ' ');

                else return s.PadRight(length, ' ');
            }
        }

        /// <summary>
        /// One-Based extension for original Substring who retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startIndex">The one-based starting character position of a substring in this instance.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <returns>A string that is equivalent to the substring of length length that begins at startIndex in this instance, or System.String.Empty if startIndex is equal to the length of this instance and length is zero.</returns>
        public static string OneBasedSubString(this string s, int startIndex, int length)
        {
            return s.Substring((startIndex - 1), length);
        }

        public static bool SalvarArquivoRCB(string strDiretorio, int iCodCliente, string strIdAmostra, string strBody, out string strMensagem)
        {
            try
            {
                //CONSTRUTOR DO NOME DO ARQUIVO
                string strFileName = $"{strDiretorio}\\{DateTime.Now.ToString("yyyyMMddTHHmmss")}_{iCodCliente}_{strIdAmostra}.RCB";

                //SE NÃO EXISTIR, CRIA O DIRETÓRIO SOLICITADO
                System.IO.FileInfo ArquivoRCB = new System.IO.FileInfo(strFileName);
                ArquivoRCB.Directory.Create();

                //SALVA XML EM DISCO
                System.IO.File.WriteAllText(ArquivoRCB.FullName, strBody, System.Text.Encoding.GetEncoding("ISO-8859-1"));

                strMensagem = "Arquivo .RCB salvo com sucesso.";
                return true;
            }
            catch (Exception ex)
            {
                strMensagem = $"Não foi possível salvar o arquivo .RCB em disco. {ex.Message}";
                return false;
            }
        }
    }
}