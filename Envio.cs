using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InterpretadorProtocoloMatrix.Interfaces;

/// <summary>
/// Summary description for Envio
/// </summary>
namespace InterpretadorProtocoloMatrix.Envio
{
    public class AmostraEnvio : IMatrix, IMatrixEnvio
    {
        #region Construtores
        public AmostraEnvio()
        {
            FormatarLinha();
        }
        #endregion

        #region Propriedades
        /// <summary>
        ///Constante "10". Indica que esta linha contém dados da amostra. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; } = (TipoLinha)10;

        /// <summary>
        ///Identificação da amostra, atribuída pelo LIS. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        public string Amostra
        {
            get => _amostra;
            set => _amostra = value.NormalizeProperty(12);
        }
        private string _amostra = string.Empty.NormalizeProperty(12);

        /// <summary>
        ///Índice = 15
        /// </summary>
        public string Reservado15 { get; } = string.Empty.PadLeft(6, ' ');

        /// <summary>
        ///Verificar objeto "Ordem". (índice = 21)
        /// </summary>
        [Required()]
        [StringLength(1, MinimumLength = 1)]
        public Ordem Ordem { get; set; }

        /// <summary>
        /// Conteúdo numérico inteiro, maior ou igual a 1. Se não informado (em branco), o MatrixConnect assumirá diluição 1:1 para a amostra. (índice = 22)
        /// </summary>
        [StringLength(7, MinimumLength = 7)]
        public string Diluicao
        {
            get => _diluicao;
            set => _diluicao = value.NormalizeProperty(7);
        }
        private string _diluicao = string.Empty.NormalizeProperty(7);

        /// <summary>
        ///Código do agrupamento a que a amostra pertence (pedido, lote, worklist ...) no LIS. (índice = 29)
        /// </summary>
        [StringLength(12, MinimumLength = 12)]
        public string Agrupamento
        {
            get => _agrupamento;
            set => _agrupamento = value.NormalizeProperty(12);
        }
        private string _agrupamento = string.Empty.NormalizeProperty(12);

        /// <summary>
        ///Índice = 41
        /// </summary>
        public string Reservado41 { get; } = string.Empty.PadLeft(1, ' ');

        /// <summary>
        /// Hora em que a amostra foi coletada. Para abdicar da informação da hora de coleta, o LIS deverá enviar este campo em branco. (índice = 42; formato "HHMM")
        /// </summary>
        [StringLength(4, MinimumLength = 4)]
        public string HoraColeta
        {
            get => _horaColeta.NormalizeProperty(4);
            set => _horaColeta = value.NormalizeProperty(4);
        }
        private string _horaColeta = string.Empty.NormalizeProperty(4);

        /// <summary>
        ///Prioridade da amostra: "R" para rotina ou "U" para urgência. Default: "R". (índice = 46)
        /// </summary>
        [StringLength(1, MinimumLength = 1)]
        public string Prioridade
        {
            get => _prioridade;
            set => _prioridade = value ?? "R";
        }
        private string _prioridade = "R";

        /// <summary>
        ///Material biológico da amostra.  Serão aceitos os valores que o cliente cadastrar na tabela Material do MatrixConnect (campo No LIS). Amostras com identificação do material não cadastrado no MatrixConnect serão aceitas, sendo desconsiderado o campo material informado. (índice = 47)
        /// </summary>
        [StringLength(8, MinimumLength = 8)]
        public string Material
        {
            get => _material;
            set => _material = value.NormalizeProperty(8);
        }
        private string _material = string.Empty.NormalizeProperty(8);

        /// <summary>
        /// Instrumento em que os exames dessa amostra serão realizados. Recomenda-se o envio deste campo em branco, deixando a cargo do MatrixConnect a determinação, através do recurso Triagem, de em qual(is) instrumento(s) a amostra deverá ser recebida. (índice = 55)
        /// </summary>
        [StringLength(6, MinimumLength = 6)]
        public string Instrumento
        {
            get => _instrumento;
            set => _instrumento = value.NormalizeProperty(6);
        }
        private string _instrumento = string.Empty.NormalizeProperty(6);

