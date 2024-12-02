using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmacia
{
    namespace enums
    {
        public enum piel_t
        {
            Mixta,
            Seca,
            Grasa,
            Sensible
        }

        public enum MetodoPago_t
        {
            Efectivo,
            Credito,
            Debito,
            Transferencia
        }

        public enum turno_t
        {
            Diurno,
            Vespertino
        }

        public enum tipoCliente_t
        {
            Regular,
            Frecuente,
            VIP
        }
    }
}
