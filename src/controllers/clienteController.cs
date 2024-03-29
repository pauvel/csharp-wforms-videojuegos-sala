﻿using models.db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace controllers
{
    public class clienteController
    {
        auditoriaController aCtrl = new auditoriaController();
        public async Task<clientes> obtenerPorId(int clienteId)
        {
            using (var db = new videojuegos_db())
            {
                var cliente = await db.clientes.FirstOrDefaultAsync( cl => 
                    cl.id == clienteId
                );
                return cliente;
            }
        }

        public async Task<clientes> verificarNombre(String clienteNombre)
        {
            using (var db = new videojuegos_db())
            {
                var cl = await db.clientes.FirstOrDefaultAsync(c =>
                   c.nombre == clienteNombre &&
                   c.estatus != 0
                );
                await aCtrl.auditar(new auditorias
                {
                    accion = $"Verifico el nombre {clienteNombre}.",
                    fecha =  Convert.ToDateTime( DateTime.Now.ToString("d") ),
                    estatus = 1,
                    usuario_id = globals.usuario.id
                });
                return cl != null ? cl : null;
            }
        }
        public async Task<clientes> crearNuevo( clientes cliente)
        {
            using (var db = new videojuegos_db())
            {
                var newCliente = db.clientes.Add(cliente);
                await db.SaveChangesAsync();
                await aCtrl.auditar(new auditorias
                {
                    accion = $"Creo al cliente {cliente.nombre}.",
                    fecha = Convert.ToDateTime(DateTime.Now.ToString("d")),
                    estatus = 1,
                    usuario_id = globals.usuario.id
                });
                return newCliente;
            }
        }

        public async Task<clientes> actualizar(clientes cliente)
        {
            using (var db = new videojuegos_db())
            {
                var cl = await db.clientes.FirstOrDefaultAsync(c => 
                    c.id == cliente.id
                );
                cl.nombre = cliente.nombre;
                await db.SaveChangesAsync();
                await aCtrl.auditar(new auditorias
                {
                    accion = $"Actualizo el cliente {cliente.nombre}.",
                    fecha = Convert.ToDateTime(DateTime.Now.ToString("d")),
                    estatus = 1,
                    usuario_id = globals.usuario.id
                });
                return cl;
            }
        }

        public async Task<clientes> darDeBaja(int clienteId)
        {
            using (var db = new videojuegos_db())
            {
                var cliente = await db.clientes.FirstOrDefaultAsync(u =>
                    u.id == clienteId
                );
                cliente.estatus = 0;
                await db.SaveChangesAsync();
                await aCtrl.auditar(new auditorias
                {
                    accion = $"Dio de baja al cliente {cliente.nombre}.",
                    fecha = Convert.ToDateTime(DateTime.Now.ToString("d")),
                    estatus = 1,
                    usuario_id = globals.usuario.id
                });
                return cliente;
            }
        }

        public async Task<List<clientes>> obtenerTodos()
        {
            using (var db = new videojuegos_db())
            {
                var cls = await db.clientes.Take(100)
                    .OrderByDescending(o => o.id)
                    .ToListAsync();
                await aCtrl.auditar(new auditorias
                {
                    accion = $"Obtener todos los clientes.",
                    fecha = Convert.ToDateTime(DateTime.Now.ToString("d")),
                    estatus = 1,
                    usuario_id = globals.usuario.id
                });
                return cls
                        .OrderByDescending(o => o.id)
                        .Where(u => u.estatus != 0)
                        .ToList();
            }
        }
    }
}