        /// <summary>
        /// Identificação do paciente no LIS. Chave para vinculação da amostra aos dados de paciente enviados nas linhas tipo 11. (índice = 61)
        /// </summary>
        [StringLength(12, MinimumLength = 12)]
        public string RegistroPaciente
        {
            get => _registroPaciente;
            set => _registroPaciente = value.NormalizeProperty(12);
        }
        public string _registroPaciente = string.Empty.NormalizeProperty(12);

        /// <summary>
        /// Código da procedência da amostra. Ex.: "ONCO" - clínica de oncologia; "GINECO" - ambulatório de ginecologia; "RUAX" - posto de coleta da Rua X. (índice = 73)
        /// </summary>
        [StringLength(8, MinimumLength = 8)]
        public string Origem
        {
            get => _origem;
            set => _origem = value.NormalizeProperty(8);
        }
        private string _origem = string.Empty.NormalizeProperty(8);

        /// <summary>
        /// Data em que a amostra foi coletada. Se a data de coleta for informada e a hora de coleta não for informada, a hora de coleta será assumida como 00:00. (índice = 81)
        /// </summary>
        [StringLength(8, MinimumLength = 8)]
        public string DataColeta
        {
            get => _dataColeta;
            set => _dataColeta = value.NormalizeProperty(8);
        }
        private string _dataColeta = string.Empty.NormalizeProperty(8);

        /// <summary>
        ///  De 1 a 20 exames podem ser solicitados para a amostra em cada linha 10. Exames adicionais deverão ser informados através do uso de linha(s) 10 adicional(is). Utilizar identificações de exames conforme nomenclatura utilizada no próprio LIS; as traduções necessárias serão feitas pelo MatrixConnect por ocasião da recepção da amostra. (índice = 89)
        /// </summary>
        [Required()]
        public Dictionary<string, string> Exames
        {
            get
            {
                if (!_exames.Any())
                {
                    for (int i = 0; i <= 20; i++)
                        _exames.Add(string.Format("EXAME{0}", i), string.Empty.NormalizeProperty(8));
                }

                return _exames;
            }
            set => _exames = value;
        }
        private Dictionary<string, string> _exames = new Dictionary<string, string>();

        /// <summary>
        ///Índice = 249
        /// </summary>
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; }

        public List<Atributo> AtributosAmostra { get; set; } = new List<Atributo>();

        public DadosExtendidos DadosExtendidosAmostra { get; set; } = new DadosExtendidos();

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect FormatarLinha()
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Linha =
                    (int)Tipo +
                    Amostra +
                    Reservado15 +
                    Ordem.Valor +
                    Diluicao +
                    Agrupamento +
                    Reservado41 +
                    HoraColeta +
                    Prioridade +
                    Material +
                    Instrumento +
                    RegistroPaciente +
                    Origem +
                    DataColeta;

                for (int i = 0; i <= 20; i++)
                    Linha += Exames[string.Format("EXAME{0}", i)];

                DigitosVerificacao = Util.CalcularModulo256(Linha);
                Linha += DigitosVerificacao;

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

    public class Paciente : IMatrix, IMatrixEnvio
    {
        #region Propriedades
        /// <summary>
        ///Constante "11". Indica que esta linha contém dados do paciente. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; } = (TipoLinha)11;

        /// <summary>
        /// Identificação do paciente no LIS. Chave para vinculação da(s) amostra(s) enviada(s) na(s) linha(s) tipo 10 ao respectivo paciente. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        public string RegistroPaciente
        {
            get => _registroPaciente;
            set => _registroPaciente = value.NormalizeProperty(12);
        }
        private string _registroPaciente = string.Empty.NormalizeProperty(12);

        /// <summary>
        /// Nome do paciente. Sugestão: formato sobrenome, nome, para facilidade de consultas futuras no MatrixConnect. (índice = 15)
        /// </summary>
        [Required()]
        [StringLength(50, MinimumLength = 50)]
        public string Nome
        {
            get => _nome;
            set => _nome = value.NormalizeProperty(50);
        }
        private string _nome = string.Empty.NormalizeProperty(50);

        /// <summary>
        /// Idade do paciente, para cálculo da data de nascimento. A informação da data de nascimento prevalece sobre este campo. Se ambas forem informadas, a idade informada neste campo será desprezada. (índice = 65; formato AAAMMDD)
        /// </summary>
        [StringLength(7, MinimumLength = 7)]
        public string Idade
        {
            get => _idade;
            set => _idade = value.NormalizeProperty(7);
        }
        private string _idade = string.Empty.NormalizeProperty(7);

