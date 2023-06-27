using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using FlexoCotizaciones.Models;

namespace FlexoCotizaciones;


public class ParametrosRepository
{
    string _dbPath;

    public string StatusMessage { get; set; }

    private SQLiteConnection conn;

    private void Init()
    {
        if (conn is not null)
            return;
        conn = new(_dbPath);
        conn.CreateTable<Parametros>();
    }

    public ParametrosRepository(string dbPath)
    {
        _dbPath = dbPath;
    }

    public void AddNewParametro(string name, string precio, string valordesde, string valorhasta,string tipo)
    {
        int result = 0;
        try
        {
            Init();



            // basic validation to ensure a name was entered
            if (string.IsNullOrEmpty(name))
                throw new Exception("Nombre de parámetro requerido");
            // basic validation to ensure a name was entered
            if (string.IsNullOrEmpty(precio))
                throw new Exception("valor de parámetro requerido");

            // TODO: Insert the new person into the database
            Parametros parametros = new Parametros() { Nombre = name, Precio = long.Parse(precio), Valordesde = long.Parse(valordesde), ValorHasta = long.Parse(valorhasta),Tipo = int.Parse(tipo) };
            result = conn.Insert(parametros);

            StatusMessage = string.Format("{0} record(s) added (Name: {1})", result, name);
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to add {0}. Error: {1}", name, ex.Message);
        }

    }


    public void deleteAll()
    {
        try
        {
            Init();
            conn.Execute("TRUNCATE table Parametros");
                     

        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }
       
    }


    public List<Parametros> GetAllPeople()
    {
        // TODO: Init then retrieve a list of Person objects from the database into a list
        try
        {
            Init();
            return conn.Table<Parametros>().ToList();

        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }

        return new List<Parametros>();
    }
}

