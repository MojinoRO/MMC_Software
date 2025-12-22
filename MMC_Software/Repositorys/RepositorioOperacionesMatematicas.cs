using System;

namespace MMC_Software
{
    public class RepositorioOperacionesMatematicas
    {

        public decimal CalcularValorMasIva(decimal ValorBase, decimal Iva)
        {
            decimal Resultado = ValorBase * (1 + (Iva / 100));
            return Resultado;
        }

        public decimal CalcularValirSinIva(decimal ValorBase, decimal Iva)
        {
            decimal Resultado = ValorBase / (1 + (Iva / 100));
            return Resultado;

        }

        public decimal CalcularValorIva(decimal ValorBase, decimal Iva)
        {
            decimal Resultado = ValorBase * (Iva / 100);
            return Resultado;
        }


        public decimal CalcularValorVentaIncremento(int TipoRedondeo, decimal CostoSinIva, decimal Incremento, decimal iva)
        {
            // el parametro que se le pone de tipo de redondeo va ser para que redondie o no 
            // 1 sin redondeo 
            // 2 con redondeo
            decimal VentaMasUtilidad = CostoSinIva * (1 + (Incremento / 100));
            decimal Resultado = VentaMasUtilidad * (1 + iva / 100);

            decimal ValorRedondeado;
            if (TipoRedondeo == 2)
            {
                decimal BaseInferior = Math.Floor(Resultado / 100) * 100;
                decimal Diferencia = Resultado - BaseInferior;

                if (Diferencia <= 50)
                {
                    ValorRedondeado = BaseInferior + 50;
                }
                else
                {
                    ValorRedondeado = BaseInferior + 100;
                }
            }
            else
            {
                ValorRedondeado = Resultado;
            }

            return ValorRedondeado;
        }
    }
}