        /// <summary>
        /// Data de nascimento do paciente. Tem prevalência sobre a idade informada no campo anterior. (índice = 72; formato AAAAMMDD)
        /// </summary>
        [StringLength(8, MinimumLength = 8)]
        public string DataNascimento
        {
            get => _dataNascimento;
            set => _dataNascimento = value.NormalizeProperty(8);
        }
        private string _dataNascimento = string.Empty.NormalizeProperty(8);

        /// <summary>
        /// Sexo do paciente. Serão aceitos os valores cadastrados no MatrixConnect. (índice = 80)
        /// </summary>
        [StringLength(1, MinimumLength = 1)]
        public string Sexo
        {
            get => _sexo;
            set => _sexo = value.NormalizeProperty(1);
        }
        private string _sexo = string.Empty.NormalizeProperty(1);

        /// <summary>
        /// Cor do paciente. Serão aceitos os valores cadastrados no MatrixConnect. (índice = 81)
        /// </summary>
        [StringLength(1, MinimumLength = 1)]
        public string Cor
        {
            get => _cor;
            set => _cor = value.NormalizeProperty(1);
        }
        private string _cor = string.Empty.NormalizeProperty(1);

        /// <summary>
        ///Índice = 82
        /// </summary>
        public string Reservado82 { get; } = string.Empty.PadLeft(167, ' ');

        /// <summary>
        ///Índice = 249
        /// </summary>
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; } = string.Empty.PadLeft(2, ' ');

        public List<AmostraEnvio> AmostrasPaciente { get; set; } = new List<AmostraEnvio>();

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect FormatarLinha()
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Linha =
                    (int)Tipo +
                    RegistroPaciente +
                    Nome +
                    Idade +
                    DataNascimento +
                    Sexo +
                    Cor +
                    Reservado82;

                DigitosVerificacao = Util.CalcularModulo256(Linha);
                Linha += DigitosVerificacao;

