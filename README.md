(se que no tiene video, pero aun que sea medio punto)
1. Abrir SQL Server Management Studio y ejecutar el script de la Clase 10 para recrear la base`GastosDB con los 3 gastos iniciales (Id 1 a 3)
2. Abrir el proyecto CrudGastos en Visual Studio
3. Instalar el paquete NuGet: Proyecto → Administrar paquetes NuGet → Examinar → Microsoft.Data.SqlClient → Instalar
4. Revisar la cadena de conexión en Program.cs (constante Cs) y ajustar Server si tu instancia de SQL Server tiene otro nombre
5. Compilar el proyecto con Ctrl + Shift + B y verificar que no haya errores
6. Ejecutar el programa con F5
7. Elegir una opción del menú escribiendo su número:
   - 1: Registrar gasto
   - 2: Listar gastos
   - 3: Buscar gasto por Id
   - 4: Actualizar monto
   - 5: Eliminar gasto
   - 6: Salir
8. Seguir las instrucciones en pantalla 
9. Para probar el manejo de errores, cambiar GastosDB por un nombre inexistente en la cadena de conexión y usar la opción 2: el programa muestra "Error al listar: ..." y vuelve al menú sin cerrarse.
10. Para terminar, elegir la opción 6.
