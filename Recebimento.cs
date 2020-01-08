using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel.DataAnnotations;
using InterpretadorProtocoloMatrix.Interfaces;

namespace InterpretadorProtocoloMatrix.Recebimento
{
    /*
     * LAYOUT DO ARQUIVO QUE SERÁ DEVOLVIDO PELO MATRIXCONNECT AO LIS, CONTENDO OS RESULTADOS DOS EXAMES DE UMA ÚNICA AMOSTRA.
     * FORMATO DO NOME DOS ARQUIVOS: pppnnnnn.ENV
     * 
     * + ppp 
     * REFERE-SE AO CAMPO PREFIXO, UM CÓDIGO ATRIBUÍDO NO MATRIXCONNECT COM EXCLUSIVIDADE A CADA INSTRUMENTO DO LABORATÓRIO.
     * 
     * + nnnnn
     * REFERE-SE A UM NÚMERO SEQUENCIAL, INCREMENTADO A CADA ARQUIVO DE RESULTADOS GERADO NO INSTRUMENTO EM QUESTÃO.
     * 
     * 
     * +++ GRÁFICOS+++
     * 
     * CASO O GRÁFICO A SER GERADO ESTEJA CONFIGURADO PARA “PONTOS” NO CAMPO “GERAÇÃO”, O MATRIXCONNECT IRÁ ENVIAR APENAS AS LINHAS 25 E 26, 
     * CASO NECESSÁRIO, PARA O LIS. CASO ESTEJA CONFIGURADO PARA “IMAGEM”, O MATRIXCONNECT IRÁ ENVIAR A LINHA 27.
     */

    public class Paciente : IMatrix, IMatrixRecebimento
    {
        #region Construtores
        public Paciente(string linha)
        {
            ImportarLinha(linha);
        }
        #endregion

        #region Propriedades
        /// <summary>
        ///Constante "11". Indica que esta linha contém dados do paciente. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; private set; } = (TipoLinha)11;

        /// <summary>
        /// Identificação do paciente no LIS. (índice = 3)
        /// </summary>
        [StringLength(12, MinimumLength = 12)]
        public string RegistroPaciente { get; set; }

        /// <summary>
        /// Nome do paciente. (índice = 15)
        /// </summary>
        [Required()]
        [StringLength(50, MinimumLength = 50)]
        public string Nome { get; set; }

        /// <summary>
        /// Data de nascimento informada ou, caso não informada pelo LIS, data de nascimento calculada com base na idade informada, retroativamente em relação à data de recepção do arquivo do LIS. Se ambos os campos não tiverem sido informados até o momento da geração deste arquivo, a data de nascimento seguirá em branco. (índice = 65)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string DataNascimento { get; set; }

        /// <summary>
        /// Sexo do paciente. (índice = 73)
        /// </summary>
        [Required()]
        [StringLength(1, MinimumLength = 1)]
        public string Sexo { get; set; }

        /// <summary>
        /// Cor do paciente. (índice = 74)
        /// </summary>
        [Required()]
        [StringLength(1, MinimumLength = 1)]
        public string Cor { get; set; }

        /// <summary>
        /// (índice = 75)
        /// </summary>
        public string Reservado75 { get; } = string.Empty.PadLeft(174, ' ');

        /// <summary>
        /// (índice = 249)
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; }

        public string Linha { get; private set; }

        public List<AmostraRecebimento> Amostras { get; set; } = new List<AmostraRecebimento>();
        #endregion

