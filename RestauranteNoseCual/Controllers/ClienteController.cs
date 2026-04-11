using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestauranteNoseCual.Base_de_Datos;
using RestauranteNoseCual.Models;
using RestauranteNoseCual.Services;

namespace RestauranteNoseCual.Controllers
{
    public  class Control_Cliente
    {
        Services.ClienteService _clienteService = new();
        public async Task<string> ObtenerRolRealAsync(string correo)
        {
            try
            {
                var cliente = await _clienteService.ObtenerPorCorreoAsync(correo);
                if (cliente != null)
                {
                    return cliente.Rol; // Retorna "Admin", "Mesero" o "Cliente"
                }
                return "Cliente";
            }
            catch (Exception)
            {
                return "Cliente";
            }
        }

        public async Task<Cliente> ObtenerCliente(string Telefono)
        {

            try
            {
                var cliente = await _clienteService.BuscarPorTelefonoAsync(Telefono);
                if (cliente != null) 
                {
                    return cliente;
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }
    }
}
