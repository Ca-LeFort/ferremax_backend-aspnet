using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ApiPrincipal_Ferremas.Models;

public partial class SistemaFerremasContext : DbContext
{
    public SistemaFerremasContext()
    {
    }

    public SistemaFerremasContext(DbContextOptions<SistemaFerremasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Carrito> Carritos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Comuna> Comunas { get; set; }

    public virtual DbSet<Despacho> Despachos { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EstadoCivil> EstadoCivils { get; set; }

    public virtual DbSet<EstadoPago> EstadoPagos { get; set; }

    public virtual DbSet<EstadoPedido> EstadoPedidos { get; set; }

    public virtual DbSet<Genero> Generos { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<MedioPago> MedioPagos { get; set; }

    public virtual DbSet<Notificacion> Notificacions { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ProductoCarrito> ProductoCarritos { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Sucursal> Sucursals { get; set; }

    public virtual DbSet<TipoEmpleado> TipoEmpleados { get; set; }

    public virtual DbSet<TipoProducto> TipoProductos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=sistema_ferremas;user=ferremas;password=ferremas2025", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.IdCarrito).HasName("PRIMARY");

            entity.ToTable("CARRITO");

            entity.HasIndex(e => e.RutCliente, "rut_cliente");

            entity.Property(e => e.IdCarrito).HasColumnName("id_carrito");
            entity.Property(e => e.Estado)
                .HasMaxLength(25)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(e => e.RutCliente)
                .HasMaxLength(10)
                .HasColumnName("rut_cliente");

            entity.HasOne(d => d.RutClienteNavigation).WithMany(p => p.Carritos)
                .HasForeignKey(d => d.RutCliente)
                .HasConstraintName("CARRITO_ibfk_1");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.RutCliente).HasName("PRIMARY");

            entity.ToTable("CLIENTE");

            entity.HasIndex(e => e.IdComuna, "id_comuna");

            entity.HasIndex(e => e.IdEstCivil, "id_est_civil");

            entity.HasIndex(e => e.IdGenero, "id_genero");

            entity.HasIndex(e => e.IdNotificacion, "id_notificacion");

            entity.Property(e => e.RutCliente)
                .HasMaxLength(10)
                .HasColumnName("rut_cliente");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.IdComuna).HasColumnName("id_comuna");
            entity.Property(e => e.IdEstCivil).HasColumnName("id_est_civil");
            entity.Property(e => e.IdGenero).HasColumnName("id_genero");
            entity.Property(e => e.IdNotificacion).HasColumnName("id_notificacion");
            entity.Property(e => e.PApellido)
                .HasMaxLength(50)
                .HasColumnName("p_apellido");
            entity.Property(e => e.PNombre)
                .HasMaxLength(50)
                .HasColumnName("p_nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.SApellido)
                .HasMaxLength(50)
                .HasColumnName("s_apellido");
            entity.Property(e => e.SNombre)
                .HasMaxLength(50)
                .HasColumnName("s_nombre");
            entity.Property(e => e.Telefono).HasColumnName("telefono");

            entity.HasOne(d => d.IdComunaNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdComuna)
                .HasConstraintName("CLIENTE_ibfk_3");

            entity.HasOne(d => d.IdEstCivilNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdEstCivil)
                .HasConstraintName("CLIENTE_ibfk_2");

            entity.HasOne(d => d.IdGeneroNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdGenero)
                .HasConstraintName("CLIENTE_ibfk_1");

            entity.HasOne(d => d.IdNotificacionNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdNotificacion)
                .HasConstraintName("CLIENTE_ibfk_4");
        });

        modelBuilder.Entity<Comuna>(entity =>
        {
            entity.HasKey(e => e.IdComuna).HasName("PRIMARY");

            entity.ToTable("COMUNA");

            entity.HasIndex(e => e.IdRegion, "id_region");

            entity.Property(e => e.IdComuna).HasColumnName("id_comuna");
            entity.Property(e => e.IdRegion).HasColumnName("id_region");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdRegionNavigation).WithMany(p => p.Comunas)
                .HasForeignKey(d => d.IdRegion)
                .HasConstraintName("COMUNA_ibfk_1");
        });

        modelBuilder.Entity<Despacho>(entity =>
        {
            entity.HasKey(e => e.IdDespacho).HasName("PRIMARY");

            entity.ToTable("DESPACHO");

            entity.Property(e => e.IdDespacho).HasColumnName("id_despacho");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.RutEmpleado).HasName("PRIMARY");

            entity.ToTable("EMPLEADO");

            entity.HasIndex(e => e.IdComuna, "id_comuna");

            entity.HasIndex(e => e.IdEstCivil, "id_est_civil");

            entity.HasIndex(e => e.IdGenero, "id_genero");

            entity.HasIndex(e => e.IdTipoEmp, "id_tipo_emp");

            entity.Property(e => e.RutEmpleado)
                .HasMaxLength(10)
                .HasColumnName("rut_empleado");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FechaContrato).HasColumnName("fecha_contrato");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.IdComuna).HasColumnName("id_comuna");
            entity.Property(e => e.IdEstCivil).HasColumnName("id_est_civil");
            entity.Property(e => e.IdGenero).HasColumnName("id_genero");
            entity.Property(e => e.IdTipoEmp).HasColumnName("id_tipo_emp");
            entity.Property(e => e.PApellido)
                .HasMaxLength(50)
                .HasColumnName("p_apellido");
            entity.Property(e => e.PNombre)
                .HasMaxLength(50)
                .HasColumnName("p_nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.SApellido)
                .HasMaxLength(50)
                .HasColumnName("s_apellido");
            entity.Property(e => e.SNombre)
                .HasMaxLength(50)
                .HasColumnName("s_nombre");
            entity.Property(e => e.Telefono).HasColumnName("telefono");

            entity.HasOne(d => d.IdComunaNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdComuna)
                .HasConstraintName("EMPLEADO_ibfk_3");

            entity.HasOne(d => d.IdEstCivilNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdEstCivil)
                .HasConstraintName("EMPLEADO_ibfk_2");

            entity.HasOne(d => d.IdGeneroNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdGenero)
                .HasConstraintName("EMPLEADO_ibfk_1");

            entity.HasOne(d => d.IdTipoEmpNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdTipoEmp)
                .HasConstraintName("EMPLEADO_ibfk_4");
        });

        modelBuilder.Entity<EstadoCivil>(entity =>
        {
            entity.HasKey(e => e.IdEstCivil).HasName("PRIMARY");

            entity.ToTable("ESTADO_CIVIL");

            entity.Property(e => e.IdEstCivil).HasColumnName("id_est_civil");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoPago>(entity =>
        {
            entity.HasKey(e => e.IdEstPago).HasName("PRIMARY");

            entity.ToTable("ESTADO_PAGO");

            entity.Property(e => e.IdEstPago).HasColumnName("id_est_pago");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoPedido>(entity =>
        {
            entity.HasKey(e => e.IdEstPedido).HasName("PRIMARY");

            entity.ToTable("ESTADO_PEDIDO");

            entity.Property(e => e.IdEstPedido).HasColumnName("id_est_pedido");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Genero>(entity =>
        {
            entity.HasKey(e => e.IdGenero).HasName("PRIMARY");

            entity.ToTable("GENERO");

            entity.Property(e => e.IdGenero).HasColumnName("id_genero");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.IdMarca).HasName("PRIMARY");

            entity.ToTable("MARCA");

            entity.Property(e => e.IdMarca).HasColumnName("id_marca");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<MedioPago>(entity =>
        {
            entity.HasKey(e => e.IdMedioPago).HasName("PRIMARY");

            entity.ToTable("MEDIO_PAGO");

            entity.Property(e => e.IdMedioPago).HasColumnName("id_medio_pago");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PRIMARY");

            entity.ToTable("NOTIFICACION");

            entity.Property(e => e.IdNotificacion).HasColumnName("id_notificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PRIMARY");

            entity.ToTable("PAGO");

            entity.HasIndex(e => e.IdEstPago, "id_est_pago");

            entity.HasIndex(e => e.IdMedioPago, "id_medio_pago");

            entity.HasIndex(e => e.IdPedido, "id_pedido");

            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.FechaPago).HasColumnName("fecha_pago");
            entity.Property(e => e.IdEstPago).HasColumnName("id_est_pago");
            entity.Property(e => e.IdMedioPago).HasColumnName("id_medio_pago");
            entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
            entity.Property(e => e.Monto).HasColumnName("monto");
            entity.Property(e => e.Referencia)
                .HasMaxLength(255)
                .HasColumnName("referencia");

            entity.HasOne(d => d.IdEstPagoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdEstPago)
                .HasConstraintName("PAGO_ibfk_3");

            entity.HasOne(d => d.IdMedioPagoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdMedioPago)
                .HasConstraintName("PAGO_ibfk_2");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdPedido)
                .HasConstraintName("PAGO_ibfk_1");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PRIMARY");

            entity.ToTable("PEDIDO");

            entity.HasIndex(e => e.IdCarrito, "id_carrito");

            entity.HasIndex(e => e.IdDespacho, "id_despacho");

            entity.HasIndex(e => e.IdEstPedido, "id_est_pedido");

            entity.HasIndex(e => e.IdSucursal, "id_sucursal");

            entity.HasIndex(e => e.RutCliente, "rut_cliente");

            entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
            entity.Property(e => e.FechaPedido).HasColumnName("fecha_pedido");
            entity.Property(e => e.IdCarrito).HasColumnName("id_carrito");
            entity.Property(e => e.IdDespacho).HasColumnName("id_despacho");
            entity.Property(e => e.IdEstPedido).HasColumnName("id_est_pedido");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.PrecioTotal).HasColumnName("precio_total");
            entity.Property(e => e.RutCliente)
                .HasMaxLength(10)
                .HasColumnName("rut_cliente");

            entity.HasOne(d => d.IdCarritoNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdCarrito)
                .HasConstraintName("PEDIDO_ibfk_1");

            entity.HasOne(d => d.IdDespachoNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdDespacho)
                .HasConstraintName("PEDIDO_ibfk_4");

            entity.HasOne(d => d.IdEstPedidoNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdEstPedido)
                .HasConstraintName("PEDIDO_ibfk_3");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("PEDIDO_ibfk_5");

            entity.HasOne(d => d.RutClienteNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.RutCliente)
                .HasConstraintName("PEDIDO_ibfk_2");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PRIMARY");

            entity.ToTable("PRODUCTO");

            entity.HasIndex(e => e.IdMarca, "id_marca");

            entity.HasIndex(e => e.IdTipoProd, "id_tipo_prod");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdMarca).HasColumnName("id_marca");
            entity.Property(e => e.IdTipoProd).HasColumnName("id_tipo_prod");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(255)
                .HasColumnName("imagen_url");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio).HasColumnName("precio");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdMarca)
                .HasConstraintName("PRODUCTO_ibfk_1");

            entity.HasOne(d => d.IdTipoProdNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdTipoProd)
                .HasConstraintName("PRODUCTO_ibfk_2");
        });

        modelBuilder.Entity<ProductoCarrito>(entity =>
        {
            entity.HasKey(e => e.IdProdCarrito).HasName("PRIMARY");

            entity.ToTable("PRODUCTO_CARRITO");

            entity.HasIndex(e => e.IdCarrito, "id_carrito");

            entity.HasIndex(e => e.IdProducto, "id_producto");

            entity.Property(e => e.IdProdCarrito).HasColumnName("id_prod_carrito");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdCarrito).HasColumnName("id_carrito");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");

            entity.HasOne(d => d.IdCarritoNavigation).WithMany(p => p.ProductoCarritos)
                .HasForeignKey(d => d.IdCarrito)
                .HasConstraintName("PRODUCTO_CARRITO_ibfk_2");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductoCarritos)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("PRODUCTO_CARRITO_ibfk_1");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.IdRegion).HasName("PRIMARY");

            entity.ToTable("REGION");

            entity.Property(e => e.IdRegion).HasColumnName("id_region");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal).HasName("PRIMARY");

            entity.ToTable("SUCURSAL");

            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoEmpleado>(entity =>
        {
            entity.HasKey(e => e.IdTipoEmp).HasName("PRIMARY");

            entity.ToTable("TIPO_EMPLEADO");

            entity.Property(e => e.IdTipoEmp).HasColumnName("id_tipo_emp");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoProducto>(entity =>
        {
            entity.HasKey(e => e.IdTipoProd).HasName("PRIMARY");

            entity.ToTable("TIPO_PRODUCTO");

            entity.Property(e => e.IdTipoProd).HasColumnName("id_tipo_prod");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
