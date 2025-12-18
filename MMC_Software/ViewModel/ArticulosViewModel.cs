using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMC_Software.ViewModel
{
    public class ArticulosViewModel : INotifyPropertyChanged
    {
        public int ArticulosID { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ArticulosViewModel(int articulosID)
        {
            ArticulosID = articulosID;
        }

        public ArticulosViewModel()
        {

        }

        //Intento de push
    }

}
