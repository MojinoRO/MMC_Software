using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMC_Software
{
    internal class Notas
    {

        ///ESTA SECCION ES PARA GUARDAR DATOS IMPORTANTES DE MI CODIGO 
        ///
        /// 
        ///                      PARA COMPRAS- VENTAS-DV ETC
        /// 1) la mejor recomendacion que puedo darte es que uses MVVM para separar la logica de la interfaz gráfica
        /// 2) usa Entity Framework para manejar la base de datos de manera mas eficiente
        /// 3) para cargar documentos que cada ventana se independiente y no dependa de otra ventana 
        /// 4) revisar los evento que maneja los data grid para evitar errores al cargar datos como las subcripciones etc , ejemplo mi ventana de compras
        /// consejo practico leer el codigo y entender cada linea   
        /// 


        ///como funciona el sqltransaction como sobrecarga de parametros en c#
        ///en cada herencia de la clase que llame al contructor depende el que use  uno limpia los objetos de conn y transacction por que los voy a crear en los metodos
        ///el otro me llena la varibale con las que llame la clase y lo que le pase por parametros 
        ///


        // InotifyPropertyChanged 
        //
        // cuando se usa un I notifyproperTYChanged se debe implementar siempre un pulic event asi:
        //
        // public event PropertychangedEventhandles PropertyChanged <== nombre del evento
        //
        // tambien debe tener una propiedad protegioda que va ser la que se va invokar y debe tener argumentos pero depende de la propiedad los parametros son distinos 
        // 
        // propreted void  OnpropertyChangedOn([system.runtime.compilerservices.callmermbername] string Name =NULL)
        // {
        //      PropertyChanged?.inkove(this.new propertyChangedeventargs(name));
        // }

        /*public ICommand BuscarDocumento { get; }


        Qué es:
        Es una propiedad pública de tipo ICommand. En MVVM, todos los comandos que quieres exponer a la vista (XAML) deben ser públicos para que el binding funcione.

        Para qué sirve:

        Permite que tu XAML se conecte a una acción del ViewModel.

        En tu ejemplo, el KeyBinding de F1 en el TextBox hace Command="{Binding BuscarDocumento}".

        WPF ve esta propiedad, y cuando se dispara la acción (F1), llama al método que asignaste en el constructor (EjecutarBuscarDocumento) a través del RelayCommand.

        Por qué no es void ni event:

        No es un método directo, sino un objeto que implementa ICommand.

        ICommand tiene dos cosas importantes:

        Execute(object parameter) → lo que se ejecuta cuando se dispara el comando.

        CanExecute(object parameter) → determina si el comando está habilitado.

        En resumen, esta línea es la “puerta de enlace” entre tu XAML y el código de tu ViewModel. Sin esa propiedad, el binding del comando no funcionaría.*/


        //                                 EVENTOS CON LAS PROPIEDADES DE VIEW CON MVVM

        /*🎯 RESUMEN PRÁCTICO
        Tarea	                       ¿Cómo se hace en MVVM?
        Click	                        Command
        Habilitar/Deshabilitar	        Bind a IsEnabled
        Ocultar/mostrar	                Bind a Visibility
        Cambiar color	                Setter + Trigger o propiedad bindada
        Hover, pressed	                XAML Triggers */


        // PARA LA TABLA DE  INVMOVINVENTARIOS EL CAMPO DE TIPOMOVIMIENTO ES IMPORTANTE YA QUE NOS DEFINE DONDE SE VA USAR 
        //2 = SALIDA
        //1 = ENTRADA

    }
}

