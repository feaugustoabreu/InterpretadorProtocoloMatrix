using System;
using System.ComponentModel.DataAnnotations;
using InterpretadorProtocoloMatrix.Interfaces;

namespace InterpretadorProtocoloMatrix.Log
{
    /// <summary>
    /// Layout do arquivo que será devolvido pelo MatrixConnect ao LIS, contendo uma ou mais ocorrências de log
    /// </summary>
    public class OcorrenciaLog : IMatrix, IMatrixRecebimento
    {
        #region Propriedades
        /// <summary>
        ///Constante "30" indica que esta linha contém uma ocorrência de log. (índice = 1)
        /// </summary>
        public TipoLinha Tipo { get; private set; } = (TipoLinha)30;

        /// <summary>
        /// Tipo de log desta ocorrência. A tabela de tipos de log pode ser requisitada ao suporte da Matrix. (índice = 3)
        /// </summary>
        [Required()]
        [StringLength(3, MinimumLength = 3)]
        public string TipoLog { get; set; }

        /// <summary>
        /// Descrição do tipo de ocorrência. (índice = 6)
        /// </summary>
        [Required()]
        [StringLength(80, MinimumLength = 80)]
        public string Descricao { get; set; }

        /// <summary>
        /// Data da ocorrência. (índice = 86)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string DataOcorrencia { get; set; }

        /// <summary>
        /// Hora da ocorrência. (índice = 94)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string HoraOcorrencia { get; set; }

        /// <summary>
        /// Instrumento em que ocorreu a ocorrência. (índice = 99)
        /// </summary>
        [Required()]
        [StringLength(6, MinimumLength = 6)]
        public string Instrumento { get; set; }

        /// <summary>
        /// Porta do MatrixConnect associada a ocorrência. (índice = 105)
        /// </summary>
        [Required()]
        [StringLength(6, MinimumLength = 6)]
        public string Porta { get; set; }

        /// <summary>
        /// Amostra associada a ocorrência. (índice = 111)
        /// </summary>
        [Required()]
        [StringLength(12, MinimumLength = 12)]
        public string Amostra { get; set; }

        /// <summary>
        /// (índice = 123)
        /// </summary>
        public string Reservado123 { get; } = string.Empty.PadLeft(6, ' ');

        /// <summary>
        /// Usuário que gerou a ocorrência. (índice = 129)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string Usuario { get; set; }

        /// <summary>
        /// (índice = 137)
        /// </summary>
        [Required()]
        [StringLength(8, MinimumLength = 8)]
        public string DataExportacao { get; set; }

        /// <summary>
        /// (índice = 145)
        /// </summary>
        [Required()]
        [StringLength(5, MinimumLength = 5)]
        public string HoraExportacao { get; set; }

        /// <summary>
        /// (índice = 150)
        /// </summary>
        [Required()]
        [StringLength(104, MinimumLength = 104)]
        public string Detalhes { get; set; }

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
                TipoLog = linha.OneBasedSubString(3, 3);
                Descricao = linha.OneBasedSubString(6, 80);
                DataOcorrencia = linha.OneBasedSubString(86, 8);
                HoraOcorrencia = linha.OneBasedSubString(94, 5);
                Instrumento = linha.OneBasedSubString(99, 6);
                Porta = linha.OneBasedSubString(105, 6);
                Amostra = linha.OneBasedSubString(111, 12);
                Usuario = linha.OneBasedSubString(129, 8);
                DataExportacao = linha.OneBasedSubString(137, 8);
                HoraExportacao = linha.OneBasedSubString(145, 5);
                Detalhes = linha.OneBasedSubString(150, 104);
                DigitosVerificacao = linha.OneBasedSubString(254, 2);
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