        #region Métodos
        public static List<Paciente> ImportarPacientes(string strDiretorioArquivo)
        {
            List<Paciente> Pacientes = new List<Paciente>();
            List<string> lMatrixLines = File.ReadAllLines(strDiretorioArquivo, System.Text.Encoding.UTF8).ToList();

            foreach (string PacienteLine in lMatrixLines.Where(w => !w.Contains(Util.MatrixRuler) && Convert.ToInt32(w.OneBasedSubString(1, 2)) == (int)TipoLinha.Paciente))
            {
                Paciente nPaciente = new Paciente(PacienteLine);

                foreach (string AmostraLine in lMatrixLines.Where(w => !w.Contains(Util.MatrixRuler) && Convert.ToInt32(w.OneBasedSubString(1, 2)) == (int)TipoLinha.AmostraRecebimento && w.OneBasedSubString(55, 12) == nPaciente.RegistroPaciente))
                {
                    AmostraRecebimento nAmostra = new AmostraRecebimento(AmostraLine);

                    foreach (string ResultadoLine in lMatrixLines.Where(w => !w.Contains(Util.MatrixRuler) && Convert.ToInt32(w.OneBasedSubString(1, 2)) == (int)TipoLinha.ResultadoRecebimento))
                        nAmostra.Resultados.Add(new ResultadoRecebimento(ResultadoLine));

                    nPaciente.Amostras.Add(nAmostra);
                }

                Pacientes.Add(nPaciente);
            }

            return Pacientes;
        }
        public RetornoMatrixConnect ImportarLinha(string linha)
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Tipo = (TipoLinha)Convert.ToInt32(linha.OneBasedSubString(1, 2));
                RegistroPaciente = linha.OneBasedSubString(3, 12);
                Nome = linha.OneBasedSubString(15, 50);
                DataNascimento = linha.OneBasedSubString(65, 8);
                Sexo = linha.OneBasedSubString(73, 1);
                Cor = linha.OneBasedSubString(74, 1);
                DigitosVerificacao = linha.OneBasedSubString(249, 2);
                Linha = linha;