                return rmc;
            }
            catch (Exception ex)
            {
                rmc.bitSucesso = false;
                rmc.Mensagem = ex.Message;

                return rmc;
            }
        }
        public bool GerarArquivoInterfaceado(out string strArquivoInterfaceado)
        {
            var sbArquivoInterfaceado = new System.Text.StringBuilder();

            try
            {
                sbArquivoInterfaceado.AppendLine(Linha);

                foreach (var AmostraColeta in AmostrasPaciente)
                    sbArquivoInterfaceado.AppendLine(AmostraColeta.Linha);

                strArquivoInterfaceado = sbArquivoInterfaceado.ToString();
                return true;
            }
            catch (Exception ex)
            {
                strArquivoInterfaceado = ex.Message;
                return false;
            }
        }
        #endregion
    }

    public class Atributo : IMatrix, IMatrixEnvio
    {
        #region Propriedades
        /// <summary>
        ///Constante "12". Indica que esta linha contém atributos da amostra. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; } = (TipoLinha)12;

        /// <summary>
        /// Identificação da amostra, atribuída pelo LIS. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        private string amostra;
        public string Amostra
        {
            get => amostra;
            set => amostra = value.NormalizeProperty(12);
        }

        /// <summary>
        ///Índice = 15
        /// </summary>
        public string Reservado15 { get; } = string.Empty.PadLeft(12, ' ');

        /// <summary>
        /// Como uma amostra pode, em determinado momento, estar vinculada a um ou mais instrumentos, o LIS pode indicar que o atributo sendo recebido aplica-se apenas ao instrumento designado neste campo. Caso este campo seja enviado pelo LIS em branco, o MatrixConnect associará o atributo a todas as ocorrências da amostra indicada, em todos os instrumentos aos quais esteja a amostra vinculada. É necessário, entretanto, que o atributo esteja definido no MatrixConnect para o instrumento em questão; caso contrário, mesmo que a amostra esteja vinculada ao instrumento, o atributo será desconsiderado. (índice = 27)
        /// </summary>
        [StringLength(6, MinimumLength = 6)]
        private string instrumento;
        public string Instrumento
        {
            get => instrumento;
            set => instrumento = value.NormalizeProperty(6);
        }

        /// <summary>
        /// Código do atributo, conforme definido no cadastro de atributos do MatrixConnect. (índice = 33)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        private string attributo;
        public string Attributo
        {
            get => attributo;
            set => attributo = value.NormalizeProperty(8);
        }

        /// <summary>
        /// Valor assumido pelo atributo. (índice = 41)
        /// </summary>
        [Required()]
        [StringLength(80, MinimumLength = 80)]
        private string valor;
        public string Valor
        {
            get => valor;
            set => valor = value.NormalizeProperty(80);
        }

        /// <summary>
        ///Índice = 121
        /// </summary>
        public string Reservado121 { get; set; } = string.Empty.PadLeft(128, ' ');

        /// <summary>
        ///Índice = 249
        /// </summary>
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; } = string.Empty.PadLeft(2, ' ');

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect FormatarLinha()
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Linha =
                    (int)Tipo +
                    Amostra +
                    Reservado15 +
                    Instrumento +
                    Attributo +
                    Valor +
                    Reservado121;

                DigitosVerificacao = Util.CalcularModulo256(Linha);
                Linha += DigitosVerificacao;

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

    public class DadosExtendidos : IMatrix, IMatrixEnvio
    {
        #region Properties
        /// <summary>
        /// Constante "13". Indica que esta linha contém outros dados da amostra (extensão da linha tipo 10). (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; } = (TipoLinha)13;

        /// <summary>
        /// Identificação da amostra, atribuída pelo LIS. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        private string amostra;
        public string Amostra
        {
            get => amostra;
            set => amostra = value.NormalizeProperty(12);
        }

        /// <summary>
        ///Índice = 15
        /// </summary>
        public string Reservado15 { get; } = string.Empty.PadLeft(12, ' ');

        /// <summary>
        /// Como uma amostra pode, em determinado momento, estar vinculada a um ou mais instrumentos, o LIS pode indicar que os dados sendo recebido aplicam-se apenas ao instrumento designado neste campo. Caso este campo seja enviado pelo LIS em branco, o MatrixConnect associará os dados a todas as ocorrências da amostra indicada, em todos os instrumentos aos quais esteja a amostra vinculada. (alguns desses dados, para serem aceitos, dependem de configuração específica no MatrixConnect). (índice = 27)
        /// </summary>
        [StringLength(6, MinimumLength = 6)]
        private string instrumento;
        public string Instrumento
        {
            get => instrumento;
            set => instrumento = value.NormalizeProperty(6);
        }

        /// <summary>
        ///  Identifica se qual é código pelo qual a amostra será referenciada no instrumento. Este campo é utilizado para instrumentos que não trabalham com a identificação da amostra propriamente dita, mas sim com uma numeração interna (geralmente um número sequencial do próprio instrumento). Este dado apenas será recebido, se no MatrixConnect o instrumento estiver configurado para trabalhar com identificação interna de amostra. Além de poder ser enviado pelo LIS, este campo poderá ser editado posteriormente pelo usuário. (índice = 33)
        /// </summary>
        [StringLength(18, MinimumLength = 18)]
        private string codigoAmostraNoInstrumento;
        public string CodigoAmostraNoInstrumento
        {
            get => codigoAmostraNoInstrumento;
            set => codigoAmostraNoInstrumento = value.NormalizeProperty(18);
        }

        /// <summary>
        ///Índice = 51
        /// </summary>
        public string Reservado51 { get; } = string.Empty.PadLeft(198, ' ');

        /// <summary>
        ///Índice = 249
        /// </summary>
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; } = string.Empty.PadLeft(2, ' ');

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect FormatarLinha()
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Linha =
                    (int)Tipo +
                    Amostra +
                    Reservado15 +
                    Instrumento +
                    CodigoAmostraNoInstrumento +
                    Reservado51;

                DigitosVerificacao = Util.CalcularModulo256(Linha);
                Linha += DigitosVerificacao;

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
    /// Opcional.
    /// </summary>
    public class ResultadoAnterior : IMatrix, IMatrixEnvio
    {
        #region Propriedades
        /// <summary>
        /// Constante "14". Indica que está linha contém um resultado anterior do paciente. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; } = (TipoLinha)14;

        /// <summary>
        ///  Identificação atual da amostra em que serão gravados os resultados anteriores. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        private string amostra;
        public string Amostra
        {
            get => amostra;
            set => amostra = value.NormalizeProperty(12);
        }

        /// <summary>
        ///Índice = 15
        /// </summary>
        public string Reservado15 { get; } = string.Empty.PadLeft(12, ' ');

        /// <summary>
        /// Identificação anterior da amostra que possui resultados anteriores. (índice = 21)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        private string identificacaoAnterior;
        public string IdentificacaoAnterior
        {
            get => identificacaoAnterior;
            set => identificacaoAnterior = value.NormalizeProperty(12);
        }

        /// <summary>
        ///Índice = 33
        /// </summary>
        public string Reservado33 { get; } = string.Empty.PadLeft(6, ' ');

        /// <summary>
        /// Campo opcional usada na gravação do resultado. Caso ele seja omitido, o MatrixConnect irá gravar o resultado anterior em todos os instrumentos em que for encontrado o mesmo número de amostra. (índice = 39)
        /// </summary>
        [StringLength(6, MinimumLength = 6)]
        private string instrumento;
        public string Instrumento
        {
            get => instrumento;
            set => instrumento = value.NormalizeProperty(6);
        }

        /// <summary>
        /// Campo opcional usado para a consistência do resultado. Caso ele seja enviado, o MatrixConnect irá verificar se o registro é o mesmo que se encontra na amostra atual que está no MatrixConnect. (índice = 45)
        /// </summary>
        [StringLength(12, MinimumLength = 12)]
        private string registroPaciente;
        public string RegistroPaciente
        {
            get => registroPaciente;
            set => registroPaciente = value.NormalizeProperty(12);
        }

        /// <summary>
        /// Identificação do exame no LIS. Caso esta identificação não esteja cadastrada no MatrixConnect, o resultado será rejeitado. (índice = 57)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        private string exameLIS;
        public string ExameLIS
        {
            get => exameLIS;
            set => exameLIS = value.NormalizeProperty(8);
        }

        /// <summary>
        /// Identificação do parâmetro no LIS. Caso esta identificação não esteja cadastrada no MatrixConnect, o resultado será rejeitado. (índice = 65)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        private string parametroLIS;
        public string ParametroLIS
        {
            get => parametroLIS;
            set => parametroLIS = value.NormalizeProperty(8);
        }

        /// <summary>
        /// Resultado do parâmetro na amostra anterior. (índice = 73)
        /// </summary>
        [Required()]
        [StringLength(80, MinimumLength = 80)]
        private string resultado;
        public string Resultado
        {
            get => resultado;
            set => resultado = value.NormalizeProperty(80);
        }

        /// <summary>
        ///Índice = 153
        /// </summary>
        public string Reservado153 { get; } = string.Empty.PadLeft(64, ' ');

        /// <summary>
        /// Data do pedido da amostra anterior. (índice = 217; formato AAAAMMDD)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        private string dataPedido;
        public string DataPedido
        {
            get => dataPedido;
            set => dataPedido = value.NormalizeProperty(8);
        }

        /// <summary>
        /// Hora do pedido da amostra anterior. (índice = 225; formato HHMM)
        /// </summary>
        [Required()]
        [StringLength(4, MinimumLength = 4)]
        private string horaPedido;
        public string HoraPedido
        {
            get => horaPedido;
            set => horaPedido = value.NormalizeProperty(4);
        }

        /// <summary>
        ///Índice = 249
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; } = string.Empty.PadLeft(2, ' ');

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect FormatarLinha()
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Linha =
                    (int)Tipo +
                    Amostra +
                    Reservado15 +
                    IdentificacaoAnterior +
                    Reservado33 +
                    Instrumento +
                    RegistroPaciente +
                    ExameLIS +
                    ParametroLIS +
                    Resultado +
                    Reservado153 +
                    DataPedido +
                    HoraPedido;

                DigitosVerificacao = Util.CalcularModulo256(Linha);
                Linha += DigitosVerificacao;

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
    /// Opcional.
    /// </summary>
    public class AtributosAmostraAnterior : IMatrix, IMatrixEnvio
    {
        #region Propriedades
        /// <summary>
        ///  Constante "15". Indica que está linha contém dados de atributo da amostra anterior. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; } = (TipoLinha)15;

        /// <summary>
        /// Identificação atual da amostra em que serão gravados os resultados anteriores (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        private string amostra;
        public string Amostra
        {
            get => amostra;
            set => amostra = value.NormalizeProperty(12);
        }

        /// <summary>
        ///Índice = 15
        /// </summary>
        public string Reservado15 { get; } = string.Empty.PadLeft(12, ' ');

        /// <summary>
        /// Identificação anterior da amostra que possui resultados anteriores. (índice = 21)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        private string identificacaoAnterior;
        public string IdentificacaoAnterior
        {
            get => identificacaoAnterior;
            set => identificacaoAnterior = value.NormalizeProperty(12);
        }

        /// <summary>
        ///Índice = 33
        /// </summary>
        public string Reservado33 { get; } = string.Empty.PadLeft(6, ' ');

        /// <summary>
        /// Campo opcional usada na gravação do resultado. Caso ele seja omitido, o MatrixConnect irá gravar o resultado anterior em todos os instrumentos em que for encontrado o mesmo número de amostra .(índice = 39)
        /// </summary>
        [StringLength(6, MinimumLength = 6)]
        private string instrumento;
        public string Instrumento
        {
            get => instrumento;
            set => instrumento = value.NormalizeProperty(6);
        }

        /// <summary>
        /// Campo opcional usado para a consistência do resultado. Caso ele seja enviado, o MatrixConnect irá verificar se o registro é o mesmo que se encontra na amostra atual que está no MatrixConnect. (índice = 45)
        /// </summary>
        [StringLength(12, MinimumLength = 12)]
        private string registroPaciente;
        public string RegistroPaciente
        {
            get => registroPaciente;
            set => registroPaciente = value.NormalizeProperty(12);
        }

        /// <summary>
        /// Identificação do atributo na amostra anterior. Caso o atributo não esteja cadastrado no MatrixConnect, o atributo será rejeitado. (índice = 57)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        private string attributo;
        public string Attributo
        {
            get => attributo;
            set => attributo = value.NormalizeProperty(8);
        }

        /// <summary>
        /// Valor do atributo na amostra anterior. (índice = 65)
        /// </summary>
        [Required()]
        [StringLength(80, MinimumLength = 80)]
        private string valor;
        public string Valor
        {
            get => valor;
            set => valor = value.NormalizeProperty(80);
        }

        /// <summary>
        ///Índice = 145
        /// </summary>
        public string Reservado145 { get; } = string.Empty.PadLeft(72, ' ');

        /// <summary>
        /// Data do pedido da amostra anterior. (índice = 217; formato AAAAMMDD)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        private string dataPedido;
        public string DataPedido
        {
            get => dataPedido;
            set => dataPedido = value.NormalizeProperty(8);
        }

        /// <summary>
        /// Hora do pedido da amostra anterior. (índice = 225; formato HHMM)
        /// </summary>
        [Required()]
        [StringLength(4, MinimumLength = 4)]
        private string horaPedido;
        public string HoraPedido
        {
            get => horaPedido;
            set => horaPedido = value.NormalizeProperty(4);
        }

        /// <summary>
        ///Índice = 249
        /// </summary>
        [Required()]
        [StringLength(2, MinimumLength = 2)]
        public string DigitosVerificacao { get; private set; } = string.Empty.PadLeft(2, ' ');

        public string Linha { get; private set; }
        #endregion

        #region Métodos
        public RetornoMatrixConnect FormatarLinha()
        {
            RetornoMatrixConnect rmc = new RetornoMatrixConnect();
            rmc.bitSucesso = true;

            try
            {
                Linha =
                    (int)Tipo +
                    Amostra +
                    Reservado15 +
                    IdentificacaoAnterior +
                    Reservado33 +
                    Instrumento +
                    RegistroPaciente +
                    Attributo +
                    Valor +
                    Reservado145 +
                    DataPedido +
                    HoraPedido;

                DigitosVerificacao = Util.CalcularModulo256(Linha);
                Linha += DigitosVerificacao;

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