using System.Collections.Generic;
using Microsoft.Data.SqlClient;

public class Gasto
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = "";
    public decimal Monto { get; set; }
    public string Categoria { get; set; } = "";
}

public class GastoRepositorio
{
    private readonly string _cs;
    public GastoRepositorio(string connectionString) => _cs = connectionString;

    // --- Ya lo tienes de la Clase 11: Insertar y Listar (se muestra Insertar/Listar como referencia) ---
    public void Insertar(Gasto g)
    {
        using var cn = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            "INSERT INTO Gastos (Descripcion, Monto, Categoria) VALUES (@d, @m, @c)", cn);
        cmd.Parameters.AddWithValue("@d", g.Descripcion);
        cmd.Parameters.AddWithValue("@m", g.Monto);
        cmd.Parameters.AddWithValue("@c", g.Categoria);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    public List<Gasto> Listar()
    {
        var lista = new List<Gasto>();
        using var cn = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            "SELECT Id, Descripcion, Monto, Categoria FROM Gastos ORDER BY Id", cn);
        cn.Open();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            lista.Add(new Gasto
            {
                Id = rd.GetInt32(0),
                Descripcion = rd.GetString(1),
                Monto = rd.GetDecimal(2),
                Categoria = rd.GetString(3)
            });
        }
        return lista;
    }

    // --- NUEVOS ---
    public Gasto? BuscarPorId(int id)
    {
        using var cn = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            "SELECT Id, Descripcion, Monto, Categoria FROM Gastos WHERE Id = @id", cn);
        cmd.Parameters.AddWithValue("@id", id);
        cn.Open();
        using var rd = cmd.ExecuteReader();
        if (!rd.Read()) return null;
        return new Gasto
        {
            Id = rd.GetInt32(0),
            Descripcion = rd.GetString(1),
            Monto = rd.GetDecimal(2),
            Categoria = rd.GetString(3)
        };
    }

    public bool Actualizar(Gasto g)
    {
        using var cn = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            "UPDATE Gastos SET Descripcion = @d, Monto = @m, Categoria = @c WHERE Id = @id", cn);
        cmd.Parameters.AddWithValue("@d", g.Descripcion);
        cmd.Parameters.AddWithValue("@m", g.Monto);
        cmd.Parameters.AddWithValue("@c", g.Categoria);
        cmd.Parameters.AddWithValue("@id", g.Id);
        cn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var cn = new SqlConnection(_cs);
        using var cmd = new SqlCommand("DELETE FROM Gastos WHERE Id = @id", cn);
        cmd.Parameters.AddWithValue("@id", id);
        cn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }
}

public class Program
{
    // Para el escenario 5, cambia temporalmente "GastosDB" por un nombre inexistente.
    private const string Cs =
        "Server=localhost;Database=GastosDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public static void Main()
    {
        var repo = new GastoRepositorio(Cs);
        int opcion;

        do
        {
            Console.WriteLine();
            Console.WriteLine("=== CRUD DE GASTOS ===");
            Console.WriteLine("1. Registrar gasto");
            Console.WriteLine("2. Listar gastos");
            Console.WriteLine("3. Buscar gasto por Id");
            Console.WriteLine("4. Actualizar monto");
            Console.WriteLine("5. Eliminar gasto");
            Console.WriteLine("6. Salir");
            Console.Write("Opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Opción inválida.");
                opcion = 0;
                continue;
            }

            switch (opcion)
            {
                case 1: Registrar(repo); break;
                case 2: Listar(repo); break;
                case 3: Buscar(repo); break;
                case 4: ActualizarMonto(repo); break;
                case 5: Eliminar(repo); break;
                case 6: Console.WriteLine("Hasta luego."); break;
                default: Console.WriteLine("Opción inválida."); break;
            }
        } while (opcion != 6);
    }

    static void Registrar(GastoRepositorio repo)
    {
        try
        {
            Console.Write("Descripción: ");
            string desc = Console.ReadLine() ?? "";
            Console.Write("Monto: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal monto) || monto <= 0)
            {
                Console.WriteLine("Monto inválido.");
                return;
            }
            Console.Write("Categoría: ");
            string cat = Console.ReadLine() ?? "";

            repo.Insertar(new Gasto { Descripcion = desc, Monto = monto, Categoria = cat });
            Console.WriteLine("Gasto registrado.");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Error al registrar: " + ex.Message);
        }
    }

    static void Listar(GastoRepositorio repo)
    {
        try
        {
            var gastos = repo.Listar();
            decimal total = 0;
            foreach (var g in gastos)
            {
                Console.WriteLine($"{g.Id}. {g.Descripcion} - Q {g.Monto:F2} - {g.Categoria}");
                total += g.Monto;
            }
            Console.WriteLine($"TOTAL GASTADO: Q {total:F2}");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Error al listar: " + ex.Message);
        }
    }

    static void Buscar(GastoRepositorio repo)
    {
        try
        {
            Console.Write("Id: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Id inválido.");
                return;
            }
            var g = repo.BuscarPorId(id);
            if (g == null)
                Console.WriteLine("No existe ese Id.");
            else
                Console.WriteLine($"{g.Descripcion}, Q {g.Monto:F2}, {g.Categoria}");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Error al buscar: " + ex.Message);
        }
    }

    static void ActualizarMonto(GastoRepositorio repo)
    {
        try
        {
            Console.Write("Id: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Id inválido.");
                return;
            }
            var g = repo.BuscarPorId(id);
            if (g == null)
            {
                Console.WriteLine("No existe ese Id.");
                return;
            }

            Console.WriteLine($"Actual: {g.Descripcion}, Q {g.Monto:F2}, {g.Categoria}");
            Console.Write("Nuevo monto: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal nuevo) || nuevo <= 0)
            {
                Console.WriteLine("Monto inválido.");
                return;
            }

            g.Monto = nuevo;
            Console.WriteLine(repo.Actualizar(g) ? "Gasto actualizado." : "No existe ese Id.");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Error al actualizar: " + ex.Message);
        }
    }

    static void Eliminar(GastoRepositorio repo)
    {
        try
        {
            Console.Write("Id: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Id inválido.");
                return;
            }
            Console.WriteLine(repo.Eliminar(id) ? "Gasto eliminado." : "No existe ese Id.");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Error al eliminar: " + ex.Message);
        }
    }
}