                return rmc;
            }
            catch (Exception ex)
            {
                rmc.bitSucesso = false;
                rmc.Mensagem = ex.Message;

                return rmc;
            }
        }
        #endregion
    }

    public class AmostraRecebimento : IMatrix, IMatrixRecebimento
    {
        #region Construtores
        public AmostraRecebimento(string linha)
        {
            ImportarLinha(linha);
        }
        #endregion

        #region Propriedades
        /// <summary>
        ///Constante "20". Indica que esta linha contém dados da amostra. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; private set; } = (TipoLinha)20;

        /// <summary>
        /// Identificação da amostra, atribuída pelo LIS. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        public string Amostra { get; set; }

        /// <summary>
        /// (índice = 15)
        /// </summary>
        public string Reservado15 { get; } = string.Empty.PadLeft(7, ' ');

        /// <summary>
        /// Conteúdo numérico inteiro, maior ou igual a 1. Contém a diluição da amostra (ex.: 5 indica 1:5). (índice = 22)
        /// </summary>
        [Required()]
        [StringLength(7, MinimumLength = 7)]
        public string Diluicao { get; set; }

        /// <summary>
        /// Código do agrupamento a que a amostra pertence (lote, worklist ...) no LIS. (índice = 29)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        public string Agrupamento { get; set; }

        /// <summary>
        /// Para diferenciação de amostras provenientes de diferentes laboratórios (departamentos, seções, bancadas ...) do cliente. Retorna o conteúdo do campo Laboratório do cadastro de instrumentos do MatrixConnect. (índice = 41)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string Laboratorio { get; set; }

        /// <summary>
        /// Instrumento em que os exames dessa amostra foram realizados. (índice = 49)
        /// </summary>
        [Required()]
        [StringLength(6, MinimumLength = 6)]
        public string Instrumento { get; set; }

        /// <summary>
        /// Identificação do paciente no LIS, conforme informado pelo próprio LIS. (índice = 55)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        public string RegistroPaciente { get; set; }

        /// <summary>
        /// Procedência da amostra, conforme informado pelo LIS (ex.: “ONCO” para clínica de oncologia, “GINECO” para ambulatório de ginecologia, “RUAX” para posto de coleta da Rua X, ...). (índice = 67)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string Origem { get; set; }

        /// <summary>
        /// Material da amostra (sangue, soro, plasma, urina ...), conforme informado pelo LIS. (índice = 75)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string Material { get; set; }

        /// <summary>
        /// Rack à qual a amostra foi associada no instrumento. Dependendo do tipo de instrumento, poderá ser indicado o código de uma rack de fato, ou o código de um rotor, de um disco, de uma microplaca etc. (índice = 83)
        /// </summary>
        [Required()]
        [StringLength(6, MinimumLength = 6)]
        public string Rack { get; set; }

        /// <summary>
        /// Data em que a amostra foi coletada, conforme informado pelo LIS. (índice = 89)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string DataColeta { get; set; }

        /// <summary>
        /// Campo livre, com envio opcional configurável no MatrixConnect. Conteúdo atribuído pelo operador para a amostra em questão. Ex.: “Material hemolisado”. (índice = 97)
        /// </summary>
        [Required()]
        [StringLength(80, MinimumLength = 80)]
        public string Observacao { get; set; }

        /// <summary>
        /// Posição à qual a amostra foi associada no rack do instrumento. (índice = 177)
        /// </summary>
        [Required()]
        [StringLength(6, MinimumLength = 6)]
        public string Escaninho { get; set; }

        /// <summary>
        /// (índice = 183)
        /// </summary>
        public string Reservado183 { get; } = string.Empty.PadLeft(66, ' ');

        /// <summary>
        /// (índice = 249)
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; }

        public string Linha { get; private set; }

        public List<ResultadoRecebimento> Resultados { get; set; } = new List<ResultadoRecebimento>();
        #endregion

        #region Métodos
        public RetornoMatrixConnect ImportarLinha(string linha)
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Tipo = (TipoLinha)Convert.ToInt32(linha.OneBasedSubString(1, 2));
                Amostra = linha.OneBasedSubString(3, 12);
                Diluicao = linha.OneBasedSubString(22, 7);
                Agrupamento = linha.OneBasedSubString(29, 12);
                Laboratorio = linha.OneBasedSubString(41, 8);
                Instrumento = linha.OneBasedSubString(49, 6);
                RegistroPaciente = linha.OneBasedSubString(55, 12);
                Origem = linha.OneBasedSubString(67, 8);
                Material = linha.OneBasedSubString(75, 8);
                Rack = linha.OneBasedSubString(83, 6);
                DataColeta = linha.OneBasedSubString(89, 8);
                Observacao = linha.OneBasedSubString(97, 80);
                Escaninho = linha.OneBasedSubString(177, 6);
                DigitosVerificacao = linha.OneBasedSubString(249, 2);
                Linha = linha;

                return rmc;
            }
            catch (Exception ex)
            {
                rmc.bitSucesso = false;
                rmc.Mensagem = ex.Message;

                return rmc;
            }
        }
        #endregion
    }

    public class ResultadoRecebimento : IMatrix, IMatrixRecebimento
    {
        #region Construtores
        public ResultadoRecebimento(string linha)
        {
            ImportarLinha(linha);
        }
        #endregion

        #region Propriedades
        /// <summary>
        ///Constante "21". Indica que esta linha contém dados de resultado. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; private set; } = (TipoLinha)21;

        /// <summary>
        /// Identificação de um dos exames solicitados pelo LIS para esta amostra, codificado conforme convenção do próprio LIS. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string ExameLIS { get; set; }

        /// <summary>
        /// Identificação de um parâmetro do exame em questão, codificado conforme convenção do próprio LIS. (índice = 11)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string ParametroLIS { get; set; }

        /// <summary>
        /// Resultado do parâmetro em questão. (índice = 19)
        /// </summary>
        [Required()]
        [StringLength(80, MinimumLength = 80)]
        public string Resultado { get; set; }

        /// <summary>
        /// (índice = 99)
        /// </summary>
        public string Reservado99 { get; } = string.Empty.PadLeft(118, ' ');

        /// <summary>
        /// Data em que o resultado foi recebido do instrumento e gravado no banco de dados do MatrixConnect. (índice = 217)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string DataResultado { get; set; }

        /// <summary>
        /// Horário em que o resultado foi recebido do instrumento e gravado no banco de dados do MatrixConnect.  (índice = 225)
        /// </summary>
        [Required()]
        [StringLength(4, MinimumLength = 4)]
        public string HoraResultado { get; set; }

        /// <summary>
        /// Data em que o driver de comunicação com o LIS processou e liberou este exame/parâmetro. Observação: o MatrixConnect, internamente, só mantém data/hora da primeira liberação; este campo, entretanto, é enviado atualizado a cada liberação. (índice = 229)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string DataLiberacao { get; set; }

        /// <summary>
        /// Horário em que o driver de comunicação com o LIS processou e liberou este exame/parâmetro. Observação: o MatrixConnect, internamente, só mantém data/hora da primeira liberação; este campo, entretanto, é enviado atualizado a cada liberação. (índice = 237)
        /// </summary>
        [Required()]
        [StringLength(4, MinimumLength = 4)]
        public string HoraLiberacao { get; set; }

        /// <summary>
        /// Identificação do usuário que solicitou a liberação do resultado do exame/parâmetro. (índice = 241)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string OperadorLiberacao { get; set; }

        /// <summary>
        /// (índice = 249)
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; }

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect ImportarLinha(string linha)
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Tipo = (TipoLinha)Convert.ToInt32(linha.OneBasedSubString(1, 2));
                ExameLIS = linha.OneBasedSubString(3, 8);
                ParametroLIS = linha.OneBasedSubString(11, 8);
                Resultado = linha.OneBasedSubString(19, 80);
                DataResultado = linha.OneBasedSubString(217, 8);
                HoraResultado = linha.OneBasedSubString(225, 4);
                DataLiberacao = linha.OneBasedSubString(229, 8);
                HoraLiberacao = linha.OneBasedSubString(237, 4);
                OperadorLiberacao = linha.OneBasedSubString(241, 8);
                DigitosVerificacao = linha.OneBasedSubString(249, 2);
                Linha = linha;

                return rmc;
            }
            catch (Exception ex)
            {
                rmc.bitSucesso = false;
                rmc.Mensagem = ex.Message;

                return rmc;
            }
        }
        #endregion
    }

    public class FlagRecebimento : IMatrix, IMatrixRecebimento
    {
        #region Propriedades
        /// <summary>
        ///Constante "23". Indica que esta linha contém os flags ocorridos no exame. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; private set; } = (TipoLinha)23;

        /// <summary>
        /// Identificação de um dos exames solicitados pelo LIS para esta amostra, codificado conforme convenção do próprio LIS. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string ExameLIS { get; set; }

        /// <summary>
        /// Identificação de um dos flags ocorridos durante a recepção do resultado do exame em questão. (índice = 11)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string Flag { get; set; }

        /// <summary>
        /// Descrição do flag, conforme configurado no MatrixConnect. (índice = 19)
        /// </summary>
        [Required()]
        [StringLength(80, MinimumLength = 80)]
        public string Descricao { get; set; }

        /// <summary>
        /// (índice = 99)
        /// </summary>
        public string Reservado99 { get; set; } = string.Empty.PadLeft(150, ' ');

        /// <summary>
        /// (índice = 249)
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; }

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect ImportarLinha(string linha)
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Tipo = (TipoLinha)Convert.ToInt32(linha.OneBasedSubString(1, 2));
                ExameLIS = linha.OneBasedSubString(3, 8);
                Flag = linha.OneBasedSubString(11, 8);
                Descricao = linha.OneBasedSubString(19, 80);
                DigitosVerificacao = linha.OneBasedSubString(249, 2);
                Linha = linha;

                return rmc;
            }
            catch (Exception ex)
            {
                rmc.bitSucesso = false;
                rmc.Mensagem = ex.Message;

                return rmc;
            }
        }
        #endregion
    }

    /// <summary>
    /// Repetição, tipicamente, é o resultado que foi enviado pelo instrumento e não foi aproveitado, pois o laboratorista decidiu repetir o exame no instrumento, dando origem a um novo resultado. Os resultados das repetições são armazenados no MatrixConnect e podem ser enviados para o LIS.
    /// </summary>
    public class Repeticao : IMatrix, IMatrixRecebimento
    {
        #region Propriedades
        /// <summary>
        ///Constante "24". Indica que esta linha contém dados de repetições. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; private set; } = (TipoLinha)24;

        /// <summary>
        /// Identificação de um dos exames solicitados pelo LIS para esta amostra, codificado conforme convenção do próprio LIS. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string ExameLIS { get; set; }

        /// <summary>
        /// Identificação de um parâmetro do exame em questão, codificado conforme convenção do próprio LIS. (índice = 11)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string ParametroLIS { get; set; }

        /// <summary>
        /// Resultado do parâmetro em questão. (índice = 19)
        /// </summary>
        [Required()]
        [StringLength(80, MinimumLength = 80)]
        public string Resultado { get; set; }

        /// <summary>
        /// (índice = 99)
        /// </summary>
        public string Reservado99 { get; } = string.Empty.PadLeft(116, ' ');

        /// <summary>
        /// Número que identifica qual é o número desta repetição, isto é, o resultado imediatamente anterior ao atual será 01, o penúltimo será 02, e assim por diante. (índice = 215)
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string NumeroResultado { get; set; }

        /// <summary>
        /// Data em que o resultado foi recebido pelo driver do instrumento e gravado no banco de dados do MatrixConnect. (índice = 217)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string DataResultado { get; set; }

        /// <summary>
        /// Horário em que o resultado foi recebido pelo driver do instrumento e gravado no banco de dados do MatrixConnect. (índice = 225)
        /// </summary>
        [Required()]
        [StringLength(4, MinimumLength = 4)]
        public string HoraResultado { get; set; }

        /// <summary>
        /// Data em que o driver de comunicação com o LIS processou e liberou este exame/parâmetro.Observação: o MatrixConnect, internamente, não mantém data e hora de liberação. (índice = 229)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string DataLiberacao { get; set; }

        /// <summary>
        /// Horário em que o driver de comunicação com o LIS processou e liberou este exame/parâmetro.Observação: o MatrixConnect, internamente, não mantém data e hora de liberação. (índice = 237)
        /// </summary>
        [Required()]
        [StringLength(4, MinimumLength = 4)]
        public string HoraLiberacao { get; set; }

        /// <summary>
        /// Identificação do usuário que solicitou a liberação do resultado do exame/parâmetro. (índice = 241)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string OperadorLiberacao { get; set; }

        /// <summary>
        /// (índice = 249)
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; }

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect ImportarLinha(string linha)
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Tipo = (TipoLinha)Convert.ToInt32(linha.OneBasedSubString(1, 2));
                ExameLIS = linha.OneBasedSubString(3, 8);
                ParametroLIS = linha.OneBasedSubString(11, 8);
                Resultado = linha.OneBasedSubString(19, 80);
                NumeroResultado = linha.OneBasedSubString(215, 2);
                DataResultado = linha.OneBasedSubString(217, 8);
                HoraResultado = linha.OneBasedSubString(225, 4);
                DataLiberacao = linha.OneBasedSubString(229, 8);
                HoraLiberacao = linha.OneBasedSubString(237, 4);
                OperadorLiberacao = linha.OneBasedSubString(241, 8);
                DigitosVerificacao = linha.OneBasedSubString(249, 2);
                Linha = linha;

                return rmc;
            }
            catch (Exception ex)
            {
                rmc.bitSucesso = false;
                rmc.Mensagem = ex.Message;

                return rmc;
            }
        }
        #endregion
    }

    public class DadosGrafico : IMatrix, IMatrixRecebimento
    {
        #region Propriedades
        /// <summary>
        ///Constante "25". Indica que esta linha contém dados do gráfico. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; private set; } = (TipoLinha)25;

        /// <summary>
        /// Identificação de um dos exames solicitados pelo LIS para esta amostra, codificado conforme convenção do próprio LIS. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string ExameLIS { get; set; }

        /// <summary>
        /// Identificação do gráfico configurado no MatrixConnect. (índice = 11)
        /// </summary>
        [Required()]
        [StringLength(15, MinimumLength = 15)]
        public string NomeGrafico { get; set; }

        /// <summary>
        /// (índice = 26)
        /// </summary>
        [Required()]
        [StringLength(4, MinimumLength = 4)]
        public string Sequencia { get; set; } //NUMÉRICO

        /// <summary>
        /// Valor do ponto no eixo X. (índice = 30)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string Ponto1 { get; set; } //NUMÉRICO

        /// <summary>
        /// Valor do dados no eixo Y. (índice = 35)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string Dado1 { get; set; } //NUMÉRICO

        /// <summary>
        /// (índice = 10 * (n - 1) + 30)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string PontoN { get; set; } //NUMÉRICO

        /// <summary>
        /// (índice = 10 * (n - 1) + 35)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string DadoN { get; set; } //NUMÉRICO

        /// <summary>
        /// (índice = 220)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string Ponto20 { get; set; } //NUMÉRICO

        /// <summary>
        /// (índice = 225)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string Dado20 { get; set; } //NUMÉRICO

        /// <summary>
        /// (índice = 230)
        /// </summary>
        public string Reservado230 { get; } = string.Empty.PadLeft(19, ' ');

        /// <summary>
        /// (índice = 249)
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; }

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect ImportarLinha(string linha)
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Tipo = (TipoLinha)Convert.ToInt32(linha.OneBasedSubString(1, 2));
                ExameLIS = linha.OneBasedSubString(3, 8);
                NomeGrafico = linha.OneBasedSubString(11, 15);
                Sequencia = linha.OneBasedSubString(26, 4);
                Ponto1 = linha.OneBasedSubString(30, 5);
                Dado1 = linha.OneBasedSubString(35, 5);
                PontoN = linha.OneBasedSubString(30, 5); //TODO: VERIFICAR EQUAÇÃO PARA RECUPERAR ÍNDICE DA PROPRIEDADE.
                DadoN = linha.OneBasedSubString(35, 5); //TODO: VERIFICAR EQUAÇÃO PARA RECUPERAR ÍNDICE DA PROPRIEDADE.
                Ponto20 = linha.OneBasedSubString(220, 5);
                Dado20 = linha.OneBasedSubString(225, 5);
                DigitosVerificacao = linha.OneBasedSubString(249, 2);
                Linha = linha;

                return rmc;
            }
            catch (Exception ex)
            {
                rmc.bitSucesso = false;
                rmc.Mensagem = ex.Message;

                return rmc;
            }
        }
        #endregion
    }

    public class DadosAdicionaisGrafico : IMatrix, IMatrixRecebimento
    {
        #region Propriedades
        /// <summary>
        ///Constante "26". Indica que esta linha contém dados adicionais do gráfico. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; private set; } = (TipoLinha)26;

        /// <summary>
        /// Identificação de um dos exames solicitados pelo LIS para esta amostra, codificado conforme convenção do próprio LIS. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string ExameLIS { get; set; }

        /// <summary>
        /// Identificação do gráfico configurado no MatrixConnect. (índice = 11)
        /// </summary>
        [Required()]
        [StringLength(15, MinimumLength = 15)]
        public string NomeGrafico { get; set; }

        /// <summary>
        /// Mnemônico da informação adicional mandado pelo instrumento. (índice = 26)
        /// </summary>
        [Required()]
        [StringLength(3, MinimumLength = 3)]
        public TipoDadoAdicional TipoDadoAdicional { get; set; }

        /// <summary>
        /// (índice = 29)
        /// </summary>
        [Required()]
        [StringLength(4, MinimumLength = 4)]
        public string Sequencia { get; set; } //NUMÉRICO

        /// <summary>
        /// Identificação do ponto. Pode ser um número de sequencia, um mnemônico que identifica o ponto, sua coordenada "X", ou qualquer outro valor dependendo da conveniência. (índice = 33)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string IdPonto1 { get; set; }

        /// <summary>
        /// Valor que o ponto assume. Pode ser tanto a coordenad X ou Y, variando de acordo com o tipo de gráfico, e nome do ponto. (índice = 38)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string ValorPonto1 { get; set; } //NUMÉRICO

        /// <summary>
        /// (índice = 10 * (n - 1) + 33)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string IdPontoN { get; set; } //TODO: VERIFICAR PORQUE ÍNDICE É UMA EXPRESSÃO NUMÉRICA, MAS O TIPO É ALFANUMÉRICO

        /// <summary>
        /// (índice = 10 * (n - 1) + 38)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string ValorPontoN { get; set; } //NUMÉRICO

        /// <summary>
        /// (índice = 223)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string IdPonto20 { get; set; }

        /// <summary>
        /// (índice = 228)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string ValorPonto20 { get; set; } //NUMÉRICO

        /// <summary>
        /// (índice = 233)
        /// </summary>
        public string Reservado233 { get; } = string.Empty.PadLeft(16, ' ');

        /// <summary>
        /// (índice = 249)
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; }

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect ImportarLinha(string linha)
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Tipo = (TipoLinha)Convert.ToInt32(linha.OneBasedSubString(1, 2));
                ExameLIS = linha.OneBasedSubString(3, 8);
                NomeGrafico = linha.OneBasedSubString(11, 15);
                TipoDadoAdicional = linha.OneBasedSubString(26, 3) == "MIN" ? TipoDadoAdicional.MIN : TipoDadoAdicional.TSH;
                Sequencia = linha.OneBasedSubString(29, 4);
                IdPonto1 = linha.OneBasedSubString(33, 5);
                ValorPonto1 = linha.OneBasedSubString(38, 5);
                IdPontoN = linha.OneBasedSubString(33, 5); //TODO: VERIFICAR EQUAÇÃO PARA RECUPERAR ÍNDICE DA PROPRIEDADE.
                ValorPontoN = linha.OneBasedSubString(38, 5); //TODO: VERIFICAR EQUAÇÃO PARA RECUPERAR ÍNDICE DA PROPRIEDADE.
                IdPonto20 = linha.OneBasedSubString(223, 5);
                ValorPonto20 = linha.OneBasedSubString(228, 5);
                DigitosVerificacao = linha.OneBasedSubString(249, 2);
                Linha = linha;

                return rmc;
            }
            catch (Exception ex)
            {
                rmc.bitSucesso = false;
                rmc.Mensagem = ex.Message;

                return rmc;
            }
        }
        #endregion
    }

    public class ArquivoGrafico : IMatrix, IMatrixRecebimento
    {
        #region Propriedades
        /// <summary>
        ///Constante "27". Indica que esta linha contém o nome e o endereço do arquivo de imagem a ser gerado. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; private set; } = (TipoLinha)27;

        /// <summary>
        /// Identificação de um dos exames solicitados pelo LIS para esta amostra, codificado conforme convenção do próprio LIS. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string ExameLIS { get; set; }

        /// <summary>
        /// Identificação do gráfico configurado no Matrix Interface. (índice = 11)
        /// </summary>
        [Required()]
        [StringLength(15, MinimumLength = 15)]
        public string NomeGrafico { get; set; }

        /// <summary>
        /// Caminho para o arquivo de imagem a ser gerado. (índice = 26)
        /// </summary>
        [Required()]
        [StringLength(130, MinimumLength = 130)]
        public string EnderecoGrafico { get; set; }

        /// <summary>
        /// Nome do arquivo a ser gerado. (índice = 156)
        /// </summary>
        [Required()]
        [StringLength(80, MinimumLength = 80)]
        public string NomeArquivo { get; set; }

        /// <summary>
        /// (índice = 236)
        /// </summary>
        public string Reservado236 { get; } = string.Empty.PadLeft(13, ' ');

        /// <summary>
        /// (índice = 249)
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; }

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect ImportarLinha(string linha)
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Tipo = (TipoLinha)Convert.ToInt32(linha.OneBasedSubString(1, 2));
                ExameLIS = linha.OneBasedSubString(3, 8);
                NomeGrafico = linha.OneBasedSubString(11, 15);
                EnderecoGrafico = linha.OneBasedSubString(26, 130);
                NomeArquivo = linha.OneBasedSubString(156, 80);
                DigitosVerificacao = linha.OneBasedSubString(249, 2);
                Linha = linha;

                return rmc;
            }
            catch (Exception ex)
            {
                rmc.bitSucesso = false;
                rmc.Mensagem = ex.Message;

                return rmc;
            }
        }
        #endregion
    }

    public class FimLote : IMatrix, IMatrixRecebimento
    {
        #region Propriedades
        /// <summary>
        ///Constante "22". Indica que esta linha finaliza o arquivo de resultados de uma amostra.. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; private set; } = (TipoLinha)22;

        /// <summary>
        /// Total de linhas neste arquivo ENV, inclusive a linha 11 (se houver), a linha 20, todas as linhas 21, todas as linhas 23, todas as linhas 24, e a própria linha 22. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string TotalLinhasArquivo { get; set; } //NUMÉRICO

        /// <summary>
        /// (índice = 249)
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; }

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect ImportarLinha(string linha)
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Tipo = (TipoLinha)Convert.ToInt32(linha.OneBasedSubString(1, 2));
                TotalLinhasArquivo = linha.OneBasedSubString(3, 5);
                DigitosVerificacao = linha.OneBasedSubString(8, 2);
                Linha = linha;

                return rmc;
            }
            catch (Exception ex)
            {
                rmc.bitSucesso = false;
                rmc.Mensagem = ex.Message;

                return rmc;
            }
        }
        #endregion
    }
}