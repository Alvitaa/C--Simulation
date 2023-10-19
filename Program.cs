using System;
using System.IO;

class simulacion
{
    static void Main()
    {
        //Se inicializan algunas variables.        
        int contador = 0;
        int contadorP = 0;

        //Se borra en caso halla un archivo seres anterior.
        File.Delete("seres.txt");

        //---------------GENERACIÓN---------------//

        //Se generan los 100 seres iniciales.
        for (int i = 0; i < 100; i++)
        {
            //Se genera un ser
            generacion();
            //Se cuenta cuantos seres se generan en la simulacion.
            contador++;
        }
        Console.WriteLine($"Se crearon los {contador} seres unicelulares iniciales.");
        Console.WriteLine($"Iniciando la simulación de 100 iteraciones.\n");

        //---------------SIMULACION---------------//

        //Se realiza 100 veces
        for (int i = 1; i < 101; i++)
        {
            Console.Write($"En la iteración [{i}]: ");
            //ETAPA DE REPRODUCCIÓN

            contador += reproduccion();

            //ETAPA DE PURGACIÓN

            if (contador > 50)
            {
                contadorP = purga();
                Console.Write($"fueron purgados {contador - contadorP}, ");
                contador = contadorP;
            }
            else
            {
                Console.Write("no hubo purgados, ");
            }

            //ETAPA DE MUTACIÓN

            mutacion();

            Console.WriteLine($"quedan {contador} seres.");

            //En caso todos los seres mueran, la simulación no cambiará los resultados en las siguientes iteraciones
            if(contador == 0){
            Console.WriteLine("\nMurieron todos los seres.");
                Console.WriteLine($"Se detuvo la simulacion en la iteración [{i}].");
                break;
            }else if(contador == 1){    //En caso sólo quede un ser, la simulación no cambiará porque no habrá reproducción.
                Console.WriteLine("\nQuedó sólo un ser, imposibilitando la reproducción.");
                Console.WriteLine($"Se detuvo la simulacion en la iteración [{i}].");
                break;
            }
        }
    }

    //Convierte una string en un arreglo char
    public static char[] arreglo_char(String ser)
    {
        //Se declara un arreglo de char
        char[] conversion = new char[ser.Length];

        for(int i = 0; i < ser.Length; i++)
        {
            //se extrae de una cadena un string y luego se convierte a char
            conversion[i] = Char.Parse(ser.Substring(i, 1));
        }
        return conversion;
    }

    public static int comparar(char[] a,char b)
    {
        //Se declara una variable
        int cont = 0;
        for(int i = 0; i < a.Length; i++)
        {
            //Compara si arreglo del char es igual a char que se ingresa como parametro
            if (a[i].Equals(b))
                //Incrementa el contador para saber cuantas veces se repite
                cont++;
        }
        return cont;
    }

    //Genera un ser unicelular con sus bases.
    public static void generacion()
    {
        //crea una cadena de string vacía
        String ser = "";
        //generar aleatoriamente una base del ser unicelular.
        for (int i = 0; i < 30; i++)
        {
            Random rand = new Random();
            Char letra;
            //genera un número entero entre 0 y 100.
            int nro = rand.Next(0, 101);
            //dependiendo del resultado se seleccionará una base u otra.
            if (nro <= 40)
            {
                letra = 'A';
            }
            else if (nro <= 70)
            {
                letra = 'T';
            }
            else if (nro <= 85)
            {
                letra = 'U';
            }
            else if (nro <= 95)
            {
                letra = 'C';
            }
            else
            {
                letra = 'G';
            }
            //se añade la base a la cadena del ser unicelular.
            ser = ser + letra;
        }
        //Guarda al ser dentro de un archivo de texto.
        guardar(ser);
    }

