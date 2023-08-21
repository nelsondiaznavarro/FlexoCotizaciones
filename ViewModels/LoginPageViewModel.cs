using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexoCotizaciones.ViewModels;

public partial class LoginPageViewModel : BaseViewModel
{
    [ObservableProperty]
    public string _userName;
    [ObservableProperty]
    public string _password;
}
