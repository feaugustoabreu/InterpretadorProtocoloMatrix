using System;

namespace InterpretadorProtocoloMatrix.Interfaces
{
    public interface IMatrix
    {
        #region Propriedades
        TipoLinha Tipo { get; }

        string DigitosVerificacao { get; }

        string Linha { get; }
        #endregion
    }

    public interface IMatrixEnvio
    {
        #region Métodos
        RetornoMatrixConnect FormatarLinha();
        #endregion
    }

    public interface IMatrixRecebimento
    {
        #region Métodos
        RetornoMatrixConnect ImportarLinha(string linha);
        #endregion
    }
}