    //Devuelve un valor booleano en caso cumpla los requisitos para reproducirse.
    public static Boolean requisitos(String ser)
    {
        //Cuenta la cantidad de bases A y T dentro del ser
        //int cont = ser.ToArray().Where(lr => lr.Equals('A')).Count() + ser.ToArray().Where(lr => lr.Equals('T')).Count();
        int cont = comparar(arreglo_char(ser), 'A') + comparar(arreglo_char(ser), 'T');
        //Si la proporción de A y T del ser unicelular es más del 40%, se devuelve verdadero. 
        if (cont >= 12)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Desarrolla la etapa de reproducción.
    public static int reproduccion()
    {
        //Inicializa algunas variables
        String? ser1 = "", ser2 = "";
        String resultados = "";
        int c = 0;

        //Abre el archivo en modo lectura
        StreamReader seres = new StreamReader("seres.txt");

        //Lee los dos primeros seres
        ser1 = seres.ReadLine();
        ser2 = seres.ReadLine();
        while (ser1 != null && ser2 != null)
        {
            //Revisa si cumplen los requisitos para reproducirse.
            if (requisitos(ser1) && requisitos(ser2))
            {
                String? nuevo = "";
                Random rand = new Random();
                //crea dos cortes aleatorios
                int c1 = rand.Next(1, 15);
                int c2 = rand.Next(1, 15);
                //Crea un nuevo ser unicelular con los extremos del segundo.
                nuevo = reproducir(ser1, ser2, c1, c2);
                if (c == 0)
                {
                    resultados = nuevo;
                }
                else resultados = resultados + "\n" + nuevo;
                //Crea un nuevo ser unicelular con los extremos del primero.
                nuevo = reproducir(ser2, ser1, c1, c2);
                resultados = resultados + "\n" + nuevo;
                c += 2;
            }
            //Lee los dos seres siguientes.
            ser1 = seres.ReadLine();
            ser2 = seres.ReadLine();
        }
        //se cierra el archivo .txt
        seres.Close();

        //si hubo reproducción, se guardan los seres reproducidos.
        if (c > 0)
        {
            guardar(resultados);
            Console.Write($"nacieron {c} seres, ");
        }
        //retorna la cantidad de nuevos seres reproducidos.
        return c;
    }

    //Reproduce dos seres unicelulares.
    public static String reproducir(String ser1, String ser2, int corte1, int corte2)
    {
        //se inicializan las variables
        String nuevo = "";

        //Convierte los string de los seres en cadenas de caracteres.
        char[] s1 = arreglo_char(ser1);
        char[] s2 = arreglo_char(ser2);

        //Crea un nuevo ser unicelular.
        for (int i = 0; i < 30; i++)
        {
            if (i <= corte1)
            {
                nuevo = nuevo + s2[i];
            }
            else if (i <= 30 - (corte1 + corte2))
            {
                nuevo = nuevo + s1[i];
            }
            else nuevo = nuevo + s2[i];
        }
        //Retorna el nuevo ser unicelular
        return nuevo;
    }

    //Desarrolla la etapa de purga.
    public static int purga()
    {
        //se abre el archivo de texto en modo lectura, se crea un archivo temporal y se inicializan algunas variables.
        StreamReader seres = new StreamReader("seres.txt");
        var tempFile = Path.GetTempFileName();
        var sobrevivientes = "";

        String? linea;
        int c = 0;

        linea = seres.ReadLine();
        //recorre cada linea del archivo de texto
        while (linea != null)
        {
            String ser = linea;
            //revisa si el ser unicelular cumple los requisitos para NO ser purgado.
            if (!purgar(ser))
            {
                //en caso el ser no es purgado, lo agrega como texto a la variable sobrevivientes
                if (c == 0)
                {
                    sobrevivientes = ser;
                    c++;
                }
                else
                {
                    sobrevivientes = sobrevivientes + "\n" + ser;
                    c++;
                }
            }
            linea = seres.ReadLine();
        }
        //cierra el archivo de texto.
        seres.Close();
        //copia el texto de la variable string en el archivo temporal.
        File.WriteAllText(tempFile, sobrevivientes);
        //Borra el archivo de texto original y lo reemplaza con el archivo temporal
        File.Delete("seres.txt");
        File.Move(tempFile, "seres.txt");
        return c;
    }

    //Indica si un ser es purgado o no.
    public static Boolean purgar(String ser)
    {
        //Convierte el string ser a un array
        char[] c = arreglo_char(ser);
        //Revisa cada caracter del ser
        for (int i = 0; i < ser.Length; i++)
        {
            //Si tiene la base U, debe ser purgado
            if (c[i] == 'U')
            {
                return true;
            }
        }
        return false;
    }

    //Desarrolla la etapa de mutación.
    public static void mutacion()
    {
        //se abre el archivo de texto en modo lectura, se crea un archivo temporal y se inicializan algunas variables.
        StreamReader seres = new StreamReader("seres.txt");
        var tempFile = Path.GetTempFileName();
        var mutaciones = "";

        String? linea;
        int c = 0;

        linea = seres.ReadLine();
        //recorre cada linea del archivo de texto
        while (linea != null)
        {
            String ser = linea;
            //"muta" cada ser unicelular del archivo de texto y lo guarda en una variable string
            if (c == 0)
            {
                mutaciones = mutar(ser);
                c++;
            }
            else
            {
                mutaciones = mutaciones + "\n" + mutar(ser);
                c++;
            }
            linea = seres.ReadLine();
        }
        //cierra el archivo
        seres.Close();
        //copia el texto de la variable string en el archivo temporal.
        File.WriteAllText(tempFile, mutaciones);
        //Borra el archivo de texto original y lo reemplaza con el archivo temporal
        File.Delete("seres.txt");
        File.Move(tempFile, "seres.txt");
    }

    //Devuelve un ser mutado;
    public static String mutar(String ser)
    {
        //convierte el string ser en un array de caracteres
        char[] c = arreglo_char(ser);
        String nuevo = "";
        //recorre cada caracter
        for (int i = 0; i < ser.Length; i++)
        {
            Random rand = new Random();
            int nro = rand.Next(1, 101);
            //tiene un 5% de probabilidad de que el caracter(base) se convierta en un elemento "U".
            if (nro <= 5)
            {
                nuevo = nuevo + 'U';
            }
            else nuevo = nuevo + c[i]; //en caso no entre en el 5%, no se cambia la base.
        }
        //se retorna el nuevo ser mutado.
        return nuevo;
    }

    //Guarda un ser unicelular en una línea de un archivo de texto.
    public static void guardar(String ser)
    {
        //Se abre el archivo de texto en modo "adición" de escritura
        StreamWriter seres = new StreamWriter("seres.txt", true);
        //Escribe al ser dentro del archivo de texto
        seres.WriteLine(ser);
        //Cierra el archivo seres
        seres.Close();
    }

    //Una función que imprime todos los seres del archivo de texto.
    public static void imprimir()
    {
        //Abre el archivo en modo de lectura
        StreamReader seres = new StreamReader("seres.txt");

        //Se inicializa la variable linea
        String? linea;
        linea = seres.ReadLine();

        //Recorre cada linea del archivo de texto
        while (linea != null)
        {
            //Imprime en la consola cada ser
            Console.WriteLine(linea);
            linea = seres.ReadLine();
        }
    }